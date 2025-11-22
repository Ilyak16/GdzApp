using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using GdzApp.Models;

namespace GdzApp.Data
{
    public static class Database
    {
        private static string DbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gdz.db");
        private static string ConnString => $"Data Source={DbFile}";

        public static void Initialize()
        {
            bool create = !File.Exists(DbFile);
            using var conn = new SqliteConnection(ConnString);
            conn.Open();

            var cmd = conn.CreateCommand();

            cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT UNIQUE,
            Password TEXT,
            Email TEXT,
            EmailNotifications INTEGER,
            IsAdmin INTEGER,
            Class INTEGER
        );
        CREATE TABLE IF NOT EXISTS Textbooks (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Subject TEXT,
            Description TEXT,
            Manufacturer TEXT,
            Country TEXT,
            Authors TEXT,
            Year INTEGER,
            Class TEXT,
            ImageUrl TEXT
        );
        CREATE TABLE IF NOT EXISTS Tasks (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            TextbookId INTEGER,
            Title TEXT,
            SolutionImageUrl TEXT,
            SolutionText TEXT,
            FOREIGN KEY(TextbookId) REFERENCES Textbooks(Id) ON DELETE CASCADE
        );
    ";
            cmd.ExecuteNonQuery();

            // Миграция для изменения типа поля Class с INTEGER на TEXT
            try
            {
                var checkCmd = conn.CreateCommand();
                checkCmd.CommandText = "PRAGMA table_info(Textbooks);";
                using var reader = checkCmd.ExecuteReader();
                bool needsAlter = false;

                while (reader.Read())
                {
                    if (reader.GetString(1) == "Class" && reader.GetString(2) == "INTEGER")
                    {
                        needsAlter = true;
                        break;
                    }
                }
                reader.Close();

                if (needsAlter)
                {
                    var alterCmd = conn.CreateCommand();
                    alterCmd.CommandText = @"
                CREATE TABLE Textbooks_temp (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Subject TEXT,
                    Description TEXT,
                    Manufacturer TEXT,
                    Country TEXT,
                    Authors TEXT,
                    Year INTEGER,
                    Class TEXT,
                    ImageUrl TEXT
                );
                
                INSERT INTO Textbooks_temp (Id, Subject, Description, Manufacturer, Country, Authors, Year, Class, ImageUrl)
                SELECT Id, Subject, Description, Manufacturer, Country, Authors, Year, CAST(Class AS TEXT), ImageUrl 
                FROM Textbooks;
                
                DROP TABLE Textbooks;
                
                ALTER TABLE Textbooks_temp RENAME TO Textbooks;
            ";
                    alterCmd.ExecuteNonQuery();
                    Console.WriteLine("Таблица Textbooks успешно обновлена (Class -> TEXT)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении таблицы Textbooks: {ex.Message}");
            }

            // Миграция для добавления поля SolutionText в таблицу Tasks
            try
            {
                var checkSolutionCmd = conn.CreateCommand();
                checkSolutionCmd.CommandText = "PRAGMA table_info(Tasks);";
                using var reader = checkSolutionCmd.ExecuteReader();
                bool hasSolutionText = false;

                while (reader.Read())
                {
                    if (reader.GetString(1) == "SolutionText")
                    {
                        hasSolutionText = true;
                        break;
                    }
                }
                reader.Close();

                if (!hasSolutionText)
                {
                    var alterCmd = conn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE Tasks ADD COLUMN SolutionText TEXT DEFAULT '';";
                    alterCmd.ExecuteNonQuery();
                    Console.WriteLine("Добавлено поле SolutionText в таблицу Tasks");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении поля SolutionText: {ex.Message}");
            }

            // если файл только что создан — добавим default admin
            if (create)
            {
                InsertAdminIfNotExists(conn);
            }
        }

        private static void InsertAdminIfNotExists(SqliteConnection conn)
        {
            var check = conn.CreateCommand();
            check.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = 'admin';";
            var cnt = Convert.ToInt32(check.ExecuteScalar());
            if (cnt == 0)
            {
                var insert = conn.CreateCommand();
                insert.CommandText = @"
                    INSERT INTO Users (Username, Password, Email, EmailNotifications, IsAdmin, Class)
                    VALUES ('admin','admin','admin@example.com', 1, 1, 0);
                ";
                insert.ExecuteNonQuery();
            }
        }

        // --- Users CRUD ---
        public static User? GetUserByUsername(string username)
        {
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, Password, Email, EmailNotifications, IsAdmin, Class FROM Users WHERE Username = $u;";
            cmd.Parameters.AddWithValue("$u", username);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new User
                {
                    Id = rdr.GetInt32(0),
                    Username = rdr.GetString(1),
                    Password = rdr.GetString(2),
                    Email = rdr.GetString(3),
                    EmailNotifications = rdr.GetInt32(4) == 1,
                    IsAdmin = rdr.GetInt32(5) == 1,
                    Class = rdr.GetInt32(6)
                };
            }
            return null;
        }

        public static void InsertUser(User u)
        {
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Username, Password, Email, EmailNotifications, IsAdmin, Class) VALUES ($user,$pass,$email,$notif,0,$class);";
            cmd.Parameters.AddWithValue("$user", u.Username);
            cmd.Parameters.AddWithValue("$pass", u.Password);
            cmd.Parameters.AddWithValue("$email", u.Email);
            cmd.Parameters.AddWithValue("$notif", u.EmailNotifications ? 1 : 0);
            cmd.Parameters.AddWithValue("$class", u.Class);
            cmd.ExecuteNonQuery();
        }

        // --- Textbooks CRUD ---
        public static List<Textbook> GetAllTextbooks()
        {
            var list = new List<Textbook>();
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Subject, Description, Manufacturer, Country, Authors, Year, Class, ImageUrl FROM Textbooks;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Textbook
                {
                    Id = rdr.GetInt32(0),
                    Subject = rdr.GetString(1),
                    Description = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                    Manufacturer = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                    Country = rdr.IsDBNull(4) ? "" : rdr.GetString(4),
                    Authors = rdr.IsDBNull(5) ? "" : rdr.GetString(5),
                    Year = rdr.IsDBNull(6) ? 0 : rdr.GetInt32(6),
                    Class = rdr.IsDBNull(7) ? "" : rdr.GetString(7),
                    ImageUrl = rdr.IsDBNull(8) ? "" : rdr.GetString(8)
                });
            }
            return list;
        }

        public static int InsertTextbook(Textbook t)
        {
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"INSERT INTO Textbooks (Subject, Description, Manufacturer, Country, Authors, Year, Class, ImageUrl)
                  VALUES ($sub,$desc,$man,$country,$authors,$year,$class,$img);
                  SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$sub", t.Subject);
            cmd.Parameters.AddWithValue("$desc", t.Description);
            cmd.Parameters.AddWithValue("$man", t.Manufacturer);
            cmd.Parameters.AddWithValue("$country", t.Country);
            cmd.Parameters.AddWithValue("$authors", t.Authors);
            cmd.Parameters.AddWithValue("$year", t.Year);
            cmd.Parameters.AddWithValue("$class", t.Class);
            cmd.Parameters.AddWithValue("$img", t.ImageUrl);
            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public static void UpdateTextbook(Textbook t)
        {
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"UPDATE Textbooks SET Subject=$sub, Description=$desc, Manufacturer=$man, Country=$country,
                  Authors=$authors, Year=$year, Class=$class, ImageUrl=$img WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$sub", t.Subject);
            cmd.Parameters.AddWithValue("$desc", t.Description);
            cmd.Parameters.AddWithValue("$man", t.Manufacturer);
            cmd.Parameters.AddWithValue("$country", t.Country);
            cmd.Parameters.AddWithValue("$authors", t.Authors);
            cmd.Parameters.AddWithValue("$year", t.Year);
            cmd.Parameters.AddWithValue("$class", t.Class);
            cmd.Parameters.AddWithValue("$img", t.ImageUrl);
            cmd.Parameters.AddWithValue("$id", t.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteTextbook(int id)
        {
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            // Удалится и связанная задача (ON DELETE CASCADE)
            cmd.CommandText = "DELETE FROM Textbooks WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        // --- Tasks CRUD ---
        public static List<TaskItem> GetTasksByTextbook(int textbookId)
        {
            var list = new List<TaskItem>();
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, TextbookId, Title, SolutionImageUrl FROM Tasks WHERE TextbookId = $tid;";
            cmd.Parameters.AddWithValue("$tid", textbookId);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new TaskItem
                {
                    Id = rdr.GetInt32(0),
                    TextbookId = rdr.GetInt32(1),
                    Title = rdr.GetString(2),
                    SolutionImageUrl = rdr.IsDBNull(3) ? "" : rdr.GetString(3)
                });
            }
            return list;
        }

        public static int InsertTask(TaskItem t)
        {
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"INSERT INTO Tasks (TextbookId, Title, SolutionImageUrl) VALUES ($tid,$title,$img);
                  SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$tid", t.TextbookId);
            cmd.Parameters.AddWithValue("$title", t.Title);
            cmd.Parameters.AddWithValue("$img", t.SolutionImageUrl);
            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public static void UpdateTask(TaskItem t)
        {
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Tasks SET Title=$title, SolutionImageUrl=$img WHERE Id=$id;";
            cmd.Parameters.AddWithValue("$title", t.Title);
            cmd.Parameters.AddWithValue("$img", t.SolutionImageUrl);
            cmd.Parameters.AddWithValue("$id", t.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteTask(int id)
        {
            using var conn = new SqliteConnection(ConnString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Tasks WHERE Id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }
    }
}