using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Opium.Communicator.CSharp
{
    public static class Runner
    {
        public static Task runProcAsync(string exeLocation, string command)
        {
            
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                if (string.IsNullOrEmpty(command))
                    command = "getinfo";

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exeLocation,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true,
                        Arguments = command
                    },
                    EnableRaisingEvents = true
                };
                
                proc.Start();

                var response = proc.StandardOutput.ReadToEnd();

                proc.Dispose();

                DisplayResults(response);

            }
            catch (Exception e)
            {
               DisplayResults(e.Message);
            }

            return tcs.Task;
        }

       


        static String DisplayResults(string response)
        {
            return response;
        }
    }
}
