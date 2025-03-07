using System.Text.RegularExpressions;
using Validator;

namespace Project2 
{
    partial class Program 
    {
        static void Main (string[] args)
        {
            List<Product> products = new();

            while (true) 
            {
                try 
                {
                    Console.WriteLine("Select an option: ");
                    Console.WriteLine("1. Add a product");
                    Console.WriteLine("2. List all products");
                    Console.WriteLine("3. Exit");
                    switch 
                    (
                        Prompter.Prompt("Your choice: ", 
                            new Validator.Validator().WholeNumber<int>().OneOf(1, 2, 3), 
                            canThrow: true
                        )
                    ) 
                    {
                        case 1: 
                            try 
                            {
                                while (true)
                                {
                                    Console.WriteLine("Enter product details (press Esc to cancel)");
                                    products.Add(new Product(
                                        Prompter.Prompt("ID: ", new Validator.Validator().Regex(IdRegex()), canCancel: true),
                                        Prompter.Prompt("Name: ", new Validator.Validator().MinLength(5), canCancel: true),
                                        Prompter.Prompt("Price: ", new Validator.Validator().Number<decimal>().GreaterThan(0), canCancel: true)
                                    ));
                                    Console.WriteLine("Added product to list");
                                }
                            }
                            catch (PrompterCancelledException)
                            {

                            }
                            Console.Write("\n");
                            break;

                        case 2:
                            Console.WriteLine("Product list: ");
                            foreach (Product p in products)
                                Console.WriteLine($"{p.ID} : {p.Name} : {p.Price}");
                            Console.Write("\n");
                            break;

                        case 3: 
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