using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Task
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public string DueDate { get; set; }

        public bool IsCompleted { get; set; }



    }
}
