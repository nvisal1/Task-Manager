using System.Collections.Generic;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;
using System.Linq;
using System.Linq.Dynamic;
using System;

namespace TaskManager.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public void CreateTask(Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public void DeleteTask(Task task)
        {
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }

        public List<Task> GetAllTasks(string taskStatus = null, string orderByDate = null)
        {
            string orderByQuery = orderByDate.Equals("Desc") ? "DueDate DESC" : "DueDate ASC";
            string taskStatusQuery = generateTaskStatusQuery(taskStatus);

            List<Task> tasks = _context.Tasks.AsQueryable()
                .OrderBy(orderByQuery)
                .Where(taskStatusQuery)
                .ToList();

            return tasks;
        }

        public Task GetTaskById(int id)
        {
            Task task = (from t in _context.Tasks where t.Id == id select t).SingleOrDefault();
            return task;
        }

        public Task GetTaskByName(string taskName)
        {
            Task task = (from t in _context.Tasks where t.Name.Equals(taskName) select t).SingleOrDefault();
            return task;
        }

        public void UpdateTask(string taskName, string dueDate, bool isCompleted, Task task)
        {
            task.Name = taskName;
            task.DueDate = Convert.ToDateTime(dueDate);
            task.IsCompleted = isCompleted;

            _context.SaveChanges();
        }

        public int GetTotalTaskCount()
        {
            int taskCount = (from t in _context.Tasks select t).Count();
            return taskCount;
        }

        private string generateTaskStatusQuery(string taskStatus)
        {
            string taskStatusQuery;
            if (taskStatus.Equals("Completed"))
            {
                taskStatusQuery = "IsCompleted=true";
            }
            else if (taskStatus.Equals("NotCompleted"))
            {
                taskStatusQuery = "IsCompleted=false";
            }
            else
            {
                taskStatusQuery = "IsCompleted=true OR IsCompleted=false";
            }

            return taskStatusQuery;
        }
    }
}
