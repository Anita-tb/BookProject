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
                    Environment.Exit(0);
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
    public string Authors { get; set; }
    public int CurrentPage { get; set; }
    public int ReadPart{get;set;}
    public string Category { get; set; }
    
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
        Console.WriteLine("2 for Adding Books");
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
            CommandText = $"SELECT b.Id, b.Title, b.Page, b.CurrentPage, b.Category, STRING_AGG(a.AuthorName, ', ') AS AuthorNames FROM Books b LEFT JOIN AuthorBook ab ON b.Id = ab.BookRef LEFT JOIN Authors a ON ab.AuthorRef = a.Id WHERE b.UserId = {LogInApp.userId} GROUP BY b.Id, b.Title, b.Page, b.CurrentPage, b.Category",
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
                Authors = (string)reader["AuthorNames"],
                CurrentPage = (int)reader["CurrentPage"],
                ReadPart = (int)(Convert.ToDecimal(reader["CurrentPage"])/Convert.ToDecimal(reader["Page"])*100),
                Category = (string)reader["Category"],
                
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
            
            Console.WriteLine($"Id: {book.Id}\t\t Title:{book.Title}\t\t Page:{book.Page}\t\t Authors: {book.Authors}\t\t CurrentPage:{book.CurrentPage}\t\t ReadPart:{book.ReadPart}%\t\t Category:{book.Category}");
        }
    }
    
    
    public void AddBook()
    {
        List<int> authorsref = new List<int>();
        int bookref = 0;
        string author = "";
        string commandtext = "";
        Console.Write("Enter Book Title: ");
        string title = Console.ReadLine();
        Console.Write("Enter Book Page: ");
        int page = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter Book Current Page: ");
        int currentPage = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter Book Category: ");
        string category = Console.ReadLine();
        Console.Write("Enter Book Author: ");
        author = Console.ReadLine();
        
        do
        {
            try
            {
                SqlCommand commandauthor = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = $"insert into Authors(AuthorName) values ('{author}'); select Id from Authors where AuthorName = '{author}'"

                };
                connection.Open();


                var reader = commandauthor.ExecuteReader();
                while (reader.Read())
                {
                    authorsref.Add(Convert.ToInt32(reader["Id"]));
                }
                connection.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                SqlCommand commandauthorn = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = $"select Id from Authors where AuthorName = '{author}'"

                };
                connection.Open();
                var readern = commandauthorn.ExecuteReader();
                while (readern.Read())
                {
                    authorsref.Add(Convert.ToInt32(readern["Id"]));
                }

                connection.Close();
            }

            
            
            Console.Write("Enter another book author (press Enter and leave it blank if there isn't another author): ");
            author= Console.ReadLine();
            
        } while (!string.IsNullOrEmpty(author));
        
        
        SqlCommand commandbook = new SqlCommand
        {
            Connection = connection,
            CommandType = CommandType.Text,
            CommandText =
                $"insert into Books(Title, Page, CurrentPage, UserId, Category) values ('{title}', {page}, {currentPage}, {LogInApp.userId}, '{category}'); select Id from Books where Title = '{title}'"
        };
        connection.Open();
        var reader1 = commandbook.ExecuteReader();
        while (reader1.Read())
        {
            bookref = Convert.ToInt32(reader1["Id"]);
        }
        
        connection.Close();

        foreach (int authorref in authorsref)
        {
            SqlCommand commandAuthorBook = new SqlCommand
            {
                Connection = connection,
                CommandType = CommandType.Text,
                CommandText = $"insert into AuthorBook(BookRef, AuthorRef) values ({bookref}, {authorref})"
            };
            connection.Open();
            commandAuthorBook.ExecuteNonQuery();
            connection.Close();
            
        }
        authorsref.Clear();
        
        
        ListOfBooks();
    }

    public void EditBook()
    {
        List<int> authorsref = new List<int>();
        ListOfBooks();
        Console.WriteLine("Enter Book Id: ");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Enter column you want to edit(title, page, authors, currentpage, category): ");
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
                commandText = $"UPDATE Books SET Page={page} where Id={id} AND UserID={LogInApp.userId}";
                break;
            case "category":
                Console.Write("Enter Book NewCategory: ");
                string category = Console.ReadLine();
                commandText = $"UPDATE Books SET Category={category} where Id={id} AND UserID={LogInApp.userId}";
                break;
            case "authors":
                Console.Write("Enter Book NewAuthors (Comma Seperated): ");
                string[] authors = Console.ReadLine().Split(',');
                foreach (string author in authors)
                {
                    try
                    {
                        SqlCommand commandauthorupdate = new SqlCommand
                        {
                            Connection = connection,
                            CommandType = CommandType.Text,
                            CommandText = $"insert into Authors(AuthorName) values ('{author}'); select Id from Authors where AuthorName = '{author}'"

                        };
                        connection.Open();


                        var reader = commandauthorupdate.ExecuteReader();
                        while (reader.Read())
                        {
                            authorsref.Add(Convert.ToInt32(reader["Id"]));
                        }
                        connection.Close();
                    }
                    catch (Exception e)
                    {
                        connection.Close();
                        SqlCommand commandauthorn = new SqlCommand
                        {
                            Connection = connection,
                            CommandType = CommandType.Text,
                            CommandText = $"select Id from Authors where AuthorName = '{author}'"

                        };
                        connection.Open();
                        var readern = commandauthorn.ExecuteReader();
                        while (readern.Read())
                        {
                            authorsref.Add(Convert.ToInt32(readern["Id"]));
                        }

                        connection.Close();
                    }
                }

                SqlCommand commanddeleteauthorbook = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = $"DELETE from AuthorBook where BookRef={id}"
                };
                connection.Open();
                commanddeleteauthorbook.ExecuteNonQuery();
                connection.Close();
                foreach (int authorref in authorsref)
                {
                    SqlCommand commandAuthorBook = new SqlCommand
                    {
                        Connection = connection,
                        CommandType = CommandType.Text,
                        CommandText = $"INSERT INTO AuthorBook(BookRef, AuthorRef) values ({id}, {authorref})"
                    };
                    connection.Open();
                    int resultauthor =commandAuthorBook.ExecuteNonQuery();
                    if (resultauthor > 0)
                    {
                        Console.WriteLine("Author Updated");
                    }
                    
                    connection.Close();
            
                }
                authorsref.Clear();
                break;
            case "currentpage":
                Console.Write("Enter Book NewCurrentPage: ");
                int currentpage = Convert.ToInt32(Console.ReadLine());
                commandText = $"UPDATE Books SET CurrentPage={currentpage} where Id={id} And UserID={LogInApp.userId}";
                break;
            default:
                Console.WriteLine("Invalid column name.");
                return;
        }

        if (commandText != null)
        {
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
        }
        
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
        List<int> bookids = new List<int>();
        Console.Write("Enter the field you want to search by (title or author or category): ");
        string search = Console.ReadLine();
        string commandText = null;
        
        switch (search)
        {
            case "title":
                Console.Write("Enter Book Title or a part of it: ");
                string title = Console.ReadLine();
                commandText =
                    $"SELECT b.Id, b.Title, b.Page, b.CurrentPage, b.Category, STRING_AGG(a.AuthorName, ', ') AS AuthorNames FROM Books b LEFT JOIN AuthorBook ab ON b.Id = ab.BookRef LEFT JOIN Authors a ON ab.AuthorRef = a.Id WHERE b.UserId = {LogInApp.userId} AND b.Title LIKE '%{title}%' GROUP BY b.Id, b.Title, b.Page, b.CurrentPage, b.Category";
                break;
            case "author":
                Console.Write("Enter Book author or a part of it: ");
                string author = Console.ReadLine();
                SqlCommand commandsearchauther = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText =
                        $"SELECT b.Id, STRING_AGG(a.AuthorName, ', ') AS AuthorNames FROM Books b LEFT JOIN AuthorBook ab ON b.Id = ab.BookRef LEFT JOIN Authors a ON ab.AuthorRef = a.Id WHERE b.UserId = {LogInApp.userId} AND a.AuthorName LIKE '%{author}%' GROUP BY b.Id"
                };
                connection.Open();
                var readerbookid = commandsearchauther.ExecuteReader();
                while (readerbookid.Read())
                {
                    bookids.Add(Convert.ToInt32(readerbookid["Id"]));
                }
                connection.Close();
                foreach (int bookid in bookids)
                {
                    SqlCommand commandbookid = new SqlCommand
                    {
                        Connection = connection,
                        CommandType = CommandType.Text,
                        CommandText =
                            $"SELECT b.Id, b.Title, b.Page, b.CurrentPage, b.Category, STRING_AGG(a.AuthorName, ', ') AS AuthorNames FROM Books b LEFT JOIN AuthorBook ab ON b.Id = ab.BookRef LEFT JOIN Authors a ON ab.AuthorRef = a.Id WHERE b.UserId = {LogInApp.userId} AND b.Id= {bookid} GROUP BY b.Id, b.Title, b.Page, b.CurrentPage, b.Category",
                    };
                    connection.Open();
                    var readerbookid2 = commandbookid.ExecuteReader();
                    bool c = true;
                    while (readerbookid2.Read())
                    {
            
                        c = false;
                        int readPart = (int)(Convert.ToDecimal(readerbookid2["CurrentPage"]) / Convert.ToDecimal(readerbookid2["Page"]) * 100);
                        Console.WriteLine($"Id: {readerbookid2["Id"]}\t\t Title:{readerbookid2["Title"]}\t\t Page:{readerbookid2["Page"]}\t\t Author:{readerbookid2["AuthorNames"]}\t\t CurrentPage:{readerbookid2["CurrentPage"]}\t\t ReadPart:{readPart}%\t\t Category:{readerbookid2["Category"]}");
                    }

                    if(c==true)
                    {
                        Console.WriteLine("No book found");
                    }
      
                    connection.Close();
                    
                    
                }
                break;
            case "category":
                Console.Write("Enter Book Category or a part of it: ");
                string category = Console.ReadLine();
                commandText = $"SELECT b.Id, b.Title, b.Page, b.CurrentPage, b.Category, STRING_AGG(a.AuthorName, ', ') AS AuthorNames FROM Books b LEFT JOIN AuthorBook ab ON b.Id = ab.BookRef LEFT JOIN Authors a ON ab.AuthorRef = a.Id WHERE b.UserId = {LogInApp.userId} AND b.Category LIKE '%{category}%' GROUP BY b.Id, b.Title, b.Page, b.CurrentPage, b.Category";
                break;
        }

        if (commandText != null)
        {
            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = CommandType.Text,
                CommandText = commandText
            };
            connection.Open();
            var reader = command.ExecuteReader();
        

            bool a = true;
            while (reader.Read())
            {
            
                a = false;
                int readPart = (int)(Convert.ToDecimal(reader["CurrentPage"]) / Convert.ToDecimal(reader["Page"]) * 100);
                Console.WriteLine($"Id: {reader["Id"]}\t\t Title:{reader["Title"]}\t\t Page:{reader["Page"]}\t\t Author:{reader["AuthorNames"]}\t\t CurrentPage:{reader["CurrentPage"]}\t\t ReadPart:{readPart}%\t\t Category:{reader["Category"]}");
            }

            if(a==true)
            {
                Console.WriteLine("No book found");
            }
      
            connection.Close();
        }
        
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
            CommandText = $"SELECT * FROM People where Username = '{username}' and Password = {password}",
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





