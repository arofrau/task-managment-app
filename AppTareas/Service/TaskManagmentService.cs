using AppTareas.Models;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace AppTareas.Service
{
    public class TaskManagmentService
    {
        static List<TaskItem> TasksItems { get; }
        static List<TaskList> TasksLists { get; }

        static int idTaskItem = 3;
        static int idTaskList = 2;
        private static DateOnly today = DateOnly.FromDateTime(DateTime.Today.Date);

        static TaskManagmentService()
        {
            TasksItems = new List<TaskItem>
            {
                new TaskItem(){Id = 1,Title = "Task_1",CreatedTimeDate = DateTime.Today, Frecuency = Frecuency.No_Repetition,TaskListId = 1},
                new TaskItem(){Id = 2,Title = "Task_2",CreatedTimeDate = DateTime.Today, Frecuency = Frecuency.No_Repetition,TaskListId = 1}

            };
            TasksLists = new List<TaskList>
            {
                new TaskList (){Id = 1,Name="TaskList_1",Relevance=1,TasksItem = TasksItems.Where(t => t.TaskListId == 1).ToList(),UserId = 0}
            };
        }

        public static List<TaskList> GetAllTasksLists() => TasksLists;

        public static TaskItem? GetTaskItem(int id)
        {
            var taskItem = TasksItems.FirstOrDefault(x => x.Id == id);
            
            var items = TasksItems.Where(t => t.ParentTaskId == id).ToList();

            return new TaskItem(taskItem.Id, taskItem.Title,taskItem.Description, 
                taskItem.IsTaskOverdue,taskItem.DueDate, taskItem.PreviousDueDate, taskItem.MyDayDate,
                taskItem.Frecuency, taskItem.CompletedAt,items);
        }


        public static TaskList? GetTaskList(int id)
        {
            var list = TasksLists.FirstOrDefault(l => l.Id == id);
            if (list is null) return null;

            var TasksItem = TasksItems.Where(t => t.TaskListId == id && t.ParentTaskId == null).ToList();

            return new TaskList(list.Id,list.Name,list.Relevance, TasksItem);

        }

        public static void AddTaskList(TaskList taskList)
        {
            taskList.Id = idTaskList++;
            TasksLists.Add(taskList);
        }
        /// <summary>
        ///  Adds a TaskItem to a TaskList, and the TasksItems List.
        /// </summary>
        /// <param name="taskItem"></param> the taskItem we want to add, it can come with a taskList, taskListId or ParentTaskId
        public static void AddTaskItem(TaskItem taskItem)
        {
            taskItem.Id = idTaskItem++;

            taskItem.IsTaskOverdue = DateTime.Today > taskItem.DueDate;
            taskItem.CompletedAt = null;
            
            if(taskItem.ParentTaskId.HasValue && taskItem.ParentTaskId !=0)
            {
               TaskItem taskItemParent = GetTaskItem(taskItem.ParentTaskId.Value);
               taskItemParent.SubTasks.Add(taskItem);

               taskItem.TaskListId = taskItemParent.TaskListId;
                
            }
            else if (taskItem.TaskListId == 0 && taskItem.TaskList != null)
                taskItem.TaskListId = taskItem.TaskList.Id;

            

            TasksItems.Add(taskItem);
        }

        public static void DeleteTaskItem(int id)
        {
            var taskItem = GetTaskItem(id);
            if (taskItem == null)
                return;
            TasksItems.Remove(taskItem);

        }
        public static void DeleteTaskList(int id)
        {
            var taskList = GetTaskList(id);
            if (taskList == null)
                return;
            TasksLists.Remove(taskList);

        }
        public static void UpdateTaskItem(TaskItem taskItem)
        {
            var index = TasksItems.FindIndex(index => index.Id == taskItem.Id);

            if (index == -1)
                return;
            foreach (TaskItem subTask in taskItem.SubTasks)
            {
                subTask.Id = idTaskItem++;
                subTask.TaskListId = taskItem.TaskListId;
                subTask.Frecuency = taskItem.Frecuency;
                subTask.MyDayDate = taskItem.MyDayDate;
                subTask.CreatedTimeDate = DateTime.Today;
                if (subTask.DueDate > taskItem.DueDate)
                {
                    // -> To Do - Add an exception when the DueDate of a SubTask is later than the main Task.
                    subTask.DueDate = taskItem.DueDate;
                }
                
            }

            TasksItems[index] = taskItem;

        }
        public static void UpdateTaskList(TaskList taskList)
        {
            var index = TasksLists.FindIndex(index => index.Id == taskList.Id);

            if (index == -1)
                return;

            foreach(TaskItem taskItem in taskList.TasksItem)
            {
                if(!TasksItems.Any(x => x.Id == taskItem.Id))
                {
                    taskItem.TaskList = taskList;
                    AddTaskItem(taskItem);
                    return;
                }
                else
                {
                    UpdateTaskItem(taskItem);
                    return;
                }
            }
            
                
            TasksLists[index] = taskList;

        }

        public static List<TaskItem> GetAllDailyTask()
        {
            var list = GetAllTasksLists();
            List<TaskItem> todayList = new List<TaskItem>();
            var currentDate = today;
            foreach (TaskList taskList in list)
            {
                foreach (TaskItem task in taskList.TasksItem)
                {
                    CalculateMyDay(ref todayList, task, currentDate);
                }
            }

            return todayList;
        }
        private static void CalculateMyDay(ref List<TaskItem> todayList, TaskItem task, DateOnly currentDate)
        {
            if (task.MyDayDate == currentDate)
            {
                todayList.Add(task);
                return;
            }
            if (task.CompletedAt == currentDate)
            {
                todayList.Add(task);
                return;

            }

            if (task.DueDate.HasValue)
            {
                var dueDate = DateOnly.FromDateTime(task.DueDate.Value);
                if (dueDate == currentDate)
                {
                    todayList.Add(task);
                    return;
                }
                    
                else if (dueDate < currentDate && task.CompletedAt != dueDate)
                {
                    todayList.Add(task);
                    return;
                }
                    

            }
            if (task.SubTasks.Count > 0)
            {
                foreach (TaskItem subTask in task.SubTasks)
                    CalculateMyDay(ref todayList, subTask, currentDate);
            }
        }

        public static List<TaskItem> GetAllSubTask(int id)
        {
            TaskItem taskItem = GetTaskItem(id);
            var subtaskList = new List<TaskItem>();
            if (taskItem == null)
                return null;
            if (taskItem.SubTasks != null)
                subtaskList = taskItem.SubTasks;
            return subtaskList;
        }

        public static TaskItem UpdateCompletedStatus(int id,bool IsNotCompleted)
        {
            TaskItem taskItem = GetTaskItem(id);
            if (taskItem == null)
                return null;
            if (IsNotCompleted == true)
            {
                TaskCompleted(ref taskItem);
            }
            else
            {
                TaskUncompleted(ref taskItem);
            }
            return taskItem;
        }
        public static void TaskCompleted(ref TaskItem taskItem)
        {

            if (taskItem.Frecuency != 0)
            {
                // We take the last DueDate even when the task isOverDue
                taskItem.PreviousDueDate = taskItem.DueDate;

                while (taskItem.DueDate <= DateTime.Today)
                {
                    switch (taskItem.Frecuency)
                    {
                        case Frecuency.Daily:
                            taskItem.DueDate.Value.AddDays(1);
                            break;
                        case Frecuency.Weekly:
                            taskItem.DueDate.Value.AddDays(7);
                            break;
                        case Frecuency.EveryTwoWeeks:
                            taskItem.DueDate.Value.AddDays(14);
                            break;
                        case Frecuency.Monthly:
                            taskItem.DueDate.Value.AddMonths(1);

                            break;
                        case Frecuency.Annually:
                            taskItem.DueDate.Value.AddYears(1);

                            break;
                        case Frecuency.No_Weekends:
                            var dayWeek = taskItem.DueDate.Value.DayOfWeek;
                            if (dayWeek == System.DayOfWeek.Friday)
                                taskItem.DueDate.Value.AddDays(3);
                            else taskItem.DueDate.Value.AddDays(1);
                            break;

                    }
                }

            }
            if (taskItem.IsTaskOverdue == true) { taskItem.IsTaskOverdue = false; }
            taskItem.CompletedAt = today;
        }
        public static void TaskUncompleted(ref TaskItem taskItem)
        {

            if (taskItem.PreviousDueDate != null)
            {
                taskItem.DueDate = taskItem.PreviousDueDate;
            }

            if (taskItem.DueDate < DateTime.Today)
            {
                taskItem.IsTaskOverdue = true;
            }
            taskItem.CompletedAt = null;
            
        }
    }
}
