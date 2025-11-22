using System.Windows;
using System.Windows.Controls;
using GdzApp.Data;
using GdzApp.Models;
using GdzApp.Windows;

namespace GdzApp.Pages
{
    public partial class AdminPage : Page
    {
        private MainWindow _main;
        public AdminPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            LoadTextbooks();
            TextbooksGrid.SelectionChanged += TextbooksGrid_SelectionChanged;
        }

        private void LoadTextbooks()
        {
            TextbooksGrid.ItemsSource = Database.GetAllTextbooks();
        }

        private void TextbooksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TextbooksGrid.SelectedItem is Textbook tb)
            {
                TasksGrid.ItemsSource = Database.GetTasksByTextbook(tb.Id);
            }
            else
            {
                TasksGrid.ItemsSource = null;
            }
        }

        private void AddTextbook_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new AddEditTextbookWindow();
            if (wnd.ShowDialog() == true)
            {
                Database.InsertTextbook(wnd.TheTextbook);
                LoadTextbooks();
            }
        }

        private void EditTextbook_Click(object sender, RoutedEventArgs e)
        {
            if (TextbooksGrid.SelectedItem is Textbook tb)
            {
                var wnd = new AddEditTextbookWindow(tb);
                if (wnd.ShowDialog() == true)
                {
                    Database.UpdateTextbook(wnd.TheTextbook);
                    LoadTextbooks();
                }
            }
            else MessageBox.Show("Выберите учебник.");
        }

        private void DeleteTextbook_Click(object sender, RoutedEventArgs e)
        {
            if (TextbooksGrid.SelectedItem is Textbook tb)
            {
                if (MessageBox.Show("Удалить учебник?", "Подтвердите", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Database.DeleteTextbook(tb.Id);
                    LoadTextbooks();
                    TasksGrid.ItemsSource = null;
                }
            }
            else MessageBox.Show("Выберите учебник.");
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (TextbooksGrid.SelectedItem is Textbook tb)
            {
                var wnd = new AddEditTaskWindow(tb.Id);
                if (wnd.ShowDialog() == true)
                {
                    System.Diagnostics.Debug.WriteLine("=== ДОБАВЛЕНИЕ ЗАДАНИЯ ===");
                    var taskId = Database.InsertTask(wnd.TheTask);
                    TasksGrid.ItemsSource = Database.GetTasksByTextbook(tb.Id);

                    // Проверяем что сохранилось в базе
                    Database.CheckTaskData(taskId);
                }
            }
            else MessageBox.Show("Выберите учебник чтобы добавить задание.");
        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksGrid.SelectedItem is TaskItem t)
            {
                var wnd = new AddEditTaskWindow(t);
                if (wnd.ShowDialog() == true)
                {
                    System.Diagnostics.Debug.WriteLine("=== РЕДАКТИРОВАНИЕ ЗАДАНИЯ ===");
                    Database.UpdateTask(wnd.TheTask);
                    if (TextbooksGrid.SelectedItem is Textbook tb)
                        TasksGrid.ItemsSource = Database.GetTasksByTextbook(tb.Id);

                    // Проверяем что сохранилось в базе
                    Database.CheckTaskData(t.Id);
                }
            }
            else MessageBox.Show("Выберите задание.");
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksGrid.SelectedItem is TaskItem t)
            {
                if (MessageBox.Show("Удалить задание?", "Подтвердите", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Database.DeleteTask(t.Id);
                    if (TextbooksGrid.SelectedItem is Textbook tb)
                        TasksGrid.ItemsSource = Database.GetTasksByTextbook(tb.Id);
                }
            }
            else MessageBox.Show("Выберите задание.");
        }
    }
}