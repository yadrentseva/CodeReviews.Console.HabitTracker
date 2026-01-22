Habit Logger - is an app where users can track their habits.
All information is stored in a database (SQLite) that is created and populated when the application is first launched.
Users interact with the database using commands add/new, view, edit, delete (ADO.NET, CRUD)

problems: 
I couldn't avoid duplicating code in the DB class, because each SQL command requires its own parameters. 