using System;
using Npgsql;

namespace Task
{
    class Program
    {

        static void Main(string[] args)
        {
            AddTask();
            ReadTasks();
        }
        static void ReadTasks()//System.Threading.Tasks.Task ReadTasks()
        {
            var connString = "Host=127.0.0.1;Username=todolist;Password='qwerty';Database=todo_list";

            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT title, id, description, done, due_date  FROM todo_list", conn))//, id, description, done, due_date
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task();
                        task.Title = reader.GetString(0);
                        task.Id = reader.GetInt32(1);
                        task.Description = reader.GetString(2);
                        task.Done = reader.GetBoolean(3);
                        task.DueDate = reader.GetDateTime(4);

                        Console.WriteLine(Print(task));
                    }
                }
            }
        }
        static void AddTask()
        {
            var connString = "Host=127.0.0.1;Username=todolist;Password='qwerty';Database=todo_list";

            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            int lastId = 0;
            using (var cmd = new NpgsqlCommand("SELECT id FROM todo_list", conn))//, id, description, done, due_date
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lastId = 0;
                        lastId += reader.GetInt32(0);
                    }
                }
                using (var insert = new NpgsqlCommand("INSERT INTO todo_list (title, id, description, due_date, done) VALUES (@title, @id, @description, @due_date, @done) ", conn))
                {
                    insert.Parameters.AddWithValue("title", "New Task");
                    insert.Parameters.AddWithValue("id", lastId++);
                    insert.Parameters.AddWithValue("description", "New Description");
                    insert.Parameters.AddWithValue("due_date", new DateTime(2021,05,13));
                    insert.Parameters.AddWithValue("done", false);


                    insert.ExecuteNonQuery();
                }
            }
        }
        private static string Print(Task task)
        {
            char doneFlag = task.Done ? 'x' : ' ';
            return $"{task.Id}.\t[{doneFlag}] {task.Title} ({task.DueDate.ToString("MMMM dd")})\n\tUsed: {task.Description}";
        }
    }
}
