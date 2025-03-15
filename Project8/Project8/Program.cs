using Validator;

namespace Project8
{
    partial class Program
    {
        static void Main(string[] args)
        {
            CarManager carManager = new();
            while (true) 
            {
                switch 
                (
                    Prompter.PromptSelection("Select an option: ", 
                        new SortedDictionary<int, string>{
                            [1] = "Add cars",
                            [2] = "View all cars",
                            [3] = "Get car by ID",
                            [4] = "Search car by name",
                            [100] = "Exit program",
                        }
                    )
                ) 
                {
                    case 1: 
                        carManager.PromptAddCar();
                        break;
                    case 2: 
                        carManager.ListAllCars();
                        break;
                    case 3: 
                        carManager.PromptGetCarByID();
                        break;
                    case 4: 
                        carManager.PromptSearchCarByName();
                        break;

                    case 100: 
                        Console.WriteLine("Goodbye");
                        return;
                }
            }
        }
    }
}