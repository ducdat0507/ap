using System.Text.RegularExpressions;
using Validator;

namespace Project3
{
    public partial class Library 
    {
        readonly List<Book> Books = [];

        public void PromptAddBook()
        {
            try 
            {
                while (true)
                {
                    Console.WriteLine("Enter book details (press Esc to cancel)");
                    Books.Add(new Book {
                        ISBN = Prompter.Prompt(
                            "ISBN: ", 
                            new Validator.Validator().Regex(isbnRegex(), "Field must contain a valid ISBN code").Transform(
                                n => new string([.. n.Where(char.IsDigit)])
                            ).Callback(
                                n => !Books.Any(b => b.ISBN == n), "A book with this ISBN code already exists"
                            ).Optional(),
                            canCancel: true
                        ),
                        Name = Prompter.Prompt(
                            "Name: ", 
                            new Validator.Validator(),
                            canCancel: true
                        ),
                        Author = Prompter.Prompt(
                            "Author: ", 
                            new Validator.Validator(),
                            canCancel: true
                        ),
                        Price = Prompter.Prompt(
                            "Price: ", 
                            new Validator.Validator().Number<decimal>().GreaterThan(0),
                            canCancel: true
                        ),
                    });
                    Console.WriteLine("Added book to list");
                }
            }
            catch (PrompterCancelledException)
            {

            }
        }

        public void ListAllBooks()
        {
            Console.WriteLine("Listing all books");
            ListBooks(Books);
        }

        public void SearchByName()
        {
            try 
            {
                string query = Prompter.Prompt("Enter name: ",
                    new Validator.Validator(),
                    canCancel: true
                ).ToLower();
                ListBooks(Books.Where(b => b.Name?.Contains(query, StringComparison.CurrentCultureIgnoreCase) ?? false));
            }
            catch (PrompterCancelledException)
            {

            }
        }

        public void SearchByAuthor()
        {
            try 
            {
                string query = Prompter.Prompt("Enter author: ",
                    new Validator.Validator(),
                    canCancel: true
                ).ToLower();
                ListBooks(Books.Where(b => b.Author?.Contains(query, StringComparison.CurrentCultureIgnoreCase) ?? false));
            }
            catch (PrompterCancelledException)
            {

            }
        }

        static void ListBooks(IEnumerable<Book> books)
        {
            int count = 0;
            foreach (Book book in books) 
            {
                count++;
                Console.WriteLine(
                    $"{count,5}. | {book.ISBN} | {book.Author} - {book.Name} | {book.Price}"
                );
            }
            if (count == 0)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There are no books");
                Console.ForegroundColor = color;
            }
        }

        [GeneratedRegex(@"((978[\--– ])?[0-9][0-9\--– ]{10}[\--– ][0-9xX])|((978)?[0-9]{9}[0-9Xx])")]
        private static partial Regex isbnRegex();
    }
}