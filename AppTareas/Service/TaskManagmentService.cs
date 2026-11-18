using AppTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AppTareas.Service
{
    public class TaskManagmentService
    {
        private readonly List<TaskItem> TasksItems = new();
        private readonly List<TaskList> TasksLists = new();

        private  int idTaskItem = 3;
        private  int idTaskList = 2;
        private DateTime minDueDate = DateTime.Today.AddYears(-20);
        private DateTime maxDueDate = DateTime.Today.AddYears(20);
        


        public TaskManagmentService()
        {
            TasksItems = new List<TaskItem>
            {
                new TaskItem(){Id = 1,Title = "Task_1",CreatedTimeDate =  new DateTime(2026, 11, 20),DueDate = new DateTime(2026, 11, 20), Frecuency = Frecuency.No_Weekends,TaskListId = 1},
                new TaskItem(){Id = 2,Title = "Task_2",CreatedTimeDate = DateTime.Today, Frecuency = Frecuency.No_Repetition,TaskListId = 1}

            };
            TasksLists = new List<TaskList>
            {
                new TaskList (){Id = 1,Name="TaskList_1",Relevance=1,TasksItem = TasksItems.Where(t => t.TaskListId == 1).ToList(),UserId = 0}
            };
        }
        /// <summary>
        /// Returns only the tasklist available, without there tasks
        /// </summary>
        /// <returns></returns>
        public  List<TaskList> GetAllTasksLists() => TasksLists;

        public  TaskItem? GetTaskItem(int id)
        {
            var taskItem = TasksItems.FirstOrDefault(x => x.Id == id);
            if (taskItem == null)
                return null;


            var subTask = TasksItems.Where(t => t.ParentTaskId == id).ToList();

            return new TaskItem(taskItem.Id,taskItem.Title,taskItem.Description,taskItem.CreatedTimeDate,taskItem.IsTaskOverdue,taskItem.DueDate,taskItem.PreviousDueDate,
                taskItem.MyDayDate,taskItem.Frecuency,taskItem.CompletedAt,subTask,
                taskItem.TaskListId,taskItem.ParentTaskId);
        }


        /// <summary>
        /// Gets the taskList and it's associated tasks as well as subtask foreach task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TaskList? GetTaskList(int id)
        {
            var list = TasksLists.FirstOrDefault(l => l.Id == id);
            if (list is null) return null;

            var TasksItem = TasksItems.Where(t => t.TaskListId == id && (t.ParentTaskId == null || t.ParentTaskId == 0)).ToList();
            foreach(TaskItem task in TasksItem)
            {
                var subTaskList = TasksItems.Where(st => st.ParentTaskId == task.Id).ToList();

                task.SubTasks = subTaskList;
            }

            return new TaskList(list.Id,list.Name,list.Relevance, TasksItem);

        }
        /// <summary>
        /// Adds a new taskList
        /// </summary>
        /// <param name="taskList"></param>
        public void AddTaskList(TaskList taskList)
        {
            taskList.Id = idTaskList++;
            TasksLists.Add(taskList);
        }
        /// <summary>
        ///  Adds a TaskItem to a TaskList, and the TasksItems List.
        /// </summary>
        /// <param name="taskItem"></param> the taskItem we want to add, it can come with a taskList, taskListId or ParentTaskId
        public  void AddTaskItem(TaskItem taskItem)
        {

            taskItem.IsTaskOverdue = DateTime.Today > taskItem.DueDate;
            taskItem.CompletedAt = null;
            taskItem.CreatedTimeDate = DateTime.Today;



            if (!TasksLists.Any(t => t.Id == taskItem.TaskListId))
                throw new KeyNotFoundException("TaskList doesn't exist");
            if(taskItem.ParentTaskId.HasValue && taskItem.ParentTaskId !=0)
            {
               TaskItem taskItemParent = GetTaskItem(taskItem.ParentTaskId.Value);
                if (taskItem.ParentTaskId == 0) taskItem.ParentTaskId = null;
                if (taskItemParent.ParentTaskId == 0) taskItemParent.ParentTaskId = null;


                if (taskItemParent is null)
                    throw new KeyNotFoundException("Parent task doesn't exist.");

                if (taskItemParent.ParentTaskId.HasValue)
                    throw new InvalidOperationException("A subtask can't have subtasks.");
                if (taskItemParent.TaskListId != taskItem.TaskListId)
                    throw new InvalidOperationException("Parent task is on another list");

                taskItem.TaskListId = taskItemParent.TaskListId;
                taskItem.ParentTaskId = taskItemParent.Id;

                taskItemParent.SubTasks.Add(taskItem);


            }
            else if (taskItem.TaskListId == 0 && taskItem.TaskList != null)
                taskItem.TaskListId = taskItem.TaskList.Id;

            if (taskItem.DueDate > maxDueDate || taskItem.DueDate < minDueDate)
                throw new InvalidOperationException($"{taskItem.DueDate} must be between {minDueDate:yyyy-MM-dd} and {maxDueDate:yyyy-MM-dd}.");
            if (taskItem.MyDayDate < DateOnly.FromDateTime(DateTime.Today) || taskItem.MyDayDate > DateOnly.FromDateTime(DateTime.Today.AddDays(7)))
                throw new InvalidOperationException($"{taskItem.MyDayDate} must be between {DateOnly.FromDateTime(DateTime.Today):yyyy-MM-dd} and {DateOnly.FromDateTime(DateTime.Today.AddDays(7)):yyyy-MM-dd}.");

            taskItem.Id = idTaskItem++;


            TasksItems.Add(taskItem);
        }
        /// <summary>
        /// Deletes the taskItem with the corresponding id, as well as it's subtask
        /// </summary>
        /// <param name="id"></param>
        public  void DeleteTaskItem(int id)
        {
            var taskItem = GetTaskItem(id);
            if (taskItem == null)
                return;
            TasksItems.RemoveAll(t => t.Id == id || t.ParentTaskId == id);

        }
        /// <summary>
        /// Deletes the taskList with the corresponding id, as well as it's task
        /// </summary>
        /// <param name="id"></param>
        public void DeleteTaskList(int id)
        {
            var taskList = GetTaskList(id);
            if (taskList == null)
                return;

            TasksLists.RemoveAll(tl => tl.Id == id);
            foreach(var task in taskList.TasksItem)
            {
                DeleteTaskItem(task.Id);
            }

        }
        /// <summary>
        /// Update a given taskItem.
        /// It returns null if the TaskItem doesn't exist
        /// It throws an exception if the user creates a subtask for a subtask.
        /// It throws an exception if the user creates a DueDate witha  later date than its parent Task DueDate
        /// </summary>
        /// <param name="taskItem"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public  void UpdateTaskItem(TaskItem taskItem)
        {
            var index = TasksItems.FindIndex(index => index.Id == taskItem.Id);

            if (index == -1)
                return;
            // New DueDate over new Frecuency 
            if(TasksItems[index].DueDate != taskItem.DueDate )
            {
                if (taskItem.Frecuency == Frecuency.No_Weekends && (taskItem.DueDate.Value.DayOfWeek == DayOfWeek.Saturday || taskItem.DueDate.Value.DayOfWeek == DayOfWeek.Sunday))
                    throw new InvalidOperationException("The DueDate can't be on Saturdays or Sundays in No_Weekends frecuency");
                taskItem.PreviousDueDate = taskItem.DueDate;
            }
            else if(TasksItems[index].Frecuency != taskItem.Frecuency)
            {
                taskItem.DueDate = RecalculateFrecuency(taskItem);
            }
            if(taskItem.DueDate > maxDueDate || taskItem.DueDate < minDueDate)
                throw new InvalidOperationException($"DueDate must be between {minDueDate:yyyy-MM-dd} and {maxDueDate:yyyy-MM-dd}.");
            if(taskItem.MyDayDate < DateOnly.FromDateTime(DateTime.Today) || taskItem.MyDayDate > DateOnly.FromDateTime(DateTime.Today.AddDays(7)))
                throw new InvalidOperationException($"My Day Date must be between {DateOnly.FromDateTime(DateTime.Today):yyyy-MM-dd} and {DateOnly.FromDateTime(DateTime.Today.AddDays(7)):yyyy-MM-dd}.");

            foreach (TaskItem subTask in taskItem.SubTasks)
            {
                if(TasksItems[index].ParentTask != null)
                {
                    throw new InvalidOperationException("A subtask cannot have a subtask");
                }
                if (!TasksItems.Any(t => t.Id == subTask.Id))
                {
                    subTask.Id = idTaskItem++;
                    subTask.TaskListId = taskItem.TaskListId;
                    subTask.ParentTaskId = taskItem.Id;
                    subTask.ParentTask = taskItem;
                    subTask.Frecuency = taskItem.Frecuency;
                    subTask.MyDayDate = taskItem.MyDayDate;
                    subTask.CreatedTimeDate = DateTime.Today;
                    if (subTask.DueDate > taskItem.DueDate)
                    {
                        throw new InvalidOperationException("A subtask DueDate cannot be later than main task Due Date");

                    }
                    if (subTask.CreatedTimeDate < taskItem.CreatedTimeDate)
                        throw new InvalidOperationException("A subtask cannot be created before its parent task");
                    TasksItems.Add(subTask);
                }
                
            }

            TasksItems[index] = taskItem;

        }

        private DateTime? RecalculateFrecuency(TaskItem taskItem)
        {
            var today = DateTime.Today;
            if (taskItem.Frecuency == Frecuency.No_Repetition) return null;

            DateTime next = taskItem.Frecuency switch
            {
                
                Frecuency.Daily => taskItem.DueDate.Value.AddDays(1),
                Frecuency.Weekly => taskItem.DueDate.Value.AddDays(7),
                Frecuency.EveryTwoWeeks => taskItem.DueDate.Value.AddDays(14),
                Frecuency.Monthly => taskItem.DueDate.Value.AddMonths(1),
                Frecuency.Annually => taskItem.DueDate.Value.AddYears(1),
                Frecuency.No_Weekends => NextWeekDay(DateOnly.FromDateTime(taskItem.DueDate.Value)).ToDateTime(TimeOnly.FromDateTime(taskItem.DueDate.Value)),
                _ => throw new InvalidOperationException("Frecuency not supported")

            };


            return next;
        }

        /// <summary>
        /// Updates a TaskList
        /// It return null if the taskList doesn't exist
        /// </summary>
        /// <param name="taskList"></param>
        public  void UpdateTaskList(TaskList taskList)
        {
            var index = TasksLists.FindIndex(index => index.Id == taskList.Id);

            if (index == -1)
                return;

            
            
                
            TasksLists[index] = taskList;

        }
        /// <summary>
        /// Gets all task that are asigned to today, and returns it
        /// </summary>
        /// <returns></returns>
        public  List<TaskItem> GetAllDailyTask()
        {
            List<TaskItem> todayList = new List<TaskItem>();
            foreach (TaskItem task in TasksItems)
            {
                    CalculateMyDay(ref todayList, task);
             }
            

            return todayList;
        }
        /// <summary>
        /// Adds every task that is assigned to today
        /// Tasks that have MyDayDate on today,Dudate on Today or if the task is Overdue from other days
        /// </summary>
        /// <param name="todayList"></param>
        /// <param name="task"></param>
        private void CalculateMyDay(ref List<TaskItem> todayList, TaskItem task)
        {
            if (task.MyDayDate == DateOnly.FromDateTime(DateTime.Today))
            {
                todayList.Add(task);
                return;
            }


            if (task.DueDate.HasValue)
            {
                if (task.DueDate.Value.Date == DateTime.Today.Date)
                {
                    todayList.Add(task);
                    return;
                }
                    
                else if (task.DueDate.Value.Date < DateTime.Today.Date && task.CompletedAt != DateOnly.FromDateTime(task.DueDate.Value.Date))
                {
                    todayList.Add(task);
                    return;
                }
                    

            }
            
        }
        /// <summary>
        /// Gets all subtask from an especified task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public  List<TaskItem> GetAllSubTask(int id)
        {
            TaskItem taskItem = GetTaskItem(id);
            var subtaskList = new List<TaskItem>();
            if (taskItem == null)
                return null;
            if (taskItem.SubTasks != null)
                subtaskList = taskItem.SubTasks;
            return subtaskList;
        }
        /// <summary>
        /// Controls the completed status of a Function
        /// </summary>
        /// <param name="id"></param>
        /// <param name="IsNotCompleted"></param>
        /// <returns></returns>
        public  TaskItem UpdateCompletedStatus(int id,bool IsNotCompleted)
        {
            TaskItem taskItem = GetTaskItem(id);
            
            if (taskItem == null)
                return null;
            if (IsNotCompleted == true)
            {

              
              return TaskCompleted( taskItem);
            }
            else
            {
               return  TaskUncompleted( taskItem);
            }
            


        }
        /// <summary>
        /// Passes a task to completed, if the task is already completed it returns the same task.
        /// If not it changes it's CompletedAt status to today.
        /// If the task has a frecuency it calculates its next DueDate and PreviousDueDate
        /// </summary>
        /// <param name="taskItem"></param>
        /// <returns></returns>
        public  TaskItem TaskCompleted( TaskItem taskItem)
        {
            if (IsCompleted(taskItem))
                return taskItem;

            if (taskItem.Frecuency != 0)
            {
                // We take the last DueDate even when the task isOverDue
                taskItem.PreviousDueDate = taskItem.DueDate;

                do
                { 
                    switch (taskItem.Frecuency)
                    {
                        case Frecuency.Daily:
                            taskItem.DueDate = taskItem.DueDate.Value.AddDays(1);
                            break;
                        case Frecuency.Weekly:
                            taskItem.DueDate = taskItem.DueDate.Value.AddDays(7);
                            break;
                        case Frecuency.EveryTwoWeeks:
                            taskItem.DueDate = taskItem.DueDate.Value.AddDays(14);
                            break;
                        case Frecuency.Monthly:
                            taskItem.DueDate = taskItem.DueDate.Value.AddMonths(1);

                            break;
                        case Frecuency.Annually:
                            taskItem.DueDate = taskItem.DueDate.Value.AddYears(1);

                            break;
                        case Frecuency.No_Weekends:

                            taskItem.DueDate = DateOnly.FromDateTime(taskItem.DueDate.Value)
                                .ToDateTime(TimeOnly.FromDateTime(taskItem.DueDate.Value));
                            break;

                    } 
                } while (taskItem.DueDate.Value.Date <= DateTime.Today);

            }
            if (taskItem.IsTaskOverdue == true) { taskItem.IsTaskOverdue = false; }
            taskItem.CompletedAt = DateOnly.FromDateTime(DateTime.Today);

            TasksItems[TasksItems.FindIndex(t => t.Id == taskItem.Id)] = taskItem;

            return new TaskItem(taskItem.Id, taskItem.Title, taskItem.Description, taskItem.CreatedTimeDate, taskItem.IsTaskOverdue, 
                taskItem.DueDate, taskItem.PreviousDueDate, taskItem.MyDayDate, taskItem.Frecuency, taskItem.CompletedAt,
                taskItem.SubTasks,taskItem.TaskListId, taskItem.ParentTaskId);
        }

        
        /// <summary>
        /// It Uncompletes a task.
        /// If it was already uncompleted it returns the same task
        /// It ajust the DueDate to the previous states and recalculates if the task is Overdue or not.
        /// It changes the status of CompletetAt to null
        /// </summary>
        /// <param name="taskItem"></param>
        /// <returns></returns>
        public  TaskItem TaskUncompleted( TaskItem taskItem)
        {
            if (!IsCompleted(taskItem))
                return taskItem;

            if (taskItem.PreviousDueDate != null)
            {
                taskItem.DueDate = taskItem.PreviousDueDate;
            }

            if (taskItem.DueDate < DateTime.Today)
            {
                taskItem.IsTaskOverdue = true;
            }
            taskItem.CompletedAt = null;

            TasksItems[TasksItems.FindIndex(t => t.Id == taskItem.Id)] = taskItem;

            return new TaskItem(taskItem.Id, taskItem.Title, taskItem.Description,taskItem.CreatedTimeDate, taskItem.IsTaskOverdue,
                taskItem.DueDate, taskItem.PreviousDueDate, taskItem.MyDayDate, taskItem.Frecuency, taskItem.CompletedAt,
                taskItem.SubTasks, taskItem.TaskListId, taskItem.ParentTaskId); 
            
        }
        /// <summary>
        /// It calculates if a task is Completed or not.
        /// The special case, when the task has a frecuency higher than 0
        /// this function compares its next duedate to today if today is greater than next the task is not completed
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private bool IsCompleted(TaskItem t)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (t.CompletedAt == null) return false;
            if (t.Frecuency == Frecuency.No_Repetition) return true;

            var completedValue = t.CompletedAt.Value;

            var next = t.Frecuency switch
            {
                Frecuency.Daily => completedValue.AddDays(1),
                Frecuency.Weekly => completedValue.AddDays(7),
                Frecuency.EveryTwoWeeks => completedValue.AddDays(14),
                Frecuency.Monthly => completedValue.AddMonths(1),
                Frecuency.Annually => completedValue.AddYears(1),
                Frecuency.No_Weekends => NextWeekDay(completedValue),
                _ => throw new InvalidOperationException("Frecuency not supported")

            };


            return today < next;

        }

        private DateOnly NextWeekDay(DateOnly day)
        {
            do
            {
                day = day.AddDays(1);
            }
            while (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
            return day;
        }
    }
}
