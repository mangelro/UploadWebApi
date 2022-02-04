/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:48:45
 *
 */

using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Dapper;

using FundacionOlivar.Modelos.ModelView;
using FundacionOlivar.Validacion;

using UploadWebApi.Aplicacion.Modelo;
using UploadWebApi.Aplicacion.Stores;
using UploadWebApi.Infraestructura.Compresion;
using UploadWebApi.Infraestructura.Datos.Excepciones;
using UploadWebApi.Infraestructura.SqlBinaryStream;



namespace UploadWebApi.Infraestructura.Datos
{
    /// <summary>
    /// http://florianreischl.blogspot.com/2011/08/streaming-sql-server-varbinarymax-in.html
    /// </summary>
    public class DapperHuellaAceiteStore : IHuellaAceiteStore
    {

        readonly IStoreConfiguration _config;
        readonly IFiltroCompresion _compresion;

        public DapperHuellaAceiteStore(IStoreConfiguration config, IFiltroCompresion compresion)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _compresion = compresion ?? throw new ArgumentNullException(nameof(compresion));
        }

        public async Task CreateAsync(HuellaAceite huella, Stream huellaRaw)
        {


            string sqlString = @"INSERT INTO [inter_HuellasAceite] 
                 ([IdMuestra]
                ,[FechaAnalisis]
                ,[NombreFichero]
                ,[Hash]
                ,[AppCliente]
                ,[Propietario]
                ,[Observaciones]
                ,[VectorReferencia]) 
                 VALUES (@IdMuestra
                ,@FechaAnalisis
                ,@NombreFichero
                ,@Hash
                ,@AppCliente
                ,@Propietario
                ,@Observaciones
                ,@VectorReferencia);
                SELECT CAST(SCOPE_IDENTITY() as int)";


            try
            {
                using (TransactionScope tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var connection = new SqlConnection(_config.ConnectionString))
                    {
                        connection.Open();

                        huella.IdHuella = await connection.QueryFirstOrDefaultAsync<int>(sqlString, huella);
                    }


                    await WriteHuellaRawAsync(huella.IdHuella, huellaRaw);

                    tran.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                System.Diagnostics.Debug.Write("[DapperHuellasStore.CreateAsync] " + ex.Message);
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 2627)
            {
                //Infraccion de unique key IX_inter_HuellasAceite
                throw new MuestraDuplicadaException($"La Muestra '{huella.IdMuestra}' ya existe en el sistema.");
            }
        }

        public async Task DeleteAsync(int idHuella)
        {
            string sqlString = @"DELETE FROM [inter_HuellasAceite] 
               WHERE IdHuella=@IdHuella";

            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                connection.Open();
                await connection.ExecuteAsync(sqlString, new { IdHuella = idHuella });
            }
        }

        public async Task<IQueryResult<HuellaAceite>> ReadAllAsync(RangoPaginacion paginacion, Guid idUsuario, Guid idAplicacion, OrdenType orden = OrdenType.DESC)
        {
            StringBuilder sqlString = new StringBuilder();
            sqlString.AppendFormat(@"SELECT
                 [IdHuella]
                ,[IdMuestra]
                ,[FechaAnalisis]
                ,[NombreFichero]
                ,[Hash]
                ,[AppCliente]
                ,[Propietario]
                ,[VectorReferencia]
                FROM (SELECT *, ROW_NUMBER() over(PARTITION BY AppCliente ORDER BY FechaAnalisis {0}) RowNum 
                    FROM [inter_HuellasAceite]
                    WHERE", orden.ToString().ToUpper());

            sqlString.Append(" [AppCliente] = @AppCliente");

            if (idUsuario != Guid.Empty)
            {
                sqlString.Append(" AND [Propietario]=@Propietario");
            }
            sqlString.Append(" ) Q1");
            sqlString.Append(" WHERE Q1.RowNum >= @RowFrom AND Q1.RowNum < @RowTo ORDER BY Q1.RowNum ASC;");

            sqlString.Append(" SELECT COUNT(*) FROM [inter_HuellasAceite] WHERE [AppCliente] = @AppCliente");
            if (idUsuario != Guid.Empty)
            {
                sqlString.Append(" AND [Propietario]=@Propietario");
            }

            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                connection.Open();

                using (var multi = connection.QueryMultiple(sqlString.ToString(),
                    new
                    {
                        AppCliente = idAplicacion,
                        Propietario = idUsuario,
                        RowFrom = paginacion.From,
                        RowTo = paginacion.To
                    }))
                {
                    var dtos = (await multi.ReadAsync<HuellaAceite>()).ToList();

                    int count = (await multi.ReadAsync<int>()).Last();

                    return new QueryResult<HuellaAceite>(dtos, count);
                }
            }
        }

        public async Task<HuellaAceite> ReadAsync(string idMuestra, Guid idUsuario, Guid idAplicacion)
        {
            StringBuilder sqlString = new StringBuilder(@"SELECT
                  h.[IdHuella]
                ,h.[IdMuestra]
                ,h.[FechaAnalisis]
                ,h.[NombreFichero]
                ,h.[Hash]
                ,h.[AppCliente]
                ,h.[FechaBloqueo]                
                ,h.[Propietario]
                ,h.[Observaciones]
                ,p.[NombrePanel] NombrePropietario
                ,h.[VectorReferencia]
                FROM [inter_HuellasAceite] h JOIN [inter_Paneles] p ON h.Propietario=p.IdUsuario
                WHERE h.IdMuestra=@IdMuestra AND h.AppCliente=@AppCliente");

            if (idUsuario != Guid.Empty)
            {
                sqlString.Append(" AND [Propietario]=@Propietario");
            }

            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                connection.Open();

                var huella = await connection.QueryFirstOrDefaultAsync<HuellaAceite>(sqlString.ToString(), new { IdMuestra = idMuestra, AppCliente = idAplicacion, Propietario = idUsuario });

                huella.ThrowIfNull(new IdNoEncontradaException($"La Muestra {idMuestra} no existe en el sistema"));

                return huella;
            }
        }

        public async Task<HuellaAceite> ReadAsync(int idHuella)
        {
            StringBuilder sqlString = new StringBuilder(@"SELECT
                 h.[IdHuella]
                ,h.[IdMuestra]
                ,h.[FechaAnalisis]
                ,h.[NombreFichero]
                ,h.[Hash]
                ,h.[AppCliente]
                ,h.[FechaBloqueo]                
                ,h.[Propietario]
                ,h.[Observaciones]
                ,p.[NombrePanel] NombrePropietario
                ,h.[VectorReferencia]
                FROM [inter_HuellasAceite] h JOIN [inter_Paneles] p ON h.Propietario=p.IdUsuario
                WHERE h.IdHuella=@IdHuella");


            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                connection.Open();

                var huella = await connection.QueryFirstOrDefaultAsync<HuellaAceite>(sqlString.ToString(), new { IdHuella = idHuella });

                huella.ThrowIfNull(new IdNoEncontradaException($"La Huella {idHuella} no existe en el sistema"));

                return huella;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idHuella"></param>
        /// <returns></returns>
        public async Task BloquearAsync(int idHuella, DateTime fechaBloqueo)
        {
            string sqlString = @"UPDATE [inter_HuellasAceite]
                                SET [FechaBloqueo]=@Fecha,
                                [FechaModificacionUTC]=GETUTCDATE()
                                WHERE IdHuella=@IdHuella AND FechaBloqueo IS NULL";

            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                connection.Open();

                await connection.ExecuteAsync(sqlString, new { IdHuella = idHuella, Fecha = fechaBloqueo });

            }

        }






        /// <summary>
        /// Lee un fichero de huella desde el origen de datos
        /// </summary>
        /// <param name="idHuella"></param>
        /// <returns></returns>
        public Task<Stream> ReadHuellaRawAsync(int idHuella)
        {

            byte[] buffer = new byte[1024];
            int leidos = 0;

            SqlBinaryData data = SqlBinaryData.CreateIntPrimaryKey(_config.ConnectionString, "inter_HuellasAceite", "Huella", idHuella, 1024);

            using (MemoryStream writer = new MemoryStream())
            {
                using (var reader = data.OpenRead())
                {
                    while ((leidos = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, leidos);
                    }
                }
                return Task.FromResult(_compresion.Descomprimir(writer));
            }
        }

        /// <summary>
        /// Escribe un fichero de huella en el origen de datos
        /// </summary>
        /// <param name="idHuella"></param>
        /// <param name="huellaRaw"></param>
        /// <returns></returns>
        public async Task WriteHuellaRawAsync(int idHuella, Stream huellaRaw)
        {

            SqlBinaryData data = SqlBinaryData.CreateIntPrimaryKey(_config.ConnectionString, "inter_HuellasAceite", "Huella", idHuella, 1024);

            byte[] buffer = new byte[1024];
            int leidos = 0;

            using (var stream = _compresion.Comprimir(huellaRaw))
            {
                using (var writer = data.OpenWrite(true))
                {

                    while ((leidos = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, leidos);
                    }

                    //do
                    //{
                    //    leidos = stream.Read(buffer, 0, buffer.Length);
                    //    writer.Write(buffer, 0, leidos);
                    //} while (leidos == buffer.Length);
                }
            }

            huellaRaw.Seek(0, SeekOrigin.Begin);
            await Task.CompletedTask;
        }


    }
}

