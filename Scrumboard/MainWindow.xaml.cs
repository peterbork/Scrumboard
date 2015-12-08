using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//my imports
using System.Windows.Threading;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Scrumboard {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        StreamWriter sWriter;
        ThreadMonitor threadMonitor = new ThreadMonitor("Client");

        public MainWindow() {
            InitializeComponent();

            // disable console
            tb_console.IsEnabled = false;

            for (int i = 0; i < 3; i++) {
                ListBoxItem lbi = new ListBoxItem();
                lbi.Width = 100;
                lbi.Height = 100;
                lbi.Content = "test";
                lbi.Background = Brushes.Red;
                ListBoxProductBacklog.Items.Add(lbi);
            }

            #region Socket Setup


            threadMonitor.ThreadEvent += Thread_MessageReciever;
            TcpClient tcpClient = new TcpClient("25.106.204.166", 1234);
            //tb_console.Text += "Connected to server.";
            sWriter = new StreamWriter(tcpClient.GetStream());
            Thread thread = new Thread(Read);
            thread.Start(tcpClient);
            #endregion

        }
        #region Socket

        private void Read(object obj) {

            sWriter.WriteLine("Console|swag");
            sWriter.Flush();

            TcpClient tcpClient = (TcpClient)obj;
            StreamReader sReader = new StreamReader(tcpClient.GetStream());

            while (true) {
                string message = sReader.ReadLine();
                List<string> commands = message.Split('|').ToList<string>();
                string command = commands[0];
                commands.RemoveAt(0);

                if (command == "Console") {
                    threadMonitor.ThreadAction(commands[0]);
                }
            }
        }

        private void Thread_MessageReciever(string message) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(Thread_MessageReciever), message);
                return;
            }
            tb_console.Text += message + "\n";
        }
        #endregion

        private void btn_chat_Click(object sender, RoutedEventArgs e) {
            Chat chatWindow = new Chat(TcpClient tcpClient);
        }
    }
}
