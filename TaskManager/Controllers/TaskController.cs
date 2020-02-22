using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using TaskManager.CustomSettings;
using TaskManager.Data;
using TaskManager.DataTransferObjects;
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
        public ActionResult<TaskResponse> CreateTask([FromBody] TaskWriteRequestPayload taskWriteRequestPayload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check to see if the Task already exists
                    Task task = (from t in _context.Tasks where t.Name == taskWriteRequestPayload.TaskName select t).Single();

                    if (task == null)
                    {
                        using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                        {
                            if (canAddMoreTasks())
                            {
                                return StatusCode((int)HttpStatusCode.Forbidden, $"Task limit reach MaxTasks: [{_taskLimits.MaxTaskEntries}]");
                            }

                            task = new Task()
                            {
                                Name = taskWriteRequestPayload.TaskName,
                                DueDate = Convert.ToDateTime(taskWriteRequestPayload.DueDate),
                                IsCompleted = taskWriteRequestPayload.IsCompleted,
                            };

                            _context.Tasks.Add(task);
                            _context.SaveChanges();

                            transaction.Commit();

                            task = (from t in _context.Tasks where t.Name == taskWriteRequestPayload.TaskName select t).Single();

                            TaskResponse response = new TaskResponse()
                            {
                                Id = (int)task.Id,
                                TaskName = task.Name,
                                DueDate = task.DueDate.ToString(),
                                IsCompleted = task.IsCompleted,
                            };

                            return response;
                        }
                    }
                    else
                    {
                        return BadRequest(ModelState);
                    }
                }
            } catch (System.Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return NoContent();
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
            return NotFound();
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
                Task task = (from t in _context.Tasks where t.Id == id select t).Single();

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
                Task task = (from t in _context.Tasks where t.Id == id select t).Single();

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

    }
}