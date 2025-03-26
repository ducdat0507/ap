using System.Data.Common;
using System.Threading.Tasks;
using Validator;

namespace Project9
{
    class StudentManager
    {
        private readonly List<Student> Students = [];
        private ulong IDCounter = 1;

        public void PromptAddStudent() 
        {
            try 
            {
                while (true)
                {
                    Console.WriteLine("Enter student details (press Esc to cancel)");
                    Student student = new();
                    PromptStudent(student);
                    student.ID = IDCounter++;
                    Students.Add(student);
                    Console.WriteLine("Added student to list with ID " + student.ID);
                }
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }

        private static void PromptStudent(Student student)
        {
            student.Name = Prompter.Prompt(
                "   Name: ", 
                new Validator.Validator(),
                canCancel: true
            );
        }

        public void PromptStudentListOptions() 
        {
            try 
            {
                while (true) 
                {
                    switch 
                    (
                        Prompter.PromptSelection("Select an option: ", 
                            new SortedDictionary<int, string>{
                                [1] = "Sort students by name (ascending)",
                                [2] = "Sort students by name (descending)",
                                [100] = "Cancel",
                            },
                            canCancel: true
                        )
                    ) 
                    {
                        case 1: 
                            Students.Sort((x, y) => x.Name!.CompareTo(y.Name));
                            return;

                        case 2: 
                            Students.Sort((x, y) => y.Name!.CompareTo(x.Name));
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

        public void PromptStudentOptions(Student item) 
        {
            try 
            {
                while (true) 
                {
                    switch 
                    (
                        Prompter.PromptSelection(
                            "Student: "
                            + $"{item.ID,8} | {item.Name} "
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
                            PromptStudent(item);
                            return;

                        case 100: 
                            Students.Remove(item);
                            Console.WriteLine("Deleted student");
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

        public void PromptGetStudentByID() 
        {
            try 
            {
                ulong id = Prompter.Prompt(
                    "Enter ID: ",
                    new Validator.Validator().WholeNumber<ulong>(),
                    canCancel: true
                );
                Student? student = Students.Find(x => x.ID == id);
                if (student != null) PromptStudentOptions(student);
                else Console.WriteLine("Student does not exist");
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }

        public void PromptSearchStudentByName() 
        {
            try 
            {
                string query = Prompter.Prompt(
                    "Enter name: ",
                    new Validator.Validator(),
                    canCancel: true
                );
                List<Student> students = Students.Where(x => x.Name!.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (students.Count > 0) ListAllStudents(students);
                else Console.WriteLine("Student does not exist");
            }
            catch (PrompterCancelledException)
            {
                Console.WriteLine("Cancelled");
            }
        }


        public void ListAllStudents(List<Student>? list = null) 
        {
            list ??= Students;
            int cursorIndex = 0;
            int pageOffset = 0;
            int pageSize = Console.WindowHeight - 2;

            void Init() 
            {
                Console.CursorVisible = false;
                Console.WriteLine("Students");
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
                            $"{item.ID,8} | {item.Name} "
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
                            PromptStudentOptions(list[cursorIndex]);
                            Init();
                            break;
                        }
                        if (key.Key == ConsoleKey.O)
                        {
                            PromptStudentListOptions();
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
                            if (cursorIndex < Students.Count - 1) 
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