using TaskManager.Domain.Common;
using TaskManager.Domain.Entities.Rules;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Shared;


namespace TaskManager.Domain.Entities
{
    public class Task : Entity
    {
        public string TaskTitle { get; private set; } = string.Empty; 
        public string TaskText { get; private set; } = "Without text.";  
        public TaskPriority Priority { get; private set; } = TaskPriority.Low;
        public bool IsCompleted { get; private set; } = false; 
        public bool IsCanceled { get; private set; } = false;
        public DateTime DeadLine {  get; private set; } = DateTime.Today.AddDays(7);
        private Task() { } //Obligated to DB.
        private Task(Guid id,
            string taskTitle,
            string taskText,
            TaskPriority priority,
            DateTime deadLine) : base(id)
        {
            TaskTitle = taskTitle;
            TaskText = taskText;
            Priority = priority;
            DeadLine = deadLine;
        }

        public static Result<Task> CreateTask( //Factory
            string _taskTitle,
            string _taskText,
            TaskPriority _priority, 
            DateTime _deadLine)
        {


            var result = CheckRules(       //Here I check de bussiness rules
                new TaskDueDateCannotBePastRule(_deadLine),
                new TaskPriorityMustBeValidRule(_priority),
                new TaskTitleMustBeValidRule(_taskTitle));

            if (result.IsFailure) return result;
           
            var _id = Guid.NewGuid(); //Here is the Guid to Entity

            return Result.Success(new Task(_id, _taskTitle, _taskText, _priority, _deadLine));
        }

        public Result UpdateTask(
            string _taskTitle,
            string _taskText,
            TaskPriority _priority, 
            DateTime _deadLine)
        {
            var result = CheckRules(       
                new TaskDueDateCannotBePastRule(_deadLine),
                new TaskPriorityMustBeValidRule(_priority),
                new TaskTitleMustBeValidRule(_taskTitle));

            if (result.IsFailure) return result;

            TaskTitle = _taskTitle;
            TaskText = _taskText;
            Priority = _priority;
            DeadLine = _deadLine;

            return Result.Success();
        }
        
        public Result SetIsCompleted(bool _isCompleted)
        {
            var result = CheckRules(new TaskCannotBeCompletedAndCanceled(this.IsCanceled));
            if (result.IsFailure) return result;

            IsCompleted = _isCompleted;
            return Result.Success();
        } 

        public Result SetIsCanceled(bool _isCanceled)
        {
            var result = CheckRules(new TaskCannotBeCompletedAndCanceled(this.IsCompleted));
            if (result.IsFailure) return result;

            IsCanceled = _isCanceled;
            return Result.Success();
        }
    }
}
