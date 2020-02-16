using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskManager.CustomSettings;
using TaskManager.Data;
using TaskManager.DataTransferObjects;

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
            this._context = context;
            this._taskLimits = taskLimits.Value;
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
            return NotFound();
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
            return NotFound();
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult<TaskResponse> GetTask([FromRoute] int id)
        {
            return NotFound();
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult<TaskResponse[]> GetAllTasks()
        {
            return NotFound();
        }

    }
}