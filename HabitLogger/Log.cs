namespace HabitLogger
{
    public class Log
    {
        public int ID { get; }
        public string HabitsName { get; }
        public string MeasuresName { get;}
        public DateTime OccurrenceDate { get;}
        public Log(int id, string habitsName, string measuresName, DateTime occurrenceDate)
        {
            ID = id;
            HabitsName = habitsName;
            MeasuresName = measuresName;
            OccurrenceDate = occurrenceDate;
        }
    }
}
