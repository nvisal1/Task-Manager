using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using TaskManager.Constants;
using TaskManager.DataTransferObjects;
using TaskManager.Interfaces;
using TaskManager.Mappers;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    /// <summary>
    /// This class contains all endpoints related to task reads. 
    /// </summary>
    [ApiController]
    [Route("tasks")]
    public class TaskReadController : ControllerBase
    {
        private readonly ITaskService _taskSerivce;

        // Constants for all possible `get all tasks` parameter values
        private const string PARAMETER_EMPTY_STRING  = "";
        private const string PARAMETER_ASC           = "Asc";
        private const string PARAMETER_DESC          = "Desc";
        private const string PARAMETER_COMPLETED     = "Completed";
        private const string PARAMETER_NOT_COMPLETED = "NotCompleted";
        private const string PARAMETER_ALL           = "All";

        public TaskReadController(ITaskService taskService)
        {
            _taskSerivce = taskService;
        }

        /// <summary>
        /// This endpoint allows a requester to get a single task.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult<TaskResponse> GetTask([FromRoute] int id)
        {
            try
            {
                Task task = _taskSerivce.GetTaskById(id);

                // If the task does not exist, return an error to the client
                if (task == null)
                {
                    ErrorResponse errorResponse = ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_FOUND, ErrorMessages.NOT_FOUND, "Id", id.ToString());
                    return StatusCode((int)HttpStatusCode.NotFound, errorResponse);
                }

                return Mapper.MapTaskToTaskResponse(task);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This endpoint allows a requester to get all tasks.
        /// </summary>
        /// <param name="orderByDate"></param>
        /// <param name="taskStatus"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult<List<TaskResponse>> GetAllTasks([FromQuery] string orderByDate = "", [FromQuery] string taskStatus = "")
        {
            try
            {
                // If either of the optional parameters are invalid, return an error to the client
                var errorResponse = ValidateGetAllTaskParameters(orderByDate, taskStatus);
                if (errorResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponse);
                }

                List<Task>tasks = _taskSerivce.GetAllTasks(taskStatus, orderByDate);

                // convert each task into a task response
                List<TaskResponse> taskResponses = new List<TaskResponse>();
                foreach (Task task in tasks)
                {
                    TaskResponse taskReponse = Mapper.MapTaskToTaskResponse(task);
                    taskResponses.Add(taskReponse);
                }

                return taskResponses;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// check if orderByDate or taskStatus contains an invalid value
        /// 
        /// return an error response if an invalid value exists
        /// </summary>
        /// <param name="orderByDate"></param>
        /// <param name="taskStatus"></param>
        /// <returns></returns>
        private ErrorResponse ValidateGetAllTaskParameters(string orderByDate, string taskStatus)
        {
            if (!(orderByDate.Equals(PARAMETER_EMPTY_STRING) || orderByDate.Equals(PARAMETER_ASC) || orderByDate.Equals(PARAMETER_DESC)))
            {
                return ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_VALID, ErrorMessages.NOT_VALID, "orderByDate", orderByDate);
            };

            if (!(taskStatus.Equals(PARAMETER_EMPTY_STRING) || taskStatus.Equals(PARAMETER_COMPLETED) || taskStatus.Equals(PARAMETER_NOT_COMPLETED) || taskStatus.Equals(PARAMETER_ALL)))
            {
                return ErrorResponse.NewErrorResponse(ErrorNumbers.NOT_VALID, ErrorMessages.NOT_VALID, "taskStatus", taskStatus);
            }

            return null;
        }

    }
}