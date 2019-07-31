/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 02/07/2019 9:18:13
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UploadWebApi
{


    /// <summary>
    /// 
    /// </summary>
    public class ProcessRunner:IDisposable
    {

        public static readonly string[] EmptyArgsList = new string[0];

        private readonly ManualResetEvent _mreProcessExit = new ManualResetEvent(true);
        private readonly ManualResetEvent _mreOutputDone = new ManualResetEvent(true);
        private readonly ManualResetEvent _mreErrorDone = new ManualResetEvent(true);

        readonly string _executable;
        readonly string[] _argsList;
        Process _running;
        TextWriter _stdIn;
        string _workingDirectory;
        int _exitCode;
        bool _isRunning;





        public ProcessRunner(string executable):this(executable, EmptyArgsList) { }
        

        public ProcessRunner(string executable, params string[] args)
        {
            _executable = FindExePath(executable);
            _argsList = args?? EmptyArgsList;

            _stdIn = null;
            _exitCode = 0;
            _isRunning = false;
            _running = null;

        }

        /// <summary>
        /// Identificador de proceso
        /// </summary>
        public int PID => _running.Id;


        /// <summary>
        /// Obtiene o establece el directorio de trabajo del proceso
        /// </summary>
        public string WorkingDirectory
        {
            get => _workingDirectory ?? Environment.CurrentDirectory;
            set => _workingDirectory = value;
        }

        /// <summary>
        /// Espera al proceso y devueve el código de salida
        /// </summary>
        public int ExitCode
        {
            get
            {
                WaitForExit();
                return _exitCode;
            }
        }

        /// <summary>
        /// Mata el proceso si aún esta en ejecución
        /// </summary>
        public void Kill()
        {

        }

        /// <summary>
        /// Cierra Std:in y espera al proceso
        /// </summary>
        public void WaitForExit()
        {
            WaitForExit(TimeSpan.MaxValue, true);
        }

        /// <summary>
        /// Cierra Std:In y espera al proceso para salir. Retorna falso si el proceso no termina en el tiempo especificado
        /// </summary>
        /// <param name="timeout"></param>
        public bool WaitForExit(TimeSpan timeout)
        {
            return WaitForExit(timeout, true);
        }

        /// <summary>
        /// Espera al proceso para salir. Retorna falso si el proceso no termina en el tiempo especificado
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="closeStdInput"></param>
        public bool WaitForExit(TimeSpan timeout,bool closeStdInput)
        {
            if (_stdIn != null && closeStdInput)
            {
                _stdIn.Close();
                _stdIn = null;
            }

            int waitTime = timeout.TotalMilliseconds >= int.MaxValue ? -1 : (int)timeout.TotalMilliseconds;

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                if (!_mreProcessExit.WaitOne(waitTime, false))
                    return false;
                if (!_mreErrorDone.WaitOne(waitTime, false))
                    return false;
                if (!_mreOutputDone.WaitOne(waitTime, false))
                    return false;


                return true;
            }


            bool exited = WaitHandle.WaitAll(new WaitHandle[] { _mreErrorDone, _mreOutputDone, _mreProcessExit }, waitTime, false);

            return exited;

        }


        /// <summary>
        /// Retorna verdadero si la instancia esta corriento un proceso
        /// </summary>
        public bool IsRunning => _isRunning && !WaitForExit(TimeSpan.Zero, false);


        /// <summary>
        /// Ejecuta el proceso y retorna el código de salida
        /// </summary>
        /// <returns></returns>
        public int Run() => Run(null, EmptyArgsList);


        /// <summary>
        /// Ejecuta el proceso con argumentos adicionales y retorna el código de salida
        /// </summary>
        /// <param name="moreArgs"></param>
        /// <returns></returns>
        public int Run(params string[] moreArgs) => Run(null, moreArgs);

        /// <summary>
        /// Ejecuta el proceso con argumentos adicionales y retorna el código de salida
        /// </summary>
        /// <param name="input"></param>
        /// <param name="moreArgs"></param>
        /// <returns></returns>
        public int Run(TextReader input, params string[] arguments)
        {
            List<string> args = new List<string>(_argsList);

            args.AddRange(arguments ?? EmptyArgList);

            return InternalRun(input, args.ToArray());
        }




        public int RunFormatArgs(params object[] formatArgs)
        {

            List<string> args = new List<string>();

            foreach (var a in _argsList)
                args.Add(String.Format(a, formatArgs));



        }


































        /// <summary>
        /// Expands environment variables and, if unqualified, locates the exe in the working directory
        /// or the evironment's path.
        /// </summary>
        /// <param name="exe">The name of the executable file</param>
        /// <returns>The fully-qualified path to the file</returns>
        /// <exception cref="System.IO.FileNotFoundException">Raised when the exe was not found</exception>
        static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
        }

    }
}