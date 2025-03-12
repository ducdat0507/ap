namespace Project4.Characters
{
    public class WarriorCharacter : Character
    {

        public WarriorCharacter(string name) : base(name) 
        {
            Stats.MaxHealth = 150;
            Stats.Attack = 50;
            Stats.Defense = 20;
            Stats.Speed = 100;
        }

        public override void Attack (ICharacter opponent)
        {
            Console.WriteLine($"{Name} hits {opponent.Name}!");
            base.Attack(opponent);
        }
    }
}