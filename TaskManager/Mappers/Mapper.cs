using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using TaskManager.DataTransferObjects;
using TaskManager.ExtensionMethods;
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

        public static ErrorResponse MapTaskWriteRequestPayloadToErrorResponse(TaskWriteRequestPayload taskWriteRequestPayload, ModelStateDictionary ModelState)
        {
            List<ErrorResponse> errorResponses = new List<ErrorResponse>();

            foreach (string key in ModelState.Keys)
            {
                if (ModelState[key].ValidationState == ModelValidationState.Invalid)
                {
                    foreach (ModelError error in ModelState[key].Errors)
                    {
                        string cleansedKey = key.CleanseModelStateKey();

                        try
                        {
                            ErrorResponse errorResponse = new ErrorResponse();
                            (errorResponse.ErrorDescription, errorResponse.ErrorNumber) = ErrorResponse.GetErrorMessage(error.ErrorMessage);
                            errorResponse.ParameterName = cleansedKey;
                            errorResponse.ParameterValue = (string)typeof(TaskWriteRequestPayload).GetProperty(cleansedKey).GetValue(taskWriteRequestPayload);
                            errorResponses.Add(errorResponse);
                        }
                        catch (NullReferenceException _)
                        {
                            ErrorResponse errorResponse = new ErrorResponse();
                            (errorResponse.ErrorDescription, errorResponse.ErrorNumber) = ErrorResponse.GetErrorMessage(error.ErrorMessage);
                            errorResponse.ParameterName = cleansedKey;
                            errorResponse.ParameterValue = null;
                            errorResponses.Add(errorResponse);
                        }

                    }
                }
            }

            return errorResponses[0];
        }
    }
}
