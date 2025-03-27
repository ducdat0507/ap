using Validator;

namespace Project9
{
    partial class Program
    {
        static void Main(string[] args)
        {
            StudentManager studentManager = new();
            while (true) 
            {
                switch 
                (
                    Prompter.PromptSelection("Select an option: ", 
                        new SortedDictionary<int, string>{
                            [1] = "Add students",
                            [2] = "View all students",
                            [3] = "Get student by ID",
                            [4] = "Search student by name",
                            [100] = "Exit program",
                        }
                    )
                ) 
                {
                    case 1: 
                        studentManager.PromptAddStudent();
                        break;
                    case 2: 
                        studentManager.ListAllStudents();
                        break;
                    case 3: 
                        studentManager.PromptGetStudentByID();
                        break;
                    case 4: 
                        studentManager.PromptSearchStudentByName();
                        break;

                    case 100: 
                        Console.WriteLine("Goodbye");
                        return;
                }
            }
        }
    }
}