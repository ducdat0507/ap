
using System.Security.Cryptography.X509Certificates;
using Project4.Characters;
using Project4.Factories;

namespace Project4
{
    partial class Program 
    {
        static double currentTime;
        static List<ICharacter> characters = [
            CharacterFactory.Make(CharacterClass.Warrior, "Warrior"),
            CharacterFactory.Make(CharacterClass.Archer, "Archer"),
            CharacterFactory.Make(CharacterClass.Mage, "Mage"),
        ];

        static void Main (string[] args)
        {

            while (characters.Count > 1) 
            {
                foreach (var chr in characters) chr.DisplayStatus();
                SimulateBattle(characters);
                Console.WriteLine();
            }

            Console.WriteLine($"{characters[0].Name} is the last on standing!");
        }

        static void SimulateBattle(List<ICharacter> characters) 
        {
            ICharacter from = characters.Aggregate((x, y) => 
                (1 - x.Stamina) / x.Stats.Speed < (1 - y.Stamina) / y.Stats.Speed ? x : y
            );
            int fromIndex = characters.IndexOf(from);
            double timeAdd = (1 - from.Stamina) / from.Stats.Speed;
            currentTime += timeAdd;
            Console.Write($"[{currentTime * 10000,8:0}] ");

            int toIndex = new Random().Next(characters.Count - 1);
            if (toIndex >= fromIndex) toIndex++;
            ICharacter to = characters[toIndex];

            from.Attack(to);
            if (to.Health <= 0) 
            {
                Console.WriteLine($"{to.Name} falls!");
                characters.Remove(to);
            }

            foreach (var chr in characters) 
            {
                chr.Stamina += timeAdd * chr.Stats.Speed;
            }
            from.Stamina -= 1;
        }
    }
}