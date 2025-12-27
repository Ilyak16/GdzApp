using System;
using System.Windows;
using GdzApp.Models;

namespace GdzApp.Windows
{
    public partial class AddEditTaskWindow : Window
    {
        public TaskItem TheTask { get; private set; }

        public AddEditTaskWindow(int textbookId)
        {
            InitializeComponent();
            TheTask = new TaskItem { TextbookId = textbookId };
            this.Title = "Добавить задание";
        }

        public AddEditTaskWindow(TaskItem existing) : this(existing.TextbookId)
        {
            TheTask = existing;
            TitleBox.Text = existing.Title;
            ImageBox.Text = existing.SolutionImageUrl;
            TextSolutionBox.Text = existing.SolutionText;
            this.Title = "Редактировать задание";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TheTask.Title = TitleBox.Text.Trim();
            TheTask.SolutionImageUrl = ImageBox.Text.Trim();
            TheTask.SolutionText = TextSolutionBox.Text.Trim();
            System.Diagnostics.Debug.WriteLine($"=== СОХРАНЕНИЕ ЗАДАНИЯ ===");
            System.Diagnostics.Debug.WriteLine($"Заголовок: '{TheTask.Title}'");
            System.Diagnostics.Debug.WriteLine($"Изображение: '{TheTask.SolutionImageUrl}'");
            System.Diagnostics.Debug.WriteLine($"Текст решения: '{TheTask.SolutionText}'");
            System.Diagnostics.Debug.WriteLine($"Длина текста: {TheTask.SolutionText.Length}");
            System.Diagnostics.Debug.WriteLine($"=========================");

            if (string.IsNullOrEmpty(TheTask.Title))
            {
                MessageBox.Show("Укажите заголовок задания.");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}