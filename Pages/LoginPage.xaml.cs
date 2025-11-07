using System.Windows;
using System.Windows.Controls;
using GdzApp.Data;
using GdzApp.Models;

namespace GdzApp.Pages
{
    public partial class LoginPage : Page
    {
        private MainWindow _main;
        public LoginPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var user = Database.GetUserByUsername(UsernameBox.Text.Trim());
            if (user == null || user.Password != PasswordBox.Password)
            {
                Msg.Text = "Неверный логин или пароль.";
                return;
            }
            _main.LoginSuccess(user);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegisterPage(_main));
        }
    }
}