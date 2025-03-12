using Project4.Characters;

namespace Project4.Factories
{
    public class CharacterFactory
    {
        public static ICharacter Make(CharacterClass faction, string name)
        {
            return faction switch 
            {
                CharacterClass.Warrior => new WarriorCharacter(name),
                CharacterClass.Archer => new ArcherCharacter(name),
                CharacterClass.Mage => new MageCharacter(name),
                _ => throw new ArgumentException("Invalid faction"),
            };
        }
    }
}