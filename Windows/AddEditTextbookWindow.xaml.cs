using System;
using System.Windows;
using GdzApp.Models;

namespace GdzApp.Windows
{
    public partial class AddEditTextbookWindow : Window
    {
        public Textbook TheTextbook { get; private set; }

        public AddEditTextbookWindow()
        {
            InitializeComponent();
            TheTextbook = new Textbook();
            this.Title = "Добавить учебник";
        }

        public AddEditTextbookWindow(Textbook existing) : this()
        {
            TheTextbook = existing;
            this.Title = "Редактировать учебник";
            SubjectBox.Text = existing.Subject;
            AuthorsBox.Text = existing.Authors;
            DescriptionBox.Text = existing.Description;
            ManufacturerBox.Text = existing.Manufacturer;
            CountryBox.Text = existing.Country;
            YearBox.Text = existing.Year.ToString();
            ClassBox.Text = existing.Class.ToString();
            ImageUrlBox.Text = existing.ImageUrl;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TheTextbook.Subject = SubjectBox.Text.Trim();
            TheTextbook.Authors = AuthorsBox.Text.Trim();
            TheTextbook.Description = DescriptionBox.Text.Trim();
            TheTextbook.Manufacturer = ManufacturerBox.Text.Trim();
            TheTextbook.Country = CountryBox.Text.Trim();
            TheTextbook.Class = ClassBox.Text.Trim();
            if (int.TryParse(YearBox.Text.Trim(), out int y)) TheTextbook.Year = y;
            TheTextbook.ImageUrl = ImageUrlBox.Text.Trim();

            if (string.IsNullOrEmpty(TheTextbook.Subject))
            {
                MessageBox.Show("Укажите предмет.");
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
