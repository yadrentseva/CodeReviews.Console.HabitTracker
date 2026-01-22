namespace HabitLogger
{
    public class Measure
    {
        public int Id { get; }
        public string Name { get; }
        public Measure(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
