using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Task
    {
        /// <summary>
        /// id of the task
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        /// <summary>
        /// name of the task
        /// </summary>
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The date the task is due to be completed
        /// </summary>
        [Required]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// true if completed, false if not completed
        /// </summary>
        [Required]
        public bool IsCompleted { get; set; }
    }
}
