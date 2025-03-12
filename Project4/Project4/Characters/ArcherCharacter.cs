namespace Project4.Characters
{
    public class ArcherCharacter : Character
    {

        public ArcherCharacter(string name) : base(name) 
        {
            Stats.MaxHealth = 50;
            Stats.Attack = 40;
            Stats.Defense = 5;
            Stats.Speed = 150;
        }

        public override void Attack (ICharacter opponent)
        {
            Console.WriteLine($"{Name} shots {opponent.Name}!");
            base.Attack(opponent);
        }
    }
}