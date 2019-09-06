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
using System.Data.SqlClient;
using System.IO;
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

        public DapperHuellasStore(IStoreConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task CreateAsync(HuellaDto huella, byte[] huellaRaw)
        {


            string sqlString = @"INSERT INTO [inter_HuellasAceite] 
                 ([IdMuestra]
                ,[FechaHuella]
                ,[NombreFichero]
                ,[Hash]
                ,[AppCliente]
                ,[Propietario]) 
                 VALUES (@IdMuestra
                ,@FechaHuella
                ,@NombreFichero
                ,@Hash
                ,@AppCliente
                ,@Propietario);
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
                // Handle exception
            }
        }

        public void Delete(int idHuella)
        {
            throw new NotImplementedException();
        }

        public async Task<HuellaDto> ReadAsync(string idMuestra, Guid aplicacion)
        {
            string sqlString = @"SELECT
                [IdHuella]
                ,[IdMuestra]
                ,[FechaHuella]
                ,[NombreFichero]
                ,[Hash]
                ,[AppCliente]
                ,[Propietario]
                FROM [inter_HuellasAceite] 
                WHERE IdMuestra=@IdMuestra AND AppCliente=@AppCliente";

            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                connection.Open();

                var  dto= await connection.QueryFirstOrDefaultAsync<HuellaDto>(sqlString, new { IdMuestra = idMuestra, AppCliente = aplicacion });

                return dto;
            }
        }

        public Task<byte[]> ReadHuellaRawAsync(int idHuella)
        {

            byte[] buffer = new byte[128];
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
                return  Task.FromResult(writer.ToArray());
            }
        }

        public async Task WriteHuellaRawAsync(int idHuella, byte[] huellaRaw)
        {

            SqlBinaryData data = SqlBinaryData.CreateIntPrimaryKey(_config.ConnectionString, "inter_HuellasAceite", "Huella", idHuella, 1024);

            byte[] buffer = new byte[512];
            int leidos = 0;
            using (var stream = new MemoryStream(huellaRaw, false))
            {
                using (var writer = data.OpenWrite(true))
                {
                    do
                    {
                        leidos=stream.Read(buffer, 0, buffer.Length);
                        writer.Write(buffer, 0, leidos);
                    } while (leidos == buffer.Length);
                }
            }
            await Task.CompletedTask;
        }


    }
}

