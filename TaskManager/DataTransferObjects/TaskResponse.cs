using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.DataTransferObjects
{
    public class TaskResponse
    {
        public int id { get; set; }

        public string taskName { get; set; }

        public bool isCompleted { get; set; }

        public string dueDate { get; set; }
    }
}
