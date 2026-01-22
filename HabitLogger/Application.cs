using System.Data;

namespace HabitLogger
{
    public static class Application
    {
        public static void Start()
        {
            Console.WriteLine("Hello! This is your habit tracker. I'll track all your progress. Press any key to start.");
            Console.ReadLine();
            ShowMenu();

            while (true)
            {
                var comand = Console.ReadLine().ToLower();
                if (comand == "#menu")
                {
                    ShowMenu();
                    continue;
                }
                switch (comand)
                {
                    case "#add":
                        AddLog();
                        break;
                    case "#edit":
                        UpdateLog();
                        break;
                    case "#delete":
                        DeleteLog();
                        break;
                    case "#new":
                        AddHabit();
                        break;
                    case "#view":
                        ReadLog();
                        break;
                    case "#exit":
                        Environment.Exit(0); ;
                        break; 
                    default:
                        Console.Write("Unknown command.");
                        break;
                }
                Console.WriteLine("Press any key to return menu.");
                Console.ReadLine();
                ShowMenu();
            }
        }
        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("Options: #add, #edit, #delete, #new, #view. To return to menu at any time use #menu. To close app use #exit");
        }
        private static void AddLog()
        {
            var selectHabit = InputHabit();
            if (selectHabit == null) return;

            var occurrenceDate = InputDate();
            if (occurrenceDate == DateTime.MinValue) return;

            if (DB.AddLog(selectHabit, occurrenceDate))
                Console.WriteLine("Successful addition.");
            else
                Console.WriteLine("Failed to add record!");
        }
        private static void UpdateLog()
        {
            Console.WriteLine($"Select record to update");
            ReadLog();

            var id = InputId();
            if (id == 0) return;

            var selectHabit = InputHabit();
            if (selectHabit == null) return;

            var occurrenceDate = InputDate();
            if (occurrenceDate == DateTime.MinValue) return;

            if (DB.UpdateLog(id, selectHabit, occurrenceDate))
                Console.WriteLine("Successful update");
            else
                Console.WriteLine("Failed to update record!");
        }
        private static void DeleteLog()
        {
            Console.WriteLine($"Select record to delete");
            ReadLog();

            var id = InputId();
            if (id == 0) return;

            if (DB.DeleteLog(id))
                Console.WriteLine("Successful delete");
            else
                Console.WriteLine("Failed to delete record!");
        }
        private static void ReadLog()
        {
            var logs = DB.GetLogs();
            foreach (var log in logs)
            {
                Console.WriteLine($"{log.HabitsName} ({log.MeasuresName}) - {log.OccurrenceDate} - {log.ID}");
            }
        }
        private static void AddHabit()
        {
            var habit = InputNewHabbit();
            if (habit == null) return;

            if (DB.AddHabit(habit))
                Console.WriteLine("Successful addition.");
            else
                Console.WriteLine("Failed to add record!");
        }
       
        private static Habit? InputHabit()
        {
            var habits = DB.GetHabits();

            var habitsText = string.Join(", ", habits.Select(h => $"{h.Name} ({h.Measured})").ToArray());
            Console.WriteLine($"Habbits: {habitsText}");

            Habit selectHabit = null;
            do
            {
                Console.Write($"Select a habit: ");
                var inputUser = Console.ReadLine();

                if (inputUser == "#menu")
                {
                    break;
                }
                else
                {
                    selectHabit = habits.Where(h => h.Name == inputUser).FirstOrDefault();
                }
            }
            while (selectHabit == null);

            return selectHabit;
        }
        private static DateTime InputDate()
        {
            DateTime occurrenceDate = DateTime.MinValue;

            Console.WriteLine($"Date in format 'dd/mm/yyyy hh:mm:ss'. To select the current date, enter #cd");
            do
            {
                Console.Write($"Enter date: ");
                var inputUser = Console.ReadLine();
                if (inputUser == "#menu")
                {
                    break;
                }
                else if (inputUser == "#cd")
                {
                    occurrenceDate = DateTime.Now;
                }
                else
                {
                    DateTime.TryParse(inputUser, out occurrenceDate);
                }
            }
            while (occurrenceDate == DateTime.MinValue);

            return occurrenceDate;
        }
        private static int InputId()
        {
            int id = 0;
            do
            {
                Console.Write($"Enter number: ");
                var inputUser = Console.ReadLine();
                if (inputUser == "#menu")
                {
                    ShowMenu();
                    break;
                }
                else
                {
                    Int32.TryParse(inputUser, out id);
                }
            }
            while (id == 0);

            return id;
        }
        private static Habit? InputNewHabbit()
        {
            Habit? selectHabit;
            string inputHabit;

            var habits = DB.GetHabits();
            var habitsText = string.Join(", ", habits.Select(h => $"{h.Name} ({h.Measured})").ToArray());
            Console.WriteLine($"Habbits: {habitsText}");

            Console.Write($"enter the name of the habit: ");
            
            inputHabit = Console.ReadLine();
            if (inputHabit == "#menu")
            {
                return null;
            }
            else
            {
                selectHabit = habits.Where(h => h.Name == inputHabit).FirstOrDefault();
                if (selectHabit != null)
                {
                    Console.Write("This habit has already been added");
                    return null;
                }
            }

            Measure? selectMeasure;
            string inputMeasure;

            var measures = DB.GetMeasures();
            var measureText = string.Join(", ", measures.Select(m => m.Name).ToArray());
            Console.WriteLine($"Measures: {measureText}");

            do
            {
                Console.Write($"select the name of the measure: ");
                inputMeasure = Console.ReadLine();

                if (inputMeasure == "#menu")
                {
                    return null;
                }
                else
                {
                    selectMeasure = measures.Where(m => m.Name == inputMeasure).FirstOrDefault();
                }
            }
            while (selectMeasure == null); 

            return new Habit(0, inputHabit, selectMeasure);
        }
    }
}