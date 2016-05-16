using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityWebPlayerAXLib;

namespace WpfRecognize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RecognizationCore _core = new RecognizationCore();
        const int WM_SETTEXT = 0x000C;
        private Socket socketClient;
        private bool isConnect = false;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = _core;

            ConnectServer();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            CloseSocket();
            base.OnClosing(e);
        }

        public void CloseSocket()
        {
            if (isConnect)
            {
                socketClient.Send(Encoding.UTF8.GetBytes("CLS"));
                socketClient.Close();
                isConnect = false;
            }
        }

        private void ConnectServer()
        {
            try {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketClient.Connect(IPAddress.Parse("127.0.0.1"), 6666);
                isConnect = true;
            }
            catch
            {
                isConnect = false;
            }
        }

        private void WritingCanvasOnStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            _core.Recognize(this.WritingCanvas.Strokes);
        }

        private void RecognizerSelectorOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Clear();
        }

        private void ClearButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Clear();
        }

        private void SelectCharactorButtonOnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var text = button.Content as string;
            if (text != null)
            {
                if (isConnect)
                {
                    socketClient.Send(Encoding.UTF8.GetBytes(text));
                }
                this.Clear();
            }
        }

        private void Clear()
        {
            this.WritingCanvas.Strokes.Clear();
            _core.ClearAlternates();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (isConnect)
            {
                socketClient.Send(Encoding.UTF8.GetBytes("BACK"));
            }
        }

        private void FontButton_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            var text = button.Content as string;
            if (text != null)
            {
                if (isConnect)
                {
                    socketClient.Send(Encoding.UTF8.GetBytes(text));
                }
                this.Clear();
            }
        }

        private void MainLoad(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process[] myProcess = System.Diagnostics.Process.GetProcessesByName("map");
            //if (myProcess.Length <= 0)
            //{
            //    Application.Current.Shutdown();
            //}
        }

        private void PinYin(object sender, RoutedEventArgs e)
        {
            if (isConnect)
            {
                socketClient.Send(Encoding.UTF8.GetBytes("PINY"));
            }
        }
    }
}
