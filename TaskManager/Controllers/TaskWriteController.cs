using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using TaskManager.Constants;
using TaskManager.CustomSettings;
using TaskManager.DataTransferObjects;
using TaskManager.Interfaces;
using TaskManager.Mappers;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    /// <summary>
    /// This class contains all endpoints related to task writes.
    /// </summary>
    [ApiController]
    [Route("tasks")]
    public class TaskWriteController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly TaskLimits _taskLimits;

        public TaskWriteController(ITaskService taskService, IOptions<TaskLimits> taskLimits)
        {
            _taskService = taskService;
            _taskLimits = taskLimits.Value;
        }

        /// <summary>
        /// This endpoint allows a requester to save a new task.
        /// </summary>
        /// <param name="taskWriteRequestPayload"></param>
        /// <returns></returns>
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
                // If the request body is invalid, return an error to the client
                var errorResponse = ValidateModelState(taskWriteRequestPayload);
                if (errorResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponse);
                }

                // Check to see if the Task already exists
                Task task = _taskService.GetTaskByName(taskWriteRequestPayload.TaskName);

                // If the task already exists, return an error to the client
                if (task != null)
                {
                    ErrorResponse alreadyExistsErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.ALREADY_EXISTS, ErrorMessages.ALREADY_EXISTS, "TaskName", taskWriteRequestPayload.TaskName);
                    return StatusCode((int)HttpStatusCode.Conflict, alreadyExistsErrorResponse);
                }

                // If task capacity has already been reached, return an error to the client
                if (!CanAddMoreTasks())
                {
                    ErrorResponse atCapacityErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.AT_CAPACITY, ErrorMessages.AT_CAPACITY, null, null);
                    return StatusCode((int)HttpStatusCode.Forbidden, atCapacityErrorResponse);
                }

                // If the due date cannot be parsed correctly, return an error to the client.
                // This formatting error is not caught in the above model state check
                if (!DateTime.TryParse(taskWriteRequestPayload.DueDate, out DateTime expectedDate))
                {
                    ErrorResponse notValidErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_VALID, ErrorMessages.NOT_VALID, "DueDate", taskWriteRequestPayload.DueDate);
                    return StatusCode((int)HttpStatusCode.BadRequest, notValidErrorResponse);
                }

                // Convert the given request body into a Task and save it
                task = Mapper.MapTaskWriteRequestPayloadToTask(taskWriteRequestPayload);
                _taskService.CreateTask(task);

                // Get the created task in order to get the generated id
                task = _taskService.GetTaskByName(task.Name);

                TaskResponse response = Mapper.MapTaskToTaskResponse(task);
                Response.Headers.Add("Location", HttpContext.Request.Host + "/tasks/" + task.Id);
                return StatusCode((int)HttpStatusCode.Created, response);
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
                // It is possible to reach this line without checking the model
                // state above. Make sure that the model state is valid.
                //
                // Return internal server error if the model state is valid
                var errorResponse = ValidateModelState(taskWriteRequestPayload);
                if (errorResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponse);
                }

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This endpoint allows a requester to update a task.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskWriteRequestPayload"></param>
        /// <returns></returns>
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
                // If the request body is invalid, return an error to the client
                var errorResponse = ValidateModelState(taskWriteRequestPayload);
                if (errorResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponse);
                }

                // Get the task that the requester wants to update
                Task task = _taskService.GetTaskById(id);
                
                // If the specified task does not exists, return an error to the client
                if (task == null)
                {
                    ErrorResponse notFoundErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_FOUND, ErrorMessages.NOT_FOUND, "TaskName", taskWriteRequestPayload.TaskName);
                    return StatusCode((int)HttpStatusCode.NotFound, notFoundErrorResponse);
                }

                // Query by the given task name before updating the specified document
                Task duplicateTask = _taskService.GetTaskByName(taskWriteRequestPayload.TaskName);

                // If there is a task with the given name under a different id, return an error to the client
                if (duplicateTask != null)
                {
                    if (duplicateTask.Id != id)
                    {
                        ErrorResponse notFoundErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.ALREADY_EXISTS, ErrorMessages.ALREADY_EXISTS, "TaskName", taskWriteRequestPayload.TaskName);
                        return StatusCode((int)HttpStatusCode.Conflict, notFoundErrorResponse);
                    }
                }

                // If the due date cannot be parsed correctly, return an error to the client.
                // This formatting error is not caught in the above model state check
                if (!DateTime.TryParse(taskWriteRequestPayload.DueDate, out DateTime expectedDate))
                {
                    ErrorResponse badRequestErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_VALID, ErrorMessages.NOT_VALID, "DueDate", taskWriteRequestPayload.DueDate);
                    return StatusCode((int)HttpStatusCode.BadRequest, badRequestErrorResponse);
                }

                _taskService.UpdateTask(taskWriteRequestPayload.TaskName, taskWriteRequestPayload.DueDate, (bool)taskWriteRequestPayload.IsCompleted, task);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // It is possible to reach this line without checking the model
                // state above. Make sure that the model state is valid.
                //
                // Return internal server error if the model state is valid
                var errorResponse = ValidateModelState(taskWriteRequestPayload);
                if (errorResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponse);
                }

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// This endpoint allows a requester to delete a task.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult DeleteTask([FromRoute] int id)
        {
            try
            {
                Task task = _taskService.GetTaskById(id);

                // If the specified task does not exist, return an error to the client
                if (task == null)
                {
                    ErrorResponse notFoundErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_FOUND, ErrorMessages.NOT_FOUND, "Id", id.ToString());
                    return StatusCode((int)HttpStatusCode.NotFound, notFoundErrorResponse);
                }

                _taskService.DeleteTask(task);
            } catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// check if task capacity has been reached
        /// 
        /// If it has been reached, no more tasks can be added ... return false
        /// Otherwise, return true
        /// </summary>
        /// <returns></returns>
        private bool CanAddMoreTasks()
        {
            long taskCount = _taskService.GetTotalTaskCount();

            if (_taskLimits.MaxTaskEntries > taskCount)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the current model state is valid.
        /// If it is not, use the model state object to 
        /// generate an error response
        /// </summary>
        /// <param name="taskWriteRequestPayload"></param>
        /// <returns></returns>
        private ErrorResponse ValidateModelState(TaskWriteRequestPayload taskWriteRequestPayload)
        {
            if (!ModelState.IsValid)
            {
                ErrorResponse errorResponse = Mapper.MapTaskWriteRequestPayloadToErrorResponse(taskWriteRequestPayload, ModelState);
                return errorResponse;
            }

            return null;
        }
    }
}