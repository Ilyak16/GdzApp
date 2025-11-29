using System.Windows;
using System.Windows.Controls;
using GdzApp.Models;

namespace GdzApp.Pages
{
    public partial class TaskSolutionPage : Page
    {
        private MainWindow _main;
        private TaskItem _task;

        public TaskSolutionPage(MainWindow main, TaskItem task)
        {
            InitializeComponent();
            _main = main;
            _task = task;

            TitleText.Text = $"Задание {task.Title}";
            DataContext = task; // Привязываем данные задания
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}