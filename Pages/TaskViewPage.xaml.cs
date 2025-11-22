using System.Windows;
using System.Windows.Controls;
using GdzApp.Data;
using GdzApp.Models;

namespace GdzApp.Pages
{
    public partial class TaskViewPage : Page
    {
        private MainWindow _main;
        private Textbook _textbook;

        public TaskViewPage(MainWindow main, Textbook textbook)
        {
            InitializeComponent();
            _main = main;
            _textbook = textbook;

            TitleText.Text = $"Задания: {textbook.Subject} ({textbook.Authors})";
            LoadTasks();
        }

        private void LoadTasks()
        {
            var tasks = Database.GetTasksByTextbook(_textbook.Id);
            TasksList.ItemsSource = tasks;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}