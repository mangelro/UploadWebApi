/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 04/09/2019 10:48:45
 *
 */

using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using UploadWebApi.Infraestructura.SqlBinaryStream;
using UploadWebApi.Models;



namespace UploadWebApi.Applicacion.Stores
{
    /// <summary>
    /// http://florianreischl.blogspot.com/2011/08/streaming-sql-server-varbinarymax-in.html
    /// </summary>
    public class DapperHuellasStore : IHuellasStore
    {

        readonly IStoreConfiguration _config;
        readonly IFiltroCompresion _compresion;

        public DapperHuellasStore(IStoreConfiguration config, IFiltroCompresion compresion)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _compresion= compresion ?? throw new ArgumentNullException(nameof(compresion));
        }

        public async Task CreateAsync(HuellaDto huella, byte[] huellaRaw)
        {


            string sqlString = @"INSERT INTO [inter_HuellasAceite] 
                 ([IdMuestra]
                ,[FechaAnalisis]
                ,[NombreFichero]
                ,[Hash]
                ,[AppCliente]
                ,[Propietario]
                ,[Observaciones]) 
                 VALUES (@IdMuestra
                ,@FechaAnalisis
                ,@NombreFichero
                ,@Hash
                ,@AppCliente
                ,@Propietario
                ,@Observaciones);
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

        public async Task<Tuple<IEnumerable<HuellaDto>, int>> ReadAllAsync(int pageNumber, int pageSize, Guid idUsuario, Guid idAplicacion, OrdenListatoTipo orden = OrdenListatoTipo.DESC)
        {

            if (pageNumber < 1)
                throw new ArgumentException("El número de página ha de ser mayor que 0.");

            if (pageSize < 1)
                throw new ArgumentException("El tamaño de página ha de ser mayor que cero");

            int rowFrom = ((pageNumber - 1) * pageSize) + 1;
            int rowTo = rowFrom + pageSize;

            StringBuilder sqlString = new StringBuilder();
            sqlString.AppendFormat(@"SELECT
                 [IdHuella]
                ,[IdMuestra]
                ,[FechaAnalisis]
                ,[NombreFichero]
                ,[Hash]
                ,[AppCliente]
                ,[Propietario]
                FROM (SELECT *, ROW_NUMBER() over(PARTITION BY AppCliente ORDER BY FechaAnalisis {0}) RowNum 
                    FROM [inter_HuellasAceite]
                    WHERE", orden.ToString().ToUpper());

            sqlString.Append(" [AppCliente] = @AppCliente");

            if (idUsuario!=Guid.Empty)
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
                        RowFrom = rowFrom,
                        RowTo = rowTo
                    }))
                {
                    var dtos = (await multi.ReadAsync<HuellaDto>()).ToList();

                    int count =(await multi.ReadAsync<int>()).Last();

                    return Tuple.Create<IEnumerable<HuellaDto>, int>(dtos, count);
                }
            }
        }

        public async Task<HuellaDto> ReadAsync(string idMuestra, Guid idUsuario, Guid idAplicacion)
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
                ,p.[NombrePanel] NombrePropietario
                FROM [inter_HuellasAceite] h JOIN [inter_Paneles] p ON h.Propietario=p.IdUsuario
                WHERE h.IdMuestra=@IdMuestra AND h.AppCliente=@AppCliente");

            if (idUsuario != Guid.Empty)
            {
                sqlString.Append(" AND [Propietario]=@Propietario");
            }

            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                connection.Open();

                var dto = await connection.QueryFirstOrDefaultAsync<HuellaDto>(sqlString.ToString(), new { IdMuestra = idMuestra, AppCliente = idAplicacion, Propietario=idUsuario });

                return dto;
            }
        }

        public async Task<HuellaDto> ReadAsync(int idHuella)
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
                FROM [inter_HuellasAceite] h JOIN [inter_Paneles] p ON h.Propietario=p.IdUsuario
                WHERE h.IdHuella=@IdHuella");
      

            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                connection.Open();

                var dto = await connection.QueryFirstOrDefaultAsync<HuellaDto>(sqlString.ToString(), new { IdHuella = idHuella});

                return dto;
            }
        }
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idHuella"></param>
        /// <returns></returns>
        public async Task BloquearAsync(int idHuella,DateTime fechaBloqueo)
        {
            string sqlString = @"UPDATE [inter_HuellasAceite]
                                SET [FechaBloqueo]=@Fecha,
                                [FechaModificacionUTC]=GETUTCDATE()
                                WHERE IdHuella=@IdHuella AND FechaBloqueo IS NULL";

            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                connection.Open();

                await connection.ExecuteAsync(sqlString, new { IdHuella = idHuella, Fecha= fechaBloqueo });

            }

        }


        public Task<byte[]> ReadHuellaRawAsync(int idHuella)
        {

            byte[] buffer = new byte[1024];
            int leidos = 0;

            SqlBinaryData data = SqlBinaryData.CreateIntPrimaryKey(_config.ConnectionString, "inter_HuellasAceite", "Huella", idHuella, 128);

            using (MemoryStream writer = new MemoryStream())
            {
                using (var reader = data.OpenRead())
                {
                    do
                    {
                        leidos = reader.Read(buffer, 0, buffer.Length);
                        writer.Write(buffer, 0, leidos);
                    } while (leidos == buffer.Length);
                }
                return Task.FromResult(_compresion.Descomprimir(writer.ToArray()));
            }
        }

        public async Task WriteHuellaRawAsync(int idHuella, byte[] huellaRaw)
        {

            SqlBinaryData data = SqlBinaryData.CreateIntPrimaryKey(_config.ConnectionString, "inter_HuellasAceite", "Huella", idHuella, 1024);

            byte[] buffer = new byte[1024];
            int leidos = 0;
            using (var stream = new MemoryStream(_compresion.Comprimir(huellaRaw), false))
            {
                using (var writer = data.OpenWrite(true))
                {
                    do
                    {
                        leidos = stream.Read(buffer, 0, buffer.Length);
                        writer.Write(buffer, 0, leidos);
                    } while (leidos == buffer.Length);
                }
            }
            await Task.CompletedTask;
        }


    }
}

