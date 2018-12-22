using System.Windows;

namespace PexesoClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class Login_Window : Window
    {
        private MainWindow _mainWindow;
        public Login_Window(MainWindow mainWindow, string buttonContext)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Button_LogIn.Content = buttonContext;
        }

        private void Button_LogIn_Click(object sender, RoutedEventArgs e)
        {
            if (Button_LogIn.Content.ToString() == "Log in")
            {
                if (_mainWindow.LogIn(TextBox_NickName.Text, TextBox_Password.Password))
                    Close();
            }
            else if (_mainWindow.CreateAccount(TextBox_NickName.Text, TextBox_Password.Password))
            {
                Close();
            }

        }
    }
}
