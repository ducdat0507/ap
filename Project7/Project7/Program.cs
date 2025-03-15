using Validator;

namespace Project7
{
    partial class Program
    {
        static void Main(string[] args)
        {
            ProductManager productManager = new();
            while (true) 
            {
                switch 
                (
                    Prompter.PromptSelection("Select an option: ", 
                        new SortedDictionary<int, string>{
                            [1] = "Add products",
                            [2] = "View all products",
                            [3] = "Get product by ID",
                            [100] = "Exit program",
                        }
                    )
                ) 
                {
                    case 1: 
                        productManager.PromptAddProduct();
                        break;
                    case 2: 
                        productManager.ListAllProducts();
                        break;
                    case 3: 
                        productManager.PromptGetProductByID();
                        break;

                    case 100: 
                        Console.WriteLine("Goodbye");
                        return;
                }
            }
        }
    }
}