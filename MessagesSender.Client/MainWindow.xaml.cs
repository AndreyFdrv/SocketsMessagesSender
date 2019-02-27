using System.Windows;

namespace MessagesSender.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client Client;
        public MainWindow()
        {
            InitializeComponent();
            Client = new Client();
            if (Client.Start())
            {
                lblStatus.Content = "Подключение к серверу установлено";
                btnConnect.Visibility = Visibility.Hidden;
            }
        }
        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (Client.Start())
            {
                lblStatus.Content = "Подключение к серверу установлено";
                btnConnect.Visibility = Visibility.Hidden;
            }
            else
                MessageBox.Show("Не удалось подключиться к серверу");
        }
        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            var message = txtInput.Text;
            var errorMessage = Client.SendMessage(message);
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage);
                return;
            }
            txtMessages.Text += (message + "\r\n");
        }
    }
}