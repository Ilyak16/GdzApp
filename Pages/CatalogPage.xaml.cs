using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GdzApp.Models;
using GdzApp.Data;

namespace GdzApp.Pages
{
    public partial class CatalogPage : Page
    {
        private MainWindow _main;
        private List<Textbook> _allBooks = new();

        public CatalogPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            LoadBooks();
        }

        private void LoadBooks()
        {
            _allBooks = Database.GetAllTextbooks();
            BooksPanel.ItemsSource = _allBooks;
        }

        private void Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var q = SearchBox.Text.Trim().ToLower();
            var classFilter = ClassFilterBox.Text.Trim();

            var filtered = _allBooks.Where(b =>
                (string.IsNullOrEmpty(q) ||
                 b.Subject.ToLower().Contains(q) ||
                 b.Authors.ToLower().Contains(q))
                && (string.IsNullOrEmpty(classFilter) ||
                    b.Class.Contains(classFilter))
            ).ToList();

            BooksPanel.ItemsSource = filtered;
        }

        private void Reset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SearchBox.Text = "";
            ClassFilterBox.Text = "";
            BooksPanel.ItemsSource = _allBooks;
        }
        private void ViewTasks_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Textbook textbook)
            {
                NavigationService?.Navigate(new TaskViewPage(_main, textbook));
            }
        }
    }
}