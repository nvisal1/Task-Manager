using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using TaskManager.CustomSettings;
using TaskManager.Data;
using TaskManager.DataTransferObjects;
using TaskManager.ExtensionMethods;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TaskLimits _taskLimits;

        public TaskController(AppDbContext context, IOptions<TaskLimits> taskLimits)
        {
            _context = context;
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
                    Task task = (from t in _context.Tasks where t.Name.Equals(taskWriteRequestPayload.TaskName) select t).SingleOrDefault();

                    if (task == null)
                    {
                        using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
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

                            _context.Tasks.Add(task);
                            _context.SaveChanges();

                            transaction.Commit();

                            task = (from t in _context.Tasks where t.Name == taskWriteRequestPayload.TaskName select t).Single();

                            TaskResponse response = new TaskResponse()
                            {
                                Id = (int)task.Id,
                                TaskName = task.Name,
                                DueDate = taskWriteRequestPayload.DueDate,
                                IsCompleted = task.IsCompleted,
                            };

                            return StatusCode((int)HttpStatusCode.Created, response);
                        }
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
                    Task task = (from t in _context.Tasks where t.Id == id select t).SingleOrDefault();

                    if (task == null)
                    {
                        return NotFound();
                    }

                    task.Name = taskWriteRequestPayload.TaskName;
                    task.DueDate = Convert.ToDateTime(taskWriteRequestPayload.DueDate);
                    task.IsCompleted = (bool)taskWriteRequestPayload.IsCompleted;

                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return NoContent();
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
                Task task = (from t in _context.Tasks where t.Id == id select t).SingleOrDefault();

                if (task == null)
                {
                    return NotFound();
                }

                _context.Tasks.Remove(task);
                _context.SaveChanges();
            } catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return NoContent();
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
                Task task = (from t in _context.Tasks where t.Id == id select t).SingleOrDefault();

                if (task == null)
                {
                    return NotFound();
                }

                return new TaskResponse()
                {
                    Id = (int)task.Id,
                    TaskName = task.Name,
                    DueDate = task.DueDate.ToString(),
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
        public ActionResult<TaskResponse[]> GetAllTasks()
        {
            try
            {
                TaskResponse[] tasks = (from t in _context.Tasks 
                                select new TaskResponse()
                                {
                                    Id = (int)t.Id,
                                    TaskName = t.Name,
                                    DueDate = t.DueDate.ToString(),
                                    IsCompleted = t.IsCompleted,
                                })
                                .ToArray();

                return tasks;

            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        private bool canAddMoreTasks()
        {
            long totalTasks = (from t in _context.Tasks select t).Count();

            if (_taskLimits.MaxTaskEntries > totalTasks)
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

    }
}