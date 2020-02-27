
namespace TaskManager.DataTransferObjects
{
    public class TaskResponse
    {
        /// <summary>
        /// id of the task
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// name of the task
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// true if completed, false if not completed
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// The date the task is due to be completed
        /// 
        /// Use standard ISO 8601 format: "2012-04-23"
        /// </summary>
        public string DueDate { get; set; }
    }
}
