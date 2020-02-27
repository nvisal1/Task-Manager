using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.DataTransferObjects
{
    public class TaskWriteRequestPayload
    {
        /// <summary>
        /// name of the task
        /// </summary>
        [StringLength(100)]
        [Required]
        public string TaskName { get; set; }

        /// <summary>
        /// true if completed, false if not completed
        /// </summary>
        [Required]
        public bool? IsCompleted { get; set; }

        /// <summary>
        /// The date the task is due to be completed
        /// 
        /// Use standard ISO 8601 format: "2012-04-23"
        /// </summary>
        [Required]
        [MinLength(10)]
        public string DueDate { get; set; }
    }
}
