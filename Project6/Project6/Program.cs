using System.Threading.Tasks;

namespace Project6
{
    partial class Program
    {
        static async Task Main(string[] args)
        {
            Task task1 = DoArithmeticTask(1, (a, b) => a + b, 10, 5, 2000);
            Task task2 = DoArithmeticTask(2, (a, b) => a - b, 10, 5, 3000);
            Task task3 = DoArithmeticTask(3, (a, b) => a * b, 10, 5, 1500);
            Task task4 = DoArithmeticTask(4, (a, b) => a / b, 10, 5, 2500);

            await Task.WhenAny(task1, task2, task3, task4);
            Console.WriteLine("One task is completed");

            await Task.WhenAll(task1, task2, task3, task4);
            Console.WriteLine("All tasks are completed");
        }

        static async Task DoArithmeticTask(int number, Func<double, double, double> func, double a, double b, int waitTime) 
        {
            Console.WriteLine($"Doing task {number} with {a} and {b}");
            double result = func(a, b);
            await Task.Delay(waitTime);
            Console.WriteLine($"Task {number} returned {result}");
        }
    }
}