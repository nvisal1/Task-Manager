using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using TaskManager.DataTransferObjects;
using TaskManager.ExtensionMethods;
using TaskManager.Models;

namespace TaskManager.Mappers
{
    /// <summary>
    /// This class contains methods that translate
    /// entity types into other entity types
    /// </summary>
    public class Mapper
    {
        /// <summary>
        /// Converts a Task into a TaskResponse
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts a TaskWriteRequestPayload into a Task
        /// </summary>
        /// <param name="taskWriteRequestPayload"></param>
        /// <returns></returns>
        public static Task MapTaskWriteRequestPayloadToTask(TaskWriteRequestPayload taskWriteRequestPayload)
        {
            return new Task()
            {
                Name = taskWriteRequestPayload.TaskName,
                DueDate = Convert.ToDateTime(taskWriteRequestPayload.DueDate),
                IsCompleted = (bool)taskWriteRequestPayload.IsCompleted,
            };
        }

        /// <summary>
        /// Uses ModelStateDictionary to convert a TaskWriteRequestPayload into an ErrorResponse
        /// </summary>
        /// <param name="taskWriteRequestPayload"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        public static ErrorResponse MapTaskWriteRequestPayloadToErrorResponse(TaskWriteRequestPayload taskWriteRequestPayload, ModelStateDictionary ModelState)
        {
            List<ErrorResponse> errorResponses = new List<ErrorResponse>();

            // Iterate over the errors in Model State and convert them into ErrorResponses
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
                        // A null reference exception occured while assigning the parameter value. 
                        // The requester provided us with a null parameter value,
                        // create an ErrorResponse with a null parameter value and continue
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

            // Return the first ErrorResponse 
            return errorResponses[0];
        }
    }
}
