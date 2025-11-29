using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
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

            // Добавляем номера заданиям как на 3-м изображении
            var numberedTasks = tasks.Select((task, index) => new NumberedTaskItem
            {
                Id = task.Id,
                TextbookId = task.TextbookId,
                Title = task.Title,
                SolutionImageUrl = task.SolutionImageUrl,
                SolutionText = task.SolutionText,
                Number = (index + 1).ToString() // Нумерация 1, 2, 3...
            }).ToList();

            TasksList.ItemsSource = numberedTasks;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }

    // Класс для заданий с номерами
    public class NumberedTaskItem : TaskItem
    {
        public string Number { get; set; } = "";
    }
}