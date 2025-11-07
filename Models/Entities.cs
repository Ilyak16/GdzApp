namespace GdzApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Email { get; set; } = "";
        public bool EmailNotifications { get; set; }
        public bool IsAdmin { get; set; }
        public int Class { get; set; } // optional
    }

    public class Textbook
    {
        public int Id { get; set; }
        public string Subject { get; set; } = "";
        public string Description { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Country { get; set; } = "";
        public string Authors { get; set; } = "";
        public int Year { get; set; }
        public int Class { get; set; }
        public string ImageUrl { get; set; } = "";
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public int TextbookId { get; set; }
        public string Title { get; set; } = "";
        public string SolutionImageUrl { get; set; } = "";
    }
}