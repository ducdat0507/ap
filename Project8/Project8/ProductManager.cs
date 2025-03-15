using System.Data.Common;
using System.Threading.Tasks;
using Validator;

namespace Project8
{
    class CarManager
    {
        private readonly List<Car> Cars = [];
        private ulong IDCounter = 1;

        public void PromptAddCar() 
        {
            try 
            {
                while (true)
                {
                    Console.WriteLine("Enter car details (press Esc to cancel)");
                    Car car = new();
                    PromptCar(car);
                    car.ID = IDCounter++;
                    Cars.Add(car);
                    Console.WriteLine("Added car to list with ID " + car.ID);
                }
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }

        private static void PromptCar(Car car)
        {
            car.Name = Prompter.Prompt(
                "   Name: ", 
                new Validator.Validator(),
                canCancel: true
            );
            car.Model = Prompter.Prompt(
                "   Model: ", 
                new Validator.Validator(),
                canCancel: true
            );
            car.Price = Prompter.Prompt(
                "   Price: ", 
                new Validator.Validator().Number<decimal>().GreaterThan(0),
                canCancel: true
            );
            car.Quantity = Prompter.Prompt(
                "   Quantity: ", 
                new Validator.Validator().WholeNumber<long>().GreaterThanOrEqual(0),
                canCancel: true
            );
        }

        public void PromptCarListOptions() 
        {
            try 
            {
                while (true) 
                {
                    switch 
                    (
                        Prompter.PromptSelection("Select an option: ", 
                            new SortedDictionary<int, string>{
                                [1] = "Sort cars by price (ascending)",
                                [2] = "Sort cars by price (descending)",
                                [100] = "Cancel",
                            },
                            canCancel: true
                        )
                    ) 
                    {
                        case 1: 
                            Cars.Sort((x, y) => x.Price.GetValueOrDefault().CompareTo(y.Price));
                            return;

                        case 2: 
                            Cars.Sort((x, y) => y.Price.GetValueOrDefault().CompareTo(x.Price));
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

        public void PromptCarOptions(Car item) 
        {
            try 
            {
                while (true) 
                {
                    switch 
                    (
                        Prompter.PromptSelection(
                            "Car: "
                            + $"{item.ID,8} | ${item.Price,10} | x{item.Quantity,10} | {item.Name} | {item.Model} "
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
                            PromptCar(item);
                            return;

                        case 100: 
                            Cars.Remove(item);
                            Console.WriteLine("Deleted car");
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

        public void PromptGetCarByID() 
        {
            try 
            {
                ulong id = Prompter.Prompt(
                    "Enter ID: ",
                    new Validator.Validator().WholeNumber<ulong>(),
                    canCancel: true
                );
                Car? car = Cars.Find(x => x.ID == id);
                if (car != null) PromptCarOptions(car);
                else Console.WriteLine("Car does not exist");
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }

        public void PromptSearchCarByName() 
        {
            try 
            {
                string query = Prompter.Prompt(
                    "Enter name: ",
                    new Validator.Validator(),
                    canCancel: true
                );
                List<Car> cars = Cars.Where(x => x.Name!.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (cars.Count > 0) ListAllCars(cars);
                else Console.WriteLine("Car does not exist");
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }


        public void ListAllCars(List<Car>? list = null) 
        {
            list ??= Cars;
            int cursorIndex = 0;
            int pageOffset = 0;
            int pageSize = Console.WindowHeight - 2;

            void Init() 
            {
                Console.CursorVisible = false;
                Console.WriteLine("Cars");
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
                            $"{item.ID,8} | ${item.Price,10} | x{item.Quantity,10} | {item.Name} | {item.Model} "
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
                            PromptCarOptions(list[cursorIndex]);
                            Init();
                            break;
                        }
                        if (key.Key == ConsoleKey.O)
                        {
                            PromptCarListOptions();
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
                            if (cursorIndex < Cars.Count - 1) 
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