using System.Windows;
using System.Windows.Controls;
using GdzApp.Data;
using GdzApp.Models;

namespace GdzApp.Pages
{
    public partial class RegisterPage : Page
    {
        private MainWindow _main;
        public RegisterPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text.Trim();
            var pass = PasswordBox.Password;
            var email = EmailBox.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pass))
            {
                Msg.Text = "Логин и пароль обязательны.";
                return;
            }

            var existing = Database.GetUserByUsername(username);
            if (existing != null)
            {
                Msg.Text = "Пользователь с таким логином уже существует.";
                return;
            }

            int cls = 0;
            int.TryParse(ClassBox.Text.Trim(), out cls);

            var user = new User
            {
                Username = username,
                Password = pass,
                Email = email,
                EmailNotifications = NotifyCheck.IsChecked == true,
                IsAdmin = false,
                Class = cls
            };

            Database.InsertUser(user);

            MessageBox.Show("Регистрация успешна. Войдите под своим логином.");
            NavigationService?.Navigate(new LoginPage(_main));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}