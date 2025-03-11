using System.Text.RegularExpressions;
using Validator;

namespace Project3 
{
    partial class Program 
    {
        static void Main (string[] args)
        {
            Library library = new();

            while (true) 
            {
                try 
                {
                    Console.WriteLine("Select an option: ");
                    Console.WriteLine("1. Add books");
                    Console.WriteLine("2. List all books");
                    Console.WriteLine("0. Exit");
                    switch 
                    (
                        Prompter.Prompt("Your choice: ", 
                            new Validator.Validator().WholeNumber<int>().OneOf(1, 2, 0), 
                            canThrow: true
                        )
                    ) 
                    {
                        case 1: 
                            library.PromptAddBook();
                            return;

                        case 0: 
                            Console.WriteLine("Goodbye");
                            return;
                    }
                }
                catch (ValidatorFailedException)
                {

                }
            }
        }

        [GeneratedRegex(@"^[0-9]{5}$")]
        private static partial Regex IdRegex();
    }
}