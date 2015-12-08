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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        ThreadMonitor threadMonitor = new ThreadMonitor();
        private static TcpListener tcpListener;
        private static List<TcpClient> tcpClientList = new List<TcpClient>();

        public MainWindow() {
            InitializeComponent();

            tb_console.IsEnabled = false;

            threadMonitor.ThreadEvent += Thread_MessageReciever;

            tcpListener = new TcpListener(IPAddress.Any, 1234);
            tcpListener.Start();

            tb_console.Text += "Server started\n";
            Thread acceptingClientsThread = new Thread(AcceptingClients);
            acceptingClientsThread.Start();
        }

        public void AcceptingClients() {
            while (true) {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpClientList.Add(tcpClient);

                Thread thread = new Thread(ClientListener);
                thread.Start(tcpClient);
            }
        }

        public void ClientListener(object obj) {
            TcpClient tcpClient = (TcpClient)obj;
            StreamReader reader = new StreamReader(tcpClient.GetStream());
            //tb_console.Text = "Client connected! " + tcpClientList.Count + " Client(s) Connected.";
            //tb_console.Text = "yolo";
            threadMonitor.ThreadAction("connected user");

            while (true) {
                string message = reader.ReadLine();
                List<string> commands = message.Split('|').ToList<string>();
                string command = commands[0];
                commands.RemoveAt(0);
                if (command == "Console") {
                    BroadCastForOne("Console| " + commands[0], tcpClient);
                }
                else {
                    threadMonitor.ThreadAction(message);
                }
            }
        }
        #region Broadcast
        public static void BroadCastForOne(string msg, TcpClient sender) {
            for (int i = 0; i < tcpClientList.Count; i++) {
                if (tcpClientList[i] == sender) {
                    StreamWriter sWriter = new StreamWriter(tcpClientList[i].GetStream());
                    sWriter.WriteLine(msg);
                    sWriter.Flush();
                }
            }
        }
        public static void BroadCast(string msg, TcpClient Sender) {
            for (int i = 0; i < tcpClientList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(tcpClientList[i].GetStream());
                sWriter.WriteLine(msg);
                sWriter.Flush();
            }
        }
        //public void BroadCastForNew(TcpClient newClient) {
        //    for (int i = 0; i < tcpClientList.Count; i++) {
        //        StreamWriter sWriter = new StreamWriter(tcpClientList[i].GetStream());
        //        if (tcpClientList[i] == newClient) {

        //        }
        //    }
        //}
        #endregion

        public void Thread_MessageReciever(string message) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(Thread_MessageReciever), message);
                return;
            }
            tb_console.Text += message;
        }
    }
}
