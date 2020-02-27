using System.Collections.Generic;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
    /// <summary>
    /// Iterface that is used for DI in the API Controllers
    /// </summary>
    public interface ITaskService
    {
        public void CreateTask(Task task);
        public Task GetTaskByName(string taskName);
        public Task GetTaskById(int id);
        public void UpdateTask(string taskName, string dueDate, bool isCompleted, Task task);
        public void DeleteTask(Task task);
        public List<Task> GetAllTasks(string taskStatus = null, string orderByDate = null);
        public int GetTotalTaskCount();
    }
}
