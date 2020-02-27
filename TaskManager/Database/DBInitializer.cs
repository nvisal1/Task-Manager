using System;
using System.Linq;
using TaskManager.Models;

namespace TaskManager.Data
{
    /// <summary>
    /// This class is referenced at API startup.
    /// 
    /// It checks to see if the Tasks table is empty.
    /// If it is empty, it seeds the table with 4
    /// tasks.
    /// </summary>
    public class DBInitializer
    {
        public static void Initialize(AppDbContext appDbContext)
        {
            if (appDbContext.Tasks.Any())
            {
                return;
            }

            Task[] tasks = new Task[]
            {
                new Task() { Name = "Buy Groceries", IsCompleted = false, DueDate = Convert.ToDateTime("2020-02-03") },
                new Task() { Name = "Workout", IsCompleted = true, DueDate = Convert.ToDateTime("2020-01-01") },
                new Task() { Name = "Paint fence", IsCompleted = false, DueDate = Convert.ToDateTime("2020-03-15") },
                new Task() { Name = "Mow Lawn", IsCompleted = false, DueDate = Convert.ToDateTime("2020-06-11") }
            };

            foreach (Task task in tasks)
            {
                appDbContext.Tasks.Add(task);
            }

            appDbContext.SaveChanges();
        }
    }
}

