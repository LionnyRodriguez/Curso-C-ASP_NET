using TaskManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.API.DTOs;
using TaskManager.API.Extensions;
using TaskManager.API.Persistence;
using TaskManager.Domain.Enums;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;

        private readonly TaskContext _taskContext;

        public TaskController(ILogger<TaskController> logger, TaskContext taskContext)
        {
            _logger = logger;
            _taskContext = taskContext;
        }

        //-------------------------------------------------------Post-------------------------------------------------------------

        [HttpPost]

        public async Task<ActionResult<Guid>> CreateTask(
            [FromBody] CreateTaskRequest createTaskRequest,
            CancellationToken cancellationToken)
        {
            //Auxiliar variables
            string auxTaskText;
            TaskPriority auxPriority; //I declarate this variable how nullable 'cause when i try asign the createTaskRequest property
            //it can be null and generate an error
            DateTime auxDeadLine;

            //Validations
            if (string.IsNullOrEmpty(createTaskRequest._TaskTitle))
                return BadRequest("Title is required.");

            if (string.IsNullOrEmpty(createTaskRequest._TaskText))
                auxTaskText = "Without text.";
            else auxTaskText = createTaskRequest._TaskText;

            if (createTaskRequest._TaskPriority == null)
                auxPriority = TaskPriority.Low;
            else auxPriority = (TaskPriority)createTaskRequest._TaskPriority; //Explicit conversion

            if (createTaskRequest._DeadLine == null)
                auxDeadLine = DateTime.Today.AddDays(7);
            else auxDeadLine = (DateTime)createTaskRequest._DeadLine;

            var result = Domain.Entities.Task.CreateTask(
                createTaskRequest._TaskTitle,
                auxTaskText,
                auxPriority,
                auxDeadLine);

            if (result.IsFailure)
                return this.FromResult(result);

            //Persistence
            try
            {
                _taskContext.Tasks.Add(result.Value!);

                await _taskContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }

            return Created("api/Tasks", result.Value!.Id);
        }

        //-----------------------------------------------------Get with route parameters-------------------------------------------------------------------

        [HttpGet("{id}")]

        public async Task<ActionResult<TaskResponse>> GetTaskById(Guid id, CancellationToken cancellationToken)
        {
            Task? task = await _taskContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (task is null)
                return NotFound($"The Task with id {id} could not be found.");

            TaskResponse taskResponse = new TaskResponse(
                task.Id,
                task.TaskTitle,
                task.TaskText,
                task.Priority,
                task.IsCompleted,
                task.IsCanceled,
                task.DeadLine
                );

            return Ok(taskResponse);
        }

        //-----------------------------------------------------Get with filtering parameters-------------------------------------------------------------------

        [HttpGet]

        public async Task<ActionResult<IEnumerable<TaskResponse>>> ListTasks(
            [FromQuery] TaskRequestParametersForGet parameters,
            CancellationToken cancellationToken)
        {
            if (parameters._priority == null &&
                parameters._isCompleted == null &&
                parameters._isCanceled == null &&
                parameters._deadLine == null)
            {
                return BadRequest("At least one filtering criterion is required.");
            }
            if (parameters._isCompleted != null &&
                parameters._isCanceled != null &&
                parameters._isCompleted == parameters._isCanceled)
            {
                return BadRequest("A task cannot be both cancelled and completed at the same time due to business rules.");
            }

            var query = _taskContext.Tasks.AsQueryable();

            if (parameters._priority != null)
                query = query.Where(x => x.Priority == parameters._priority);

            if (parameters._isCompleted != null)
                query = query.Where(x => x.IsCompleted == parameters._isCompleted);

            if (parameters._isCanceled != null)
                query = query.Where(x => x.IsCanceled == parameters._isCanceled);

            if (parameters._deadLine != null)
                query = query.Where(x => x.DeadLine == parameters._deadLine);

            List<Task> Tasks = await query.ToListAsync(cancellationToken);

            List<TaskResponse> Responses = [];
            foreach (var task in Tasks)
            {
                Responses.Add(new TaskResponse(
                    task.Id,
                    task.TaskTitle,
                    task.TaskText,
                    task.Priority,
                    task.IsCompleted,
                    task.IsCanceled,
                    task.DeadLine));
            }

            return Ok(Responses);
        }

        //-------------------------------------------------Put--------------------------------------------------------------------

        [HttpPut("{id}")]

        public async System.Threading.Tasks.Task<ActionResult> UpdateTaskController(
            Guid id,
            [FromBody] UpdateTaskRequest _updateTaskRequest,
            CancellationToken cancellationToken)
        {
            if (_updateTaskRequest._TaskTitle == null &&
                _updateTaskRequest._TaskText == null &&
                _updateTaskRequest._TaskPriority == null &&
                _updateTaskRequest._DeadLine == null)
            {
                return BadRequest("At least one parameter to update is required.");
            }

            var TaskToUpdate = _taskContext.Tasks.FirstOrDefault(x => x.Id == id);

            if (TaskToUpdate != null)
                return NotFound($"The task with id {id} could not be found.");

            //Parameters to update
            string newTaskTitle;
            string newTaskText;
            TaskPriority? newTaskPriority;
            DateTime? newDeadLine;

            //Validations

            if (_updateTaskRequest._TaskTitle is null)
            {
                newTaskTitle = TaskToUpdate.TaskTitle;
            }
            else newTaskTitle = _updateTaskRequest._TaskTitle;


            if (_updateTaskRequest._TaskText is null)
            {
                newTaskText = TaskToUpdate.TaskText;
            }
            else newTaskText = _updateTaskRequest._TaskText;


            if (_updateTaskRequest._TaskPriority is null)
            {
                newTaskPriority = TaskToUpdate.Priority;
            }
            else newTaskPriority = _updateTaskRequest._TaskPriority;


            if (_updateTaskRequest._DeadLine is null)
            {
                newDeadLine = TaskToUpdate.DeadLine;
            }
            else newDeadLine = _updateTaskRequest._DeadLine;

            //Result
            var result = TaskToUpdate.UpdateTask(
                newTaskTitle,
                newTaskText,
                newTaskPriority,
                newDeadLine);

            if (result.IsFailure)
                return this.FromResult(result);

            try
            {
                await _taskContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new DbUpdateException("A database error occurred while attempting to save changes. " +
                    "Try again and if the problem persists, contact support.");
            }

            return NoContent();
        }

        //-------------------------------------------------IsCompleted or IsCanceled Patch--------------------------------------------------------------------

        [HttpPatch("{id}")]
        public async System.Threading.Tasks.Task<ActionResult> PatchIsCompletedOrIsCanceled(
            Guid id,
            [FromBody] SetterIsCompletedOrIsCanceled _setterIsCompletedOrIsCanceled,
            CancellationToken cancellationToken)
        {
            if (_setterIsCompletedOrIsCanceled._isCompleted == null &&
                _setterIsCompletedOrIsCanceled._isCanceled == null)
                return BadRequest("Without parameters to set.");

                var TaskToPatch = _taskContext.Tasks.FirstOrDefault(x => x.Id == id);
            if (TaskToPatch is null) 
                return NotFound($"The task with id {id} could not be found.");

            if (_setterIsCompletedOrIsCanceled._isCompleted.HasValue)
            {
                var result = TaskToPatch.SetIsCompleted(_setterIsCompletedOrIsCanceled._isCompleted);
                if (result.IsFailure) return this.FromResult(result);
            }

            if (_setterIsCompletedOrIsCanceled._isCanceled.HasValue)
            {
                var result = TaskToPatch.SetIsCanceled(_setterIsCompletedOrIsCanceled._isCanceled);
                if (result.IsFailure) return this.FromResult(result);
            }

            try
            {
                await _taskContext.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating task {TaskId}", id);
                return Problem("An error occurred while saving changes. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating task {TaskId}", id);
                return Problem("Internal server error.");
            }

            return NoContent();
        }

        //----------------------------------------------------------Delete------------------------------------------------------------------

        [HttpDelete("{id}")]
        public async System.Threading.Tasks.Task<ActionResult> DeleteTask(
            Guid id,
            CancellationToken cancellationToken)
        {
            var TaskToDelete = _taskContext.Tasks.FirstOrDefault(x => x.Id == id);

            if(TaskToDelete is null)
                return NotFound($"The task with id {id} could not be found.");

            try
            {
                _taskContext.Tasks.Remove(TaskToDelete);
                await _taskContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting task {TaskId}", id);
                return Problem("An error occurred while deleting the task. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting task {TaskId}", id);
                return Problem("Internal server error.");
            }

            return NoContent();
        }
    }
}
