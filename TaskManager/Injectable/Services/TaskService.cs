using System.Collections.Generic;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace TaskManager.Services
{
    /// <summary>
    /// This class contains a SQL implementation of
    /// the ITaskService interface
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Inserts a given task into the Tasks table
        /// </summary>
        /// <param name="task"></param>
        public void CreateTask(Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }
        
        /// <summary>
        /// Removes the given task from the Tasks table
        /// </summary>
        /// <param name="task"></param>
        public void DeleteTask(Task task)
        {
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }
        
        /// <summary>
        /// Uses Dynamic LINQ to build a query from the given
        /// optional parameters to find a list of tasks
        /// </summary>
        /// <param name="taskStatus"></param>
        /// <param name="orderByDate"></param>
        /// <returns></returns>
        public List<Task> GetAllTasks(string taskStatus = null, string orderByDate = null)
        {
            string orderByQuery = orderByDate.Equals("Desc") ? "DueDate DESC" : "DueDate ASC";
            string taskStatusQuery = GenerateTaskStatusQuery(taskStatus);

            List<Task> tasks = _context.Tasks.AsQueryable()
                .OrderBy(orderByQuery)
                .Where(taskStatusQuery)
                .ToList();

            return tasks;
        }

        /// <summary>
        /// Uses the given id to query for a row in the Tasks table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task GetTaskById(int id)
        {
            Task task = (from t in _context.Tasks where t.Id == id select t).SingleOrDefault();
            return task;
        }

        /// <summary>
        /// Uses the given name to query for a row in the Tasks table
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public Task GetTaskByName(string taskName)
        {
            Task task = (from t in _context.Tasks where t.Name.Equals(taskName) select t).SingleOrDefault();
            return task;
        }

        /// <summary>
        /// Updates the given task with the given updates in the Tasks table
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="dueDate"></param>
        /// <param name="isCompleted"></param>
        /// <param name="task"></param>
        public void UpdateTask(string taskName, string dueDate, bool isCompleted, Task task)
        {
            task.Name = taskName;
            task.DueDate = Convert.ToDateTime(dueDate);
            task.IsCompleted = isCompleted;

            _context.SaveChanges();
        }

        /// <summary>
        /// Returns the total number of rows in the Tasks table
        /// </summary>
        /// <returns></returns>
        public int GetTotalTaskCount()
        {
            int taskCount = (from t in _context.Tasks select t).Count();
            return taskCount;
        }
        
        /// <summary>
        /// Used by GetAllTasks to build the part of the query
        /// that filters by task completion 
        /// </summary>
        /// <param name="taskStatus"></param>
        /// <returns></returns>
        private string GenerateTaskStatusQuery(string taskStatus)
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
