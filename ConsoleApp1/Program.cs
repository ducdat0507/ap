using System.Net.Mail;
using Validator;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            while (true)
            {
                try
                {
                    Console.Write("Enter email: ");
                    MailAddress mailAddress = new Validator.Validator().Email().Validate(Console.ReadLine());
                }
                catch (ValidatorFailedException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}