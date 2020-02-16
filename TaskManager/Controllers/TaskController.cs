using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.DataTransferObjects;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController : ControllerBase
    {
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult<TaskResponse> createTask([FromBody] TaskWriteRequestPayload taskWriteRequestPayload)
        {

        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult updateTask([FromRoute] int id [FromBody] TaskWriteRequestPayload taskWriteRequestPayload)
        {

        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult deleteTask([FromRoute] int id)
        {

        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult<TaskResponse> getTask([FromRoute] int id)
        {

        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public ActionResult<TaskResponse[]> getAllTasks()
        {

        }

    }
}