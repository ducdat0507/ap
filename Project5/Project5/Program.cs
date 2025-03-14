using System.Threading.Tasks;

namespace Project5
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            Thread thread1 = new Thread(() => DoTask(1, 2000));
            Thread thread2 = new Thread(() => DoTask(2, 3000));
            Thread thread3 = new Thread(() => DoTask(3, 1500));

            thread1.Start();
            thread2.Start();
            thread3.Start();
            
            thread1.Join();
            thread2.Join();
            thread3.Join();

            Console.WriteLine("All threads are completed");
        }

        static void DoTask(int number, int waitTime) 
        {
            Console.WriteLine($"Starting thread {number}");
            Thread.Sleep(waitTime);
            Console.WriteLine($"Thread {number} is completed");
        }
    }
}