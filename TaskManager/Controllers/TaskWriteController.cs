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

                if (!CanAddMoreTasks())
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
                      
            } 
            catch (Exception e)
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
                var errorResponse = ValidateModelState(taskWriteRequestPayload);
                if (errorResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponse);
                }

                Task task = _taskSerivce.GetTaskById(id);

                if (task == null)
                {
                    ErrorResponse notFoundErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_FOUND, ErrorMessages.NOT_FOUND, "TaskName", taskWriteRequestPayload.TaskName);
                    return StatusCode((int)HttpStatusCode.NotFound, notFoundErrorResponse);
                }

                if (!DateTime.TryParse(taskWriteRequestPayload.DueDate, out DateTime expectedDate))
                {
                    ErrorResponse badRequestErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_VALID, ErrorMessages.NOT_VALID, "DueDate", taskWriteRequestPayload.DueDate);
                    return StatusCode((int)HttpStatusCode.BadRequest, badRequestErrorResponse);
                }

                _taskSerivce.UpdateTask(taskWriteRequestPayload.TaskName, taskWriteRequestPayload.DueDate, (bool)taskWriteRequestPayload.IsCompleted, task);
            }
            catch (Exception e)
            {
                var errorResponse = ValidateModelState(taskWriteRequestPayload);
                if (errorResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponse);
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
                    ErrorResponse notFoundErrorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_FOUND, ErrorMessages.NOT_FOUND, "Id", id.ToString());
                    return StatusCode((int)HttpStatusCode.NotFound, notFoundErrorResponse);
                }

                _taskSerivce.DeleteTask(task);
            } catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool CanAddMoreTasks()
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
                ErrorResponse errorResponse = Mapper.MapTaskWriteRequestPayloadToErrorResponse(taskWriteRequestPayload, ModelState);
                return errorResponse;
            }

            return null;
        }
    }
}