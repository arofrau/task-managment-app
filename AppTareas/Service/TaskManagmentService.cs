using AppTareas.Models;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace AppTareas.Service
{
    public class TaskManagmentService
    {
        private readonly List<TaskItem> TasksItems = new();
        private readonly List<TaskList> TasksLists = new();

        private  int idTaskItem = 3;
        private  int idTaskList = 2;


        public TaskManagmentService()
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

        public  List<TaskList> GetAllTasksLists() => TasksLists;

        public  TaskItem? GetTaskItem(int id)
        {
            var taskItem = TasksItems.FirstOrDefault(x => x.Id == id);
            if (taskItem == null)
                return null;


            var subTasks = TasksItems.Where(t => t.ParentTaskId == id).ToList();

            return new TaskItem(taskItem.Id, taskItem.Title,taskItem.Description, 
                taskItem.IsTaskOverdue,taskItem.DueDate, taskItem.PreviousDueDate, taskItem.MyDayDate,
                taskItem.Frecuency, taskItem.CompletedAt, subTasks, taskItem.TaskListId);
        }


        public  TaskList? GetTaskList(int id)
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
            taskItem.Id = idTaskItem++;

            taskItem.IsTaskOverdue = DateTime.Today > taskItem.DueDate;
            taskItem.CompletedAt = null;
            taskItem.CreatedTimeDate = DateTime.Today;
            
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

        public  void DeleteTaskItem(int id)
        {
            var taskItem = GetTaskItem(id);
            if (taskItem == null)
                return;
            TasksItems.RemoveAll(t => t.Id == id || t.ParentTaskId == id);

        }
        public  void DeleteTaskList(int id)
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
        public  void UpdateTaskItem(TaskItem taskItem)
        {
            var index = TasksItems.FindIndex(index => index.Id == taskItem.Id);

            if (index == -1)
                return;
            foreach (TaskItem subTask in taskItem.SubTasks)
            {

                if (!TasksItems.Any(t => t.Id == subTask.Id))
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
                
            }

            TasksItems[index] = taskItem;

        }
        public  void UpdateTaskList(TaskList taskList)
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

        public  List<TaskItem> GetAllDailyTask()
        {
            List<TaskItem> todayList = new List<TaskItem>();
            foreach (TaskItem task in TasksItems)
            {
                    CalculateMyDay(ref todayList, task);
             }
            

            return todayList;
        }
        private void CalculateMyDay(ref List<TaskItem> todayList, TaskItem task)
        {
            if (task.MyDayDate == DateOnly.FromDateTime(DateTime.Today))
            {
                todayList.Add(task);
                return;
            }
            if (task.CompletedAt == DateOnly.FromDateTime(DateTime.Today))
            {
                todayList.Add(task);
                return;

            }

            if (task.DueDate.HasValue)
            {
                var dueDate = DateOnly.FromDateTime(task.DueDate.Value);
                if (dueDate == DateOnly.FromDateTime(DateTime.Today))
                {
                    todayList.Add(task);
                    return;
                }
                    
                else if (dueDate < DateOnly.FromDateTime(DateTime.Today) && task.CompletedAt != dueDate)
                {
                    todayList.Add(task);
                    return;
                }
                    

            }
            if (task.SubTasks.Count > 0)
            {
                foreach (TaskItem subTask in task.SubTasks)
                    CalculateMyDay(ref todayList, subTask);
            }
        }

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
        public  TaskItem TaskCompleted( TaskItem taskItem)
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
            taskItem.CompletedAt = DateOnly.FromDateTime(DateTime.Today);

            TasksItems.Where(t => t.Id == taskItem.Id).First().CompletedAt = DateOnly.FromDateTime(DateTime.Today);

            return new TaskItem(taskItem.Id, taskItem.Title, taskItem.Description, taskItem.IsTaskOverdue, 
                taskItem.DueDate, taskItem.PreviousDueDate, taskItem.MyDayDate, taskItem.Frecuency, taskItem.CompletedAt,
                taskItem.SubTasks,taskItem.TaskListId);
        }
        public  TaskItem TaskUncompleted( TaskItem taskItem)
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

            TasksItems.Where(t => t.Id == taskItem.Id).First().CompletedAt = null;

            return new TaskItem(taskItem.Id, taskItem.Title, taskItem.Description, taskItem.IsTaskOverdue,
                taskItem.DueDate, taskItem.PreviousDueDate, taskItem.MyDayDate, taskItem.Frecuency, taskItem.CompletedAt,
                taskItem.SubTasks, taskItem.TaskListId); 
            
        }
    }
}
