using System;
using TaskManager.DataTransferObjects;
using TaskManager.Models;

namespace TaskManager.Mappers
{
    public class Mapper
    {
        public static TaskResponse MapTaskToTaskResponse(Task task)
        {
            return new TaskResponse()
            {
                Id = (int)task.Id,
                TaskName = task.Name,
                DueDate = task.DueDate.ToString("yyyy-MM-dd"),
                IsCompleted = task.IsCompleted,
            };
        }

        public static Task MapTaskWriteRequestPayloadToTask(TaskWriteRequestPayload taskWriteRequestPayload)
        {
            return new Task()
            {
                Name = taskWriteRequestPayload.TaskName,
                DueDate = Convert.ToDateTime(taskWriteRequestPayload.DueDate),
                IsCompleted = (bool)taskWriteRequestPayload.IsCompleted,
            };
        }
    }
}
