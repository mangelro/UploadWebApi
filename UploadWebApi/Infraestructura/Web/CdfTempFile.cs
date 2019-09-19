/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 11/09/2019 14:11:37
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Infraestructura.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class CdfTempFile : IDisposable
    {

        readonly string _tempPath;
        readonly string _tempFileName;

        bool _creado = false;

        public CdfTempFile(string tempPath)
        {
            _tempPath = tempPath;
            _tempFileName = Path.Combine(_tempPath, GetTempFileName());
        }


        public string TempFileName
        {
            get
            {
                ThrowIfDisposed();

                if (!_creado)
                    throw new FileNotFoundException("El archivo temporal no está creado.");

                return _tempFileName;
            }
        }

        public Task CreateAsync(byte[] raw)
        {
            return Task.Factory.StartNew(() =>
            {
                Create(raw);
            });
        }


        public void Create(byte[] dataRaw)
        {
            byte[] buffer = new byte[255];

            int leidos = 0;
            using (FileStream file = new FileStream(_tempFileName, FileMode.CreateNew, FileAccess.Write))
            {
                using (var stream = new MemoryStream(dataRaw, false))
                {
                    do
                    {
                        leidos = stream.Read(buffer, 0, buffer.Length);
                        file.Write(buffer, 0, leidos);
                    } while (leidos == buffer.Length);
                }
            }
            _creado = true;
        }

        string GetTempFileName()
        {
            //return $"{Guid.NewGuid().ToString()}_TEMP.CDF";
            return $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}_TEMP.CDF";
        }

        #region Interfaz IDisposable


        /// <summary>
        /// Comrprueba que el objeto no ha sido liberado con anterioridad
        /// </summary>
        /// <param name="memberName"></param>
        private void ThrowIfDisposed([CallerMemberName]string memberName = "")
        {
            if (_disposed)
                throw new ObjectDisposedException(this.GetType().Name, String.Format("El objeto ha sido liberado [{0}]", memberName));
        }

        // Flag: Se ha liberado anteriormente?
        bool _disposed = false;

        // Implementación pública llamable desde los consumidores
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //Implementacion Protected
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    File.Delete(_tempFileName);
                }
                catch { }
                finally { _creado = false; }
            }

            // Liberación de objetos no manejados (unmanaged)
            //

            _disposed = true;

            //Descomentar en caso de herencia
            //base.Dispose(disposing);

        }


        ~CdfTempFile()
        {
            Dispose(false);
        }

        #endregion



    }
}