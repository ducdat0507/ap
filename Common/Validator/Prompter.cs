using Validator.Generic;

namespace Validator
{
    public static class Prompter 
    {
        public static T Prompt<T>(string label, Validator<T> validator, bool canThrow = false, bool canCancel = false) 
        {
            string input = "";
            while (true)
            {
                Console.Write(label);
                input = "";
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Modifiers == 0 && key.Key == ConsoleKey.Escape)
                    {
                        if (canCancel)
                        {
                            Console.Write("\n");
                            throw new PrompterCancelledException();
                        }
                    }
                    else if (key.Modifiers == 0 && key.Key == ConsoleKey.Enter)
                    {
                        Console.Write("\n");
                        try 
                        {
                            return validator.Validate(input);
                        }
                        catch (ValidatorFailedException e)
                        {
                            ConsoleColor col = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(e.Message);
                            Console.ForegroundColor = col;
                            if (canThrow) throw;
                            else break;
                        }
                    }
                    else if (key.Modifiers == 0 && key.Key == ConsoleKey.Backspace)
                    {
                        if (input.Length > 0)
                        {
                            Console.Write("\b \b");
                            input = input[..^1];
                        }
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        Console.Write(key.KeyChar);
                        input += key.KeyChar;
                    }
                }
            }
        }
    }
}