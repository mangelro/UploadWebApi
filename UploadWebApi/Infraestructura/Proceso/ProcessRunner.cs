/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 13/09/2019 14:30:43
 *
 */

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UploadWebApi.Infraestructura.Proceso
{
    /// <summary>
    /// https://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why
    /// 
    /// </summary>
    public class ProcessRunner 
    {

        readonly bool _throwException;
        readonly string _exeFile;

        StringBuilder _output = new StringBuilder();
        StringBuilder _error = new StringBuilder();


        bool _isRunning = false;


        public ProcessRunner(string exeFile, bool throwException)
        {

            _exeFile = exeFile;
            _throwException = throwException;
        }


        Process GetProcess(ProcessStartInfo info)
        {
            var process = new Process
            {
                StartInfo = info,
            };

            return process;
        }

        ProcessStartInfo GetProcessStartInfo(string exeFile, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exeFile,
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            //startInfo.StandardErrorEncoding = System.Text.Encoding.ASCII;
            //startInfo.StandardOutputEncoding = System.Text.Encoding.ASCII;

            return startInfo;
        }

        public int Timeout { get; set; } = -1;

        public string Respuesta => _output.ToString();

        public string Error => _error.ToString();


        public int Run(string args)
        {
            _isRunning = true;

            return InternalRun(args);
        }


        public Task<int> RunAsync(string args, CancellationToken token =default(CancellationToken))
        {
            _isRunning = true;

            return Task.Factory.StartNew<int>(() =>
            {
                return InternalRun(args);

            }, cancellationToken:token);
        }


        public bool IsRunning => _isRunning;



        int InternalRun(string args)
        {
            using (var process = GetProcess(GetProcessStartInfo(_exeFile,args)))
            {

                process.EnableRaisingEvents = true;

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.Exited += (sender, e) =>
                    {
                        Debug.WriteLine($"Exited {Thread.CurrentThread.ManagedThreadId.ToString()} [{DateTime.Now.ToString("HH:mm:ss.ffff")}]");
                        _isRunning = false;
                    };

                    process.OutputDataReceived += (sender, e) =>
                    {
                        Debug.WriteLine($"OutputDataReceived {Thread.CurrentThread.ManagedThreadId.ToString()} [{DateTime.Now.ToString("HH:mm:ss.ffff")}]");

                        if (e.Data == null)
                            outputWaitHandle.Set();
                        else
                            _output.AppendLine(e.Data);
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        Debug.WriteLine($"ErrorDataReceived {Thread.CurrentThread.ManagedThreadId.ToString()} [{DateTime.Now.ToString("HH:mm:ss.ffff")}]");

                        if (e.Data == null)
                            errorWaitHandle.Set();
                        else
                            _error.AppendLine(e.Data);
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (process.WaitForExit(Timeout==-1?int.MaxValue:Timeout) &&
                        outputWaitHandle.WaitOne(Timeout) &&
                        errorWaitHandle.WaitOne(Timeout))
                    {
                        // Process completed. Check process.ExitCode here.
                        if (_throwException && !String.IsNullOrEmpty(Error))
                        {
                            throw new Exception(Error);
                        }

                        return process.ExitCode;
                    }
                    else
                    {
                        throw new TimeoutException();
                    }
                }
            }

        }

    }
}