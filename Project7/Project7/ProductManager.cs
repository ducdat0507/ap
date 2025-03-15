using System.Data.Common;
using System.Threading.Tasks;
using Validator;

namespace Project7
{
    class ProductManager
    {
        private readonly List<Product> Products = [];
        private ulong IDCounter = 1;

        public void PromptAddProduct() 
        {
            try 
            {
                while (true)
                {
                    Console.WriteLine("Enter product details (press Esc to cancel)");
                    Product product = new();
                    PromptProduct(product);
                    product.ID = IDCounter++;
                    Products.Add(product);
                    Console.WriteLine("Added product to list with ID " + product.ID);
                }
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }

        private static void PromptProduct(Product product)
        {
            product.Name = Prompter.Prompt(
                "   Name: ", 
                new Validator.Validator(),
                canCancel: true
            );
            product.Price = Prompter.Prompt(
                "   Price: ", 
                new Validator.Validator().Number<decimal>().GreaterThan(0),
                canCancel: true
            );
            product.Quantity = Prompter.Prompt(
                "   Quantity: ", 
                new Validator.Validator().WholeNumber<long>().GreaterThanOrEqual(0),
                canCancel: true
            );
        }

        public void PromptProductListOptions() 
        {
            try 
            {
                while (true) 
                {
                    switch 
                    (
                        Prompter.PromptSelection("Select an option: ", 
                            new SortedDictionary<int, string>{
                                [1] = "Sort products by price (ascending)",
                                [2] = "Sort products by price (descending)",
                                [100] = "Cancel",
                            },
                            canCancel: true
                        )
                    ) 
                    {
                        case 1: 
                            Products.Sort((x, y) => x.Price.GetValueOrDefault().CompareTo(y.Price));
                            return;

                        case 2: 
                            Products.Sort((x, y) => y.Price.GetValueOrDefault().CompareTo(x.Price));
                            return;

                        case 100: 
                            return;
                    }
                }
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }

        public void PromptProductOptions(Product item) 
        {
            try 
            {
                while (true) 
                {
                    switch 
                    (
                        Prompter.PromptSelection(
                            "Product: "
                            + $"{item.ID,8} | ${item.Price,10} | x{item.Quantity,10} | {item.Name} "
                            + "\nSelect an option: ", 
                            new SortedDictionary<int, string>{
                                [1] = "Edit",
                                [100] = "Delete",
                                [200] = "Cancel",
                            },
                            canCancel: true
                        )
                    ) 
                    {
                        case 1: 
                            PromptProduct(item);
                            return;

                        case 100: 
                            Products.Remove(item);
                            Console.WriteLine("Deleted product");
                            return;

                        case 200: 
                            return;
                    }
                }
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }

        public void PromptGetProductByID() 
        {
            try 
            {
                ulong id = Prompter.Prompt(
                    "Enter ID: ",
                    new Validator.Validator().WholeNumber<ulong>(),
                    canCancel: true
                );
                Product? product = Products.Find(x => x.ID == id);
                if (product != null) PromptProductOptions(product);
                else Console.WriteLine("Product does not exist");
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }

        public void PromptSearchProductByName() 
        {
            try 
            {
                string query = Prompter.Prompt(
                    "Enter name: ",
                    new Validator.Validator(),
                    canCancel: true
                );
                List<Product> products = Products.Where(x => x.Name!.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (products.Count > 0) ListAllProducts(products);
                else Console.WriteLine("Product does not exist");
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }


        public void ListAllProducts(List<Product>? list = null) 
        {
            list ??= Products;
            int cursorIndex = 0;
            int pageOffset = 0;
            int pageSize = Console.WindowHeight - 2;

            void Init() 
            {
                Console.CursorVisible = false;
                Console.WriteLine("Products");
                for (int a = 0; a < pageSize; a++) Console.WriteLine();
                Console.Write("[Esc] Return   [Enter] Select   [O] Options");
            }
            Init();

            void Finish()
            {
                Console.CursorVisible = true;
                Console.Clear();
            }
            while (true)
            {
                Console.CursorTop -= pageSize + 1;
                Console.CursorLeft = 20;
                Console.WriteLine($"{cursorIndex + 1,8} / {list.Count,8}");
                int top = Console.CursorTop;
                for (int a = 0; a < pageSize; a++) 
                {
                    Console.CursorLeft = 0;
                    int itemIndex = pageOffset + a;
                    if (itemIndex == cursorIndex) Console.Write("-> ");
                    else Console.Write("   ");
                    if (itemIndex < list.Count) 
                    {
                        var item = list[itemIndex];
                        Console.Write(
                            $"{item.ID,8} | ${item.Price,10} | x{item.Quantity,10} | {item.Name} "
                            .PadRight(Console.WindowWidth - 3, ' ')
                        );
                    }
                    Console.CursorTop = ++top;
                }

                while (true) 
                {
                    var key = Console.ReadKey(true);
                    if (key.Modifiers == 0)
                    {
                        if (key.Key == ConsoleKey.Escape)
                        {
                            Finish();
                            return;
                        }
                        if (key.Key == ConsoleKey.Enter)
                        {
                            PromptProductOptions(list[cursorIndex]);
                            Init();
                            break;
                        }
                        if (key.Key == ConsoleKey.O)
                        {
                            PromptProductListOptions();
                            Init();
                            break;
                        }
                        if (key.Key == ConsoleKey.UpArrow)
                        {
                            if (cursorIndex > 0) 
                            {
                                cursorIndex--;
                                pageOffset = Math.Min(pageOffset, cursorIndex);
                            }
                            break;
                        }
                        if (key.Key == ConsoleKey.DownArrow)
                        {
                            if (cursorIndex < Products.Count - 1) 
                            {
                                cursorIndex++;
                                pageOffset = Math.Max(pageOffset, cursorIndex - pageSize + 1);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}