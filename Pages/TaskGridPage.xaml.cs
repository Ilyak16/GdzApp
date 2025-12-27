using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using GdzApp.Data;
using GdzApp.Models;

namespace GdzApp.Pages
{
    public partial class TasksGridPage : Page
    {
        private MainWindow _main;
        private Textbook _textbook;
        private ObservableCollection<TaskViewModel> _tasks;

        public TasksGridPage(MainWindow main, Textbook textbook)
        {
            InitializeComponent();
            _main = main;
            _textbook = textbook;

            TitleText.Text = $"Задания: {textbook.Subject}";
            LoadTasks();
        }

        private void LoadTasks()
        {
            var tasks = Database.GetTasksByTextbook(_textbook.Id);
            _tasks = new ObservableCollection<TaskViewModel>();

            foreach (var task in tasks)
            {
                _tasks.Add(new TaskViewModel(task, this));
            }

            TasksGrid.ItemsSource = _tasks;
        }

        public void OpenTaskSolution(TaskItem task)
        {
            NavigationService?.Navigate(new TaskSolutionPage(_main, task));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
    public class TaskViewModel
    {
        private TaskItem _task;
        private TasksGridPage _page;

        public string Title => _task.Title;
        public ICommand OpenTaskCommand { get; }

        public TaskViewModel(TaskItem task, TasksGridPage page)
        {
            _task = task;
            _page = page;
            OpenTaskCommand = new RelayCommand(OpenTask);
        }

        private void OpenTask()
        {
            _page.OpenTaskSolution(_task);
        }
    }
    public class RelayCommand : ICommand
    {
        private Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute?.Invoke();
        }
    }
}