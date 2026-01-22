namespace HabitLogger
{
    public class Habit
    {
        public int Id { get; }
        public string Name { get;}
        public Measure Measured { get;}
        public Habit(int id, string name, Measure measured)
        {
            Id = id;
            Name = name;
            Measured = measured; 
        }
    }
}
