
namespace TaskManager.DataTransferObjects
{
    public class TaskResponse
    {
        public int Id { get; set; }

        public string TaskName { get; set; }

        public bool IsCompleted { get; set; }

        public string DueDate { get; set; }
    }
}
