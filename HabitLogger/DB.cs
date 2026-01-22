using Microsoft.Data.Sqlite;
using System.Data;

namespace HabitLogger
{
    public static class DB
    {
        public const string connectionString = "Data Source=habitsdata.db";
        private const string dbName = "habitsdata.db";

        public static void InitializeDatabase()
        {
            if (File.Exists(dbName))
            {
                return;
            }

            var commandsText = InitialCommandsText(); 
            SqliteCommand command;
           
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                foreach (var commandText in commandsText)
                {
                    command = new SqliteCommand(commandText, connection);
                    command.ExecuteNonQuery();
                }
            }
        }
        private static List<string> InitialCommandsText()
        {
            var commandsText = new List<string>();

            var sqlExpres_CreateTableMeasures = @"CREATE TABLE Measures 
                                    (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                                    Name TEXT NOT NULL UNIQUE)";
            commandsText.Add(sqlExpres_CreateTableMeasures);

            var sqlExpres_CreateTableHabits = @"CREATE TABLE Habits 
                                    (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                                    Name TEXT NOT NULL UNIQUE,
                                    MeasureId INT,
                                    FOREIGN KEY(MeasureId) REFERENCES Measures(Id))";
            commandsText.Add(sqlExpres_CreateTableHabits);

            var sqlExpres_CreateTableLogger = @"CREATE TABLE Logger 
                                    (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                                    HabitId INT,
                                    OccurrenceDate DATE,
                                    FOREIGN KEY(HabitId) REFERENCES Habits(Id) ON DELETE CASCADE ON UPDATE CASCADE)";
            commandsText.Add(sqlExpres_CreateTableLogger);

            var sqlExpres_FillTableMeasures = "INSERT INTO Measures (Name) VALUES ('day'), ('hour'), ('liter')";
            commandsText.Add(sqlExpres_FillTableMeasures);

            var sqlExpres_FillTableHabits = "INSERT INTO Habits (Name, MeasureId) VALUES ('meditation', 1), ('early rise', 1), ('physical activity', 2), ('drink water', 3)";
            commandsText.Add(sqlExpres_FillTableHabits);

            return commandsText;
        }

        public static bool AddLog(Habit habit, DateTime occurrenceDate)
        {
            string sqlExpression = "INSERT INTO Logger (HabitId, OccurrenceDate) VALUES (@HabitId, @OccurrenceDate)";
            using (var connection = new SqliteConnection(DB.connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("HabitId", habit.Id);
                command.Parameters.AddWithValue("OccurrenceDate", occurrenceDate);
                try
                {
                    var countAdd = command.ExecuteNonQuery();
                    if (countAdd > 0) return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return false;
        }
        public static bool UpdateLog(int logId, Habit habit, DateTime occurrenceDate)
        {
            string sqlExpression = "UPDATE Logger SET HabitId = @HabitId, OccurrenceDate = @OccurrenceDate WHERE Id = @Id";
            using (var connection = new SqliteConnection(DB.connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("Id", logId);
                command.Parameters.AddWithValue("HabitId", habit.Id);
                command.Parameters.AddWithValue("OccurrenceDate", occurrenceDate);
                try
                {
                    int countUpd = command.ExecuteNonQuery();
                    if (countUpd > 0) return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return false;
        }
        public static bool DeleteLog(int logId)
        {
            string sqlExpression = "DELETE FROM Logger WHERE Id = @Id";
            using (var connection = new SqliteConnection(DB.connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("Id", logId);
                try
                {
                    int countDel = command.ExecuteNonQuery();
                    if (countDel > 0) return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return false;
        }
        public static List<Log> GetLogs()
        {
            var logs = new List<Log>(); 
            var sqlExpression = "Select Logger.Id AS ID, Logger.OccurrenceDate AS OccurrenceDate, Habits.Name AS HabitsName, Measures.Name AS MeasuresName" +
                                    " FROM Logger AS Logger Left Join Habits as Habits ON Logger.HabitId = Habits.ID " +
                                    " LEFT JOIN Measures AS Measures ON Habits.MeasureId = Measures.Id" +
                                    " ORDER BY HabitsName, OccurrenceDate";

            using (var connection = new SqliteConnection(DB.connectionString))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(sqlExpression, connection);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            logs.Add(
                                new Log(
                                    reader.GetInt32("Id"), 
                                    reader.GetString("HabitsName"), 
                                    reader.GetString("MeasuresName"), 
                                    reader.GetDateTime("OccurrenceDate"))
                            );
                        }
                    }
                }
            }
            return logs;
        }
        public static bool AddHabit(Habit habit)
        {
            string sqlExpression = "INSERT INTO Habits (Name, MeasureId) VALUES (@Name, @MeasureId)";
            using (var connection = new SqliteConnection(DB.connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("Name", habit.Name);
                command.Parameters.AddWithValue("MeasureId", habit.Measured.Id);
                try
                {
                    var countAdd = command.ExecuteNonQuery();
                    if (countAdd > 0) return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return false;
        }
        public static List<Habit> GetHabits()
        {
            var habits = new List<Habit>();

            string sqlExpression = "SELECT Habits.Id AS Id, " +
                                    "Habits.Name AS Name, " +
                                    "Habits.MeasureId AS MeasureId, " +
                                    "Measures.Name AS MeasureName " +
                                    "FROM Habits Left Join Measures ON Habits.MeasureId = Measures.Id " +
                                    "Order BY Name";
            using (var connection = new SqliteConnection(DB.connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            habits.Add(
                                new Habit(
                                    reader.GetInt32("Id"),
                                    reader.GetString("Name"),
                                    new Measure(reader.GetInt32("MeasureId"), reader.GetString("MeasureName"))
                                )
                            );
                        }
                    }
                }
            }
            return habits;
        }
        public static List<Measure> GetMeasures()
        {
            var measures = new List<Measure>();

            string sqlExpression = "SELECT * FROM Measures";
            using (var connection = new SqliteConnection(DB.connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            measures.Add(
                                new Measure(
                                    reader.GetInt32("Id"),
                                    reader.GetString("Name")
                                )
                            );
                        }
                    }
                }
            }
            return measures;
        }    
    }
}
