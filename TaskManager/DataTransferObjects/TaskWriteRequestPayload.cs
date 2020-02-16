namespace TaskManager.DataTransferObjects
{
    public class TaskWriteRequestPayload
    {
        /// <summary>
        /// name of the task
        /// </summary>
        public string taskName { get; set; }

        /// <summary>
        /// true if completed, false if not completed
        /// </summary>
        public bool isCompleted { get; set; }

        /// <summary>
        /// The date the task is due to be completed
        /// 
        /// Use standard ISO 8601 format: "2012-04-23"
        /// </summary>
        public string dueDate { get; set; }
    }
}
