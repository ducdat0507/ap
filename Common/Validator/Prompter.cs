using System.Runtime.CompilerServices;
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

        public static T PromptSelection<T>(string label, SortedDictionary<T, string> options, bool canThrow = false, bool canCancel = false) 
            where T : notnull
        {
            int index = 0;

            void Finish()
            {
                Console.CursorVisible = true;
                Console.Write("\n\n");
                Console.CursorTop += options.Count - index + 1;
            }

            while (true)
            {
                Console.Write(label);
                foreach (var item in options)
                {
                    Console.WriteLine();
                    Console.Write("   " + item.Value);
                }
                Console.CursorTop -= options.Count - 1;
                Console.CursorLeft = 0;
                Console.CursorVisible = false;
                index = 0;
                Console.Write("-> " + options.ElementAt(0).Value + " <-");
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Modifiers == 0 && key.Key == ConsoleKey.Escape)
                    {
                        if (canCancel)
                        {
                            Finish();
                            throw new PrompterCancelledException();
                        }
                    }
                    else if (key.Modifiers == 0 && key.Key == ConsoleKey.Enter)
                    {
                        Finish();
                        return options.ElementAt(index).Key;
                    }
                    else if (key.Modifiers == 0 && key.Key == ConsoleKey.UpArrow)
                    {
                        if (index > 0)
                        {
                            Console.CursorLeft = 0;
                            Console.Write("   " + options.ElementAt(index).Value + "   ");
                            Console.CursorLeft = 0;
                            Console.CursorTop --;
                            index--;
                            Console.Write("-> " + options.ElementAt(index).Value + " <-");
                        }
                    }
                    else if (key.Modifiers == 0 && key.Key == ConsoleKey.DownArrow)
                    {
                        if (index < options.Count - 1)
                        {
                            Console.CursorLeft = 0;
                            Console.Write("   " + options.ElementAt(index).Value + "   ");
                            Console.CursorLeft = 0;
                            Console.CursorTop ++;
                            index++;
                            Console.Write("-> " + options.ElementAt(index).Value + " <-");
                        }
                    }
                }
            }
        }
    }
}