namespace Project4.Characters
{
    public class MageCharacter : Character
    {

        public MageCharacter(string name) : base(name) 
        {
            Stats.MaxHealth = 100;
            Stats.Attack = 250;
            Stats.Defense = 10;
            Stats.Speed = 50;
        }

        public override void Attack (ICharacter opponent)
        {
            Console.WriteLine($"{Name} casts magic at {opponent.Name}!");
            base.Attack(opponent);
        }
    }
}