using Project4.Statistics;

namespace Project4.Characters
{
    public abstract class Character : ICharacter
    {
        public string Name { get; set; }

        public double HealthPercent { get; set; } = 1;
        public double Health 
        {
            get { return HealthPercent * Stats.MaxHealth; }
            set { HealthPercent = value / Stats.MaxHealth; }
        }

        public double Stamina { get; set; }

        public CharacterStat Stats { get; } = new();

        public Character(string name) 
        {
            Name = name;
        }

        public void DisplayStatus() 
        {
            Console.WriteLine($"{Name} : {Health:0}/{Stats.MaxHealth:0} HP : {Stamina * 100:0} STM");
        }

        public virtual void Damage (double damage) 
        {
            damage = Math.Floor(damage * damage / (damage + Stats.Defense));
            Health -= damage;
            Console.WriteLine($"{Name} took {damage} damage!");
        }

        public virtual void Attack (ICharacter opponent) 
        {
            opponent.Damage(Stats.Attack);
        }
    }

    public enum CharacterClass 
    {
        Warrior,
        Archer,
        Mage,
    }
}