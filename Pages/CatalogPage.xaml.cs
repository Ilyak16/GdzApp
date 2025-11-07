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
            int cls = 0;
            int.TryParse(ClassFilterBox.Text.Trim(), out cls);

            var filtered = _allBooks.Where(b =>
                (string.IsNullOrEmpty(q) ||
                 b.Subject.ToLower().Contains(q) ||
                 b.Authors.ToLower().Contains(q))
                && (cls == 0 || b.Class == cls)
            ).ToList();

            BooksPanel.ItemsSource = filtered;
        }

        private void Reset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SearchBox.Text = "";
            ClassFilterBox.Text = "";
            BooksPanel.ItemsSource = _allBooks;
        }
    }
}