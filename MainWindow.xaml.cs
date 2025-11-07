using System.Windows;
using System.Windows.Controls;
using GdzApp.Pages;
using GdzApp.Models;

namespace GdzApp
{
    public partial class MainWindow : Window
    {
        public User? CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage(this));
        }

        public void LoginSuccess(User user)
        {
            CurrentUser = user;
            if (user.IsAdmin)
            {
                MainFrame.Navigate(new AdminPage(this));
            }
            else
            {
                MainFrame.Navigate(new CatalogPage(this));
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser = null;
            MainFrame.Navigate(new LoginPage(this));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack) MainFrame.GoBack();
        }
    }
}