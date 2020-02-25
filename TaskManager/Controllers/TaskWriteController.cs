using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using TaskManager.Constants;
using TaskManager.CustomSettings;
using TaskManager.DataTransferObjects;
using TaskManager.ExtensionMethods;
using TaskManager.Interfaces;
using TaskManager.Mappers;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskWriteController : ControllerBase
    {
        private readonly ITaskService _taskSerivce;
        private readonly TaskLimits _taskLimits;

        public TaskWriteController(ITaskService taskService, IOptions<TaskLimits> taskLimits)
        {
            _taskSerivce = taskService;
            _taskLimits = taskLimits.Value;
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult<TaskResponse> CreateTask([FromBody] TaskWriteRequestPayload taskWriteRequestPayload)
        {
            try
            {
                var errorResponse = ValidateModelState(taskWriteRequestPayload);
                if (errorResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponse);
                }

                // Check to see if the Task already exists
                Task task = _taskSerivce.GetTaskByName(taskWriteRequestPayload.TaskName);

                if (task != null)
                {
                    ErrorResponse alreadyExistsErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.ALREADY_EXISTS, ErrorMessages.ALREADY_EXISTS, "TaskName", taskWriteRequestPayload.TaskName);
                    return StatusCode((int)HttpStatusCode.BadRequest, alreadyExistsErrorResponse);
                }
                if (!canAddMoreTasks())
                {
                    ErrorResponse atCapacityErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.AT_CAPACITY, ErrorMessages.AT_CAPACITY, null, null);
                    return StatusCode((int)HttpStatusCode.Forbidden, atCapacityErrorResponse);
                }

                if (!DateTime.TryParse(taskWriteRequestPayload.DueDate, out DateTime expectedDate))
                {
                    ErrorResponse notValidErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_VALID, ErrorMessages.NOT_VALID, "DueDate", taskWriteRequestPayload.DueDate);
                    return StatusCode((int)HttpStatusCode.Forbidden, notValidErrorResponse);
                }

                task = Mapper.MapTaskWriteRequestPayloadToTask(taskWriteRequestPayload);
                _taskSerivce.CreateTask(task);

                // Get the created task in order to get the generated id
                task = _taskSerivce.GetTaskByName(task.Name);

                TaskResponse response = Mapper.MapTaskToTaskResponse(task);
                return StatusCode((int)HttpStatusCode.Created, response);
                      
            } catch (Exception e)
            {
                var errorResponse = ValidateModelState(taskWriteRequestPayload);
                if (errorResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponse);
                }

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult UpdateTask([FromRoute] int id, [FromBody] TaskWriteRequestPayload taskWriteRequestPayload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Task task = _taskSerivce.GetTaskById(id);

                    if (task == null)
                    {
                        return StatusCode((int)HttpStatusCode.NotFound, new ErrorResponse()
                        {
                            ErrorNumber = 5,
                            ErrorDescription = "The entity could not be found",
                            ParameterName = "TaskName",
                            ParameterValue = taskWriteRequestPayload.TaskName,
                        });
                    }

                    if (!DateTime.TryParse(taskWriteRequestPayload.DueDate, out DateTime expectedDate))
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse()
                        {
                            ErrorNumber = 7,
                            ErrorDescription = "The parameter value is not valid",
                            ParameterName = "DueDate",
                            ParameterValue = taskWriteRequestPayload.DueDate,
                        });
                    }

                    _taskSerivce.UpdateTask(taskWriteRequestPayload.TaskName, taskWriteRequestPayload.DueDate, (bool)taskWriteRequestPayload.IsCompleted, task);
                }
                else
                {
                    List<ErrorResponse> errorResponses = BuildErrorResponseList(taskWriteRequestPayload);
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponses[0]);
                }
            }
            catch (Exception e)
            {
                if (!ModelState.IsValid)
                {
                    List<ErrorResponse> errorResponses = BuildErrorResponseList(taskWriteRequestPayload);
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponses[0]);
                }

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult DeleteTask([FromRoute] int id)
        {
            try
            {
                Task task = _taskSerivce.GetTaskById(id);

                if (task == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, new ErrorResponse()
                    {
                        ErrorNumber = 5,
                        ErrorDescription = "The entity could not be found",
                        ParameterName = "Id",
                        ParameterValue = id.ToString(),
                    });
                }

                _taskSerivce.DeleteTask(task);
            } catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool canAddMoreTasks()
        {
            long taskCount = _taskSerivce.GetTotalTaskCount();

            if (_taskLimits.MaxTaskEntries > taskCount)
            {
                return true;
            }

            return false;
        }

        private ErrorResponse ValidateModelState(TaskWriteRequestPayload taskWriteRequestPayload)
        {
            if (!ModelState.IsValid)
            {
                List<ErrorResponse> errorResponses = BuildErrorResponseList(taskWriteRequestPayload);
                return errorResponses[0];
            }

            return null;
        }

        private List<ErrorResponse> BuildErrorResponseList(TaskWriteRequestPayload taskWriteRequestPayload)
        {
            List<ErrorResponse> errorResponses = new List<ErrorResponse>();

            foreach (string key in ModelState.Keys)
            {
                if (ModelState[key].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    foreach (Microsoft.AspNetCore.Mvc.ModelBinding.ModelError error in ModelState[key].Errors)
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

            return errorResponses;
        }
    }
}