using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Opium.Communicator.CSharp
{
    public static class Communicator
    {
        public static Task ExecuteCLICommandAsync(string exeLocation, string command, IProgress<string> progress)
        {
            string coin_cli_name="";
           

            progress?.Report("Executing command: " + command + "\n");

            // there is no non-generic TaskCompletionSource
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
                progress?.Report("Start " + coin_cli_name + " process..........\n");

                proc.Start();

                progress?.Report("Send command to the " + coin_cli_name + " node..........waiting for response\n");

                var response = proc.StandardOutput.ReadToEnd();

                progress?.Report("Reading response from the " + coin_cli_name + " node ..........\n");
                proc.Dispose();

                DisplayResults(x=>"",response);

            }
            catch (Exception e)
            {
                DisplayResults(x=>"","Error from " + coin_cli_name + "\n");
                DisplayResults(x=>e.Message,e.Message);

            }

            return tcs.Task;
        }

       


        public static TResult DisplayResults<TResult>(Func<String,TResult> action, string response)
        {
            return action(response);
        }
    }
}
