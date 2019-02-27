using System;
using System.Windows;
using System.Threading;

namespace MessagesSender.Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Status
        {
            get { return lblStatus.Content.ToString(); }
            set { Dispatcher.Invoke(new Action(() => { lblStatus.Content = value; })); }
        }
        public string Message
        {
            set { Dispatcher.Invoke(new Action(() => { txtMessages.Text += (value + "\r\n"); })); }
        }
        public MainWindow()
        {
            InitializeComponent();
            var server = new Server();
            new Thread(server.Start).Start(this);
        }
    }
}