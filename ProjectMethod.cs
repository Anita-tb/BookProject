using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic; 


namespace BookProject;

public class StartApp
{
    private LogInApp loginApp;
    private Project project  = new Project();
    public void Start()
    {
        project.Menu();
        int input = int.Parse(Console.ReadLine());

        bool b = true;
        do
        {
            switch (input)
            {
                case 1:
                    Console.Clear();
                    project.ListOfBooks();
                    project.BackToMenu();
                    input = int.Parse(Console.ReadLine());
                    break;
                case 2:
                    Console.Clear();
                    project.AddBook();
                    project.BackToMenu();
                    input = int.Parse(Console.ReadLine());
                    break;
                case 3:
                    Console.Clear();
                    project.DeleteBook();
                    project.BackToMenu();
                    input = int.Parse(Console.ReadLine());
                    break;
                case 4:
                    Console.Clear();
                    project.EditBook();
                    project.BackToMenu();
                    input = int.Parse(Console.ReadLine());
                    break;
                case 5:
                    Console.Clear();
                    project.SearchBook();
                    project.BackToMenu();
                    input = int.Parse(Console.ReadLine());
                    break;
                case 6:
                    Console.Clear();
                    loginApp = new LogInApp();
                    loginApp.LogIn();
                    break;
                case 7:
                    b = false;
                    Console.WriteLine("End");
                    break;
                
                default:
                    Console.WriteLine("Wrong input");
                    project.Menu();
                    input = int.Parse(Console.ReadLine());
                    break;
            }
    
        } while (b == true);
    }
}
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Page { get; set; }
    public string Author { get; set; }
    
    public int CurrentPage { get; set; }
    
    public int ReadPart{get;set;}
    
}
public class Project
{
    private SqlConnection connection;
    private List<Book> books;
    
    
    
    
    
    public Project()
    {
        SqlConnectionStringBuilder builder = new();
        builder.InitialCatalog = "BookDb";
        builder.DataSource = ".";
        builder.UserID = "sa";
        builder.Password = "Anita2004";
        builder.Encrypt = false;
        
        connection = new(builder.ConnectionString);
    }
    public void Menu()
    {
        Console.WriteLine("1 for List of Books");
        Console.WriteLine("2 for Appending Books");
        Console.WriteLine("3 for Deleting Books");
        Console.WriteLine("4 for Editing Books");
        Console.WriteLine("5 for Searching Books");
        Console.WriteLine("6 for Logging Out");
        Console.WriteLine("7 for Closing Application");
        Console.Write("enter: ");
        
        
    }
    
    public void ListOfBooks()
    {
        SqlCommand command = new SqlCommand
        { 
            Connection = connection,
            CommandType = CommandType.Text,
            CommandText = $"SELECT * FROM Books where UserId = {LogInApp.userId}",
        };
        
        connection.Open();
        var reader = command.ExecuteReader();
        
        
        books = new List<Book>();

        
        while (reader.Read())
        {
            var book = new Book
            {
                Id = (int)reader["Id"],
                Title = (string)reader["Title"],
                Page = (int)reader["Page"],
                Author = (string)reader["Author"],
                CurrentPage = (int)reader["CurrentPage"],
                ReadPart = (int)(Convert.ToDecimal(reader["CurrentPage"])/Convert.ToDecimal(reader["Page"])*100),
                
            };
            books.Add(book); 
        }
        
        if (reader.HasRows==false)
        {
            Console.WriteLine("There aren't any books");
        }

        connection.Close();
        ShowBooks();
        
    }

    public void ShowBooks()
    {
        foreach (var book in books)
        {
            
            Console.WriteLine($"Id: {book.Id}\t\t Title:{book.Title}\t\t Page:{book.Page}\t\t Author:{book.Author}\t\t CurrentPage:{book.CurrentPage}\t\t ReadPart:{book.ReadPart}%");
        }
    }
    
    
    public void AddBook()
    {
        Console.Write("Enter Book Title: ");
        string title = Console.ReadLine();
        Console.Write("Enter Book Page: ");
        int page = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter Book Author: ");
        string author = Console.ReadLine();
        Console.Write("Enter Book Current Page: ");
        int currentPage = Convert.ToInt32(Console.ReadLine());
        
        
        SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandType = CommandType.Text,
            CommandText =
                $"insert into Books(Title, Page, Author, CurrentPage, UserID) values ('{title}', {page}, '{author}', {currentPage}, {LogInApp.userId})"
        };
        connection.Open();
        int result = command.ExecuteNonQuery();
        Console.WriteLine($"Done");
        connection.Close();
        ListOfBooks();
    }

    public void EditBook()
    {
        ListOfBooks();
        Console.WriteLine("Enter Book Id: ");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Enter column you want to edit(title, page, author, currentpage): ");
        string input = Console.ReadLine().ToLower();

        string commandText = null;
        
        switch (input)
        {
            case "title":
                Console.Write("Enter Book NewTitle: ");
                string title = Console.ReadLine();
                commandText = $"UPDATE Books SET Title='{title}' where Id={id} AND UserID={LogInApp.userId}";
                break;
            case "page":
                Console.Write("Enter Book NewPage: ");
                int page = Convert.ToInt32(Console.ReadLine());
                commandText = $"UPDATE Books SET Page={page} where Id={id}";
                break;
            case "author":
                Console.Write("Enter Book NewAuthor: ");
                string author = Console.ReadLine();
                commandText = $"UPDATE Books SET Author='{author}' where Id={id}";
                break;
            case "currentpage":
                Console.Write("Enter Book NewCurrentPage: ");
                int currentpage = Convert.ToInt32(Console.ReadLine());
                commandText = $"UPDATE Books SET CurrentPage={currentpage} where Id={id}";
                break;
            default:
                Console.WriteLine("Invalid column name.");
                return;
        }
        
        SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandType = CommandType.Text,
            CommandText = commandText,
        };
        connection.Open();
        int result = command.ExecuteNonQuery();
        connection.Close();
        if (result > 0)
        {
            Console.WriteLine("Done");
        }
        else
        {
            Console.WriteLine("No book found with the specified ID.");
        }
        ListOfBooks();
    }

    public void DeleteBook()
    {
        ListOfBooks();
        Console.Write("Enter the ID of the book you want to delete: ");
        int id = Convert.ToInt32(Console.ReadLine());
        
        SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandType = CommandType.Text,
            CommandText = $"DELETE FROM Books WHERE Id = {id} AND UserId = {LogInApp.userId}",
        };
        connection.Open();
        int result = command.ExecuteNonQuery();
        connection.Close();
        if (result > 0)
        {
            Console.WriteLine("Done");
        }
        else
        {
            Console.WriteLine("No book found with the specified ID.");
        }
        ListOfBooks();
        
    }

    public void SearchBook()
    {
        Console.Write("Enter Book Title or a part of the book you want to search: ");
        string title = Console.ReadLine();
        SqlCommand command = new SqlCommand
        {
            Connection = connection,
            CommandType = CommandType.Text,
            CommandText = $"SELECT * FROM Books WHERE Title LIKE '%{title}%' AND UserID = {LogInApp.userId}",
            
        };
        connection.Open();
        var reader = command.ExecuteReader();
        

        bool a = true;
        while (reader.Read())
        {
            
            a = false;
            int readPart = (int)(Convert.ToDecimal(reader["CurrentPage"]) / Convert.ToDecimal(reader["Page"]) * 100);
            Console.WriteLine($"Id: {reader["Id"]}\t\t Title:{reader["Title"]}\t\t Page:{reader["Page"]}\t\t Author:{reader["Author"]}\t\t CurrentPage:{reader["CurrentPage"]}\t\t ReadPart:{readPart}%");
        }

        if(a==true)
        {
            Console.WriteLine("No book found");
        }
      
        connection.Close();
    }

    public void BackToMenu()
    {
        Console.WriteLine("enter b to see menu: ");
        string input = Console.ReadLine().ToLower();
        if (input == "b")
        {
            Console.Clear();
            Menu();
        }
        else
        {
            Console.WriteLine("invalid input");
            input = Console.ReadLine();
        }
    }

    
    
}

public class LogInApp
{
    private SqlConnection connection;
    private StartApp start = new StartApp();
    public static int userId;
    
    public LogInApp()
    {
        SqlConnectionStringBuilder builder = new();
        builder.InitialCatalog = "BookDb";
        builder.DataSource = ".";
        builder.UserID = "sa";
        builder.Password = "Anita2004";
        builder.Encrypt = false;
        
        connection = new(builder.ConnectionString);
    }

    public void LogIn()
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine();
        Console.Write("Enter your password: ");
        string password = Console.ReadLine();
        SqlCommand command = new SqlCommand
        { 
            Connection = connection,
            CommandType = CommandType.Text,
            CommandText = "SELECT * FROM People",
        };
        
        connection.Open();
        var reader = command.ExecuteReader();
        bool a = true;
        
        while (reader.Read())
        {
            
            if (reader["Username"].ToString() == username && reader["Password"].ToString() == password)
            {
                a = false;
                userId = Convert.ToInt32(reader["Id"]);
                Console.Clear();
                start.Start();
                break;
            }
            
        }
        connection.Close();

        if (a == true)
        {
            Console.WriteLine("Invalid username or password.");
            Console.WriteLine("Please try again.");
            Console.ReadLine();
            Console.Clear();
            LogIn();
        }
        
        
    }
}





