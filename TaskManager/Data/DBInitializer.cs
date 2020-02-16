using System.Linq;
using TaskManager.Models;

namespace TaskManager.Data
{
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
                new Task() { Id = 1, Name = "Buy Groceries", IsCompleted = false, DueDate = "2020-02-03" },
                new Task() { Id = 2, Name = "Workout", IsCompleted = true, DueDate = "2020-01-01" },
                new Task() { Id = 3, Name = "Paint fence", IsCompleted = false, DueDate = "2020-03-15" },
                new Task() { Id = 4, Name = "Mow Lawn", IsCompleted = false, DueDate = "2020-06-11" }
            };

            foreach (Task task in tasks)
            {
                appDbContext.Tasks.Add(task);
            }

            appDbContext.SaveChanges();
        }
    }
}

