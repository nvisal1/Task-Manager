using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using TaskManager.CustomSettings;
using TaskManager.DataTransferObjects;
using TaskManager.ExtensionMethods;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskSerivce;
        private readonly TaskLimits _taskLimits;

        public TaskController(ITaskService taskService, IOptions<TaskLimits> taskLimits)
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
        public async System.Threading.Tasks.Task<ActionResult<TaskResponse>> CreateTask([FromBody] TaskWriteRequestPayload taskWriteRequestPayload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check to see if the Task already exists
                    Task task = _taskSerivce.GetTaskByName(taskWriteRequestPayload.TaskName);

                    if (task == null)
                    {
                        if (!canAddMoreTasks())
                        {

                            return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse()
                            {
                                ErrorNumber = 4,
                                ErrorDescription = "The maximum number of entities have been created. No further entities can be created at this time.",
                                ParameterName = null,
                                ParameterValue = null,
                            });
                        }

                        if (!DateTime.TryParse(taskWriteRequestPayload.DueDate, out DateTime expectedDate))
                        {
                            return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse()
                            {
                                ErrorNumber = 7,
                                ErrorDescription = "The parameter value is not valid",
                                ParameterName = "DueDate",
                                ParameterValue = taskWriteRequestPayload.DueDate,
                            });
                        }

                        task = new Task()
                        {
                            Name = taskWriteRequestPayload.TaskName,
                            DueDate = Convert.ToDateTime(taskWriteRequestPayload.DueDate),
                            IsCompleted = (bool)taskWriteRequestPayload.IsCompleted,
                        };

                        _taskSerivce.CreateTask(task);

                        // Get the created task in order to get the generated id
                        task = _taskSerivce.GetTaskByName(task.Name);

                        TaskResponse response = new TaskResponse()
                        {
                            Id = (int)task.Id,
                            TaskName = task.Name,
                            DueDate = taskWriteRequestPayload.DueDate,
                            IsCompleted = task.IsCompleted,
                        };

                        return StatusCode((int)HttpStatusCode.Created, response);
                        
                    }
                    else
                    {
                        return StatusCode((int) HttpStatusCode.BadRequest, new ErrorResponse()
                        {
                            ErrorNumber = 1,
                            ErrorDescription = "The entity already exists",
                            ParameterName = "TaskName",
                            ParameterValue = taskWriteRequestPayload.TaskName,
                        });
                    }
                }
                else
                {
                    List<ErrorResponse> errorResponses = BuildErrorResponseList(taskWriteRequestPayload);
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponses[0]);
                }
            } catch (Exception e)
            {
                if (!ModelState.IsValid)
                {
                    List<ErrorResponse> errorResponses = BuildErrorResponseList(taskWriteRequestPayload);
                    return StatusCode((int)HttpStatusCode.BadRequest, errorResponses[0]);
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

                return new TaskResponse()
                {
                    Id = (int)task.Id,
                    TaskName = task.Name,
                    DueDate = task.DueDate.ToString("yyyy-MM-dd"),
                    IsCompleted = task.IsCompleted,
                };

            } catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult<List<TaskResponse>> GetAllTasks([FromQuery] string orderByDate = "", [FromQuery] string taskStatus = "")
        {
            try
            {
                List<Task> tasks = new List<Task>{};

                if (!(orderByDate.Equals("") || orderByDate.Equals("Asc") || orderByDate.Equals("Desc")))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse()
                    {
                        ErrorNumber = 7,
                        ErrorDescription = "The parameter value is not valid",
                        ParameterName = "orderByDate",
                        ParameterValue = orderByDate,
                    });
                }

                if (!(taskStatus.Equals("") || taskStatus.Equals("Completed") || taskStatus.Equals("NotCompleted") || taskStatus.Equals("All")))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse()
                    {
                        ErrorNumber = 7,
                        ErrorDescription = "The parameter value is not valid",
                        ParameterName = "taskStatus",
                        ParameterValue = taskStatus,
                    });
                }

                tasks = _taskSerivce.GetAllTasks(taskStatus, orderByDate);

                List<TaskResponse> taskResponses = new List<TaskResponse>();

                foreach (Task task in tasks)
                {
                    TaskResponse taskReponse = MapTaskToTaskResponse(task);
                    taskResponses.Add(taskReponse);
                }

                return taskResponses;

            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
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

        private TaskResponse MapTaskToTaskResponse(Task task)
        {
            return new TaskResponse()
            {
                TaskName = task.Name,
                DueDate = task.DueDate.ToString("yyyy-MM-dd"),
                IsCompleted = task.IsCompleted,
            };
        }

    }
}