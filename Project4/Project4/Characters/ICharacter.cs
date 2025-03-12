using Project4.Statistics;

namespace Project4.Characters
{
    public interface ICharacter
    {
        public string Name { get; set; }

        public double Health { get; set; }
        public double Stamina { get; set; }
        public CharacterStat Stats { get; }

        public abstract void DisplayStatus ();
        public abstract void Damage (double damage);
        public abstract void Attack (ICharacter character);
    }
}