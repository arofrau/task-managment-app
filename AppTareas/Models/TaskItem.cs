using System.Diagnostics;

namespace AppTareas.Models
{
    public enum Frecuency
    {
        No_Repetition = 0,
        Daily = 1,
        Weekly = 2,
        EveryTwoWeeks = 3,
        Monthly = 4,
        Annually = 5,
        Woking_Days = 6 
    };
    public class TaskItem
    {   
        /// <summary>
        /// Id from a specific Task
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Tittle of the Task
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Description of the task, can be optional
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// The datetime in which the task was created
        /// </summary>
        public DateTime CreatedTimeDate { get; set; }
        /// <summary>
        /// Timedate where the task will expire, can be optional
        /// </summary>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Last DueDate we use when a task is marked as not yet completed
        /// </summary>

        public DateTime? PreviousDueDate { get; set; }
        /// <summary>
        /// Used to know if the task is for My Day TaskList
        /// </summary>
        public DateOnly? MyDayDate { get; set; }
        /// <summary>
        /// Frecuency states if the task has to be reiteraited and to which frecuency
        /// </summary>
        public Frecuency Frecuency { get; set; }
        /// <summary>
        /// Date where the task was completed.
        /// </summary>
        public DateOnly CompletedAt { get; set; }
        /// <summary>
        /// Marks if a task hasn't been completed and it had a due date
        /// </summary>
        public bool IsTaskOverdue { get; set; } = false;
        
        /// <summary>
        /// Foreign key of taskListId
        /// </summary>
        public int TaskListId { get; set; }
        /// <summary>
        /// TaskList owner of this TaskItem
        /// </summary>
        public TaskList? TaskList { get; set; }
        /// <summary>
        /// Foreign key of another task
        /// </summary>
        public int? ParentTaskId { get; set; }
        /// <summary>
        /// ParentTask that owns this TaskItem
        /// </summary>
        public TaskItem? ParentTask { get; set; }
        /// <summary>
        /// A task can have subtasks inside.
        /// </summary>
        public List<TaskItem> SubTasks { get; set; } = new();
    }
}
