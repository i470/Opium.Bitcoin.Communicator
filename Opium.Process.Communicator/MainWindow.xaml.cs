using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;


namespace Opium.Shell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        String executableLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        String exeLocation;
        String coin_cli_name;
       
        public MainWindow()
        {
            InitializeComponent();
            var exedir= Path.Combine(executableLocation ?? throw new InvalidOperationException(), "exe\\");
            coin_cli_name = "btcp-cli.exe";
            exeLocation = Path.Combine(exedir ?? throw new InvalidOperationException(), coin_cli_name);

            lstCoinListBox.Items.Add("XCoin");
            lstCoinListBox.Items.Add("Bitcoin Private");
            lstCoinListBox.Items.Add("Bitcoin");
            lstCoinListBox.Items.Add("Bitcoin Cash");
            lstCoinListBox.Items.Add("Bitcoin Gold");
            lstCoinListBox.Items.Add("ZCash");
            lstCoinListBox.Items.Add("Hush");
            lstCoinListBox.Items.Add("No Shit! Can add my own.....coin?");
            startCommunicator();
        }

        private void startCommunicator()
        {
           
            var bitcoinProcesses = System.Diagnostics.Process.GetProcesses().FirstOrDefault(pr => pr.ProcessName.Contains("btcp"));

            if (bitcoinProcesses != null)
            {
                txtInfo.Content += "Node is running => " + bitcoinProcesses.ProcessName;
            }
         
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            button.IsEnabled = false;
            var command = txtInput.Text.Trim();
            var progress = new Progress<String>();

            progress.ProgressChanged += (s, r) =>
            {
                DisplayResults(r);
            };

            
            await Task.Factory.StartNew(() =>
            {
                ExecuteCommandAsync(command, progress).ConfigureAwait(false);
            });

           
            button.IsEnabled = true;
        }


        Task ExecuteCommandAsync(string command, IProgress<string> progress)
        {
            progress?.Report("Executing command: "+command+"\n");

            // there is no non-generic TaskCompletionSource
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                if (string.IsNullOrEmpty(command))
                    command = "getinfo";


            
                var proc = new System.Diagnostics.Process
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
                progress?.Report("Start "+coin_cli_name+" process..........\n");
                
                proc.Start();

                progress?.Report("Send command to the " + coin_cli_name + " node..........waiting for response\n");

                var response = proc.StandardOutput.ReadToEnd();

                progress?.Report("Reading response from the " + coin_cli_name + " node ..........\n");
                proc.Dispose();

                DisplayResults(response);

            }
            catch (Exception e)
            {
                DisplayResults("Error from " + coin_cli_name +"\n");
                DisplayResults(e.Message);

            }
           
            return tcs.Task;
        }

         void DisplayResults(string response)
        {
           Dispatcher.BeginInvoke((Action)(() => {
                txtOutput.Text += response;
            }));
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var command = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(command))
                command = "getinfo";

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var exeFile = Path.Combine(directory ?? throw new InvalidOperationException(), "exe\\btcp-cli.exe");
            
            var response = FSharp.runProc(exeFile, command, directory);

            StringBuilder sb = new StringBuilder();
            foreach (var line in response.Item1)
            {
                sb.AppendLine(line);
            }

            foreach (var line in response.Item2)
            {
                sb.AppendLine(line);
            }

            DisplayResults(sb.ToString());

        }
    }
}
