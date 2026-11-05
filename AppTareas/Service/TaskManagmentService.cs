using AppTareas.Models;

namespace AppTareas.Service
{
    public class TaskManagmentService
    {
        static List<TaskItem> TasksItems { get; }
        static List<TaskList> TasksLists { get; }

        static int idTaskItem = 2;
        static int idTaskList = 1;
        

        static TaskManagmentService()
        {
            TasksItems = new List<TaskItem>
            {
                new TaskItem(){Id = 0,Title = "Task_1",CreatedTimeDate = DateTime.Today, Frecuency = Frecuency.No_Repetition,TaskListId = 0},
                new TaskItem(){Id = 1,Title = "Task_2",CreatedTimeDate = DateTime.Today, Frecuency = Frecuency.No_Repetition,TaskListId = 0}

            };
            TasksLists = new List<TaskList>
            {
                new TaskList (){Id = 0,Name="TaskList_1",Relevance=1,TasksItem = TasksItems,UserId = 0}
            };
        }

        public static List<TaskList> GetAllTasksLists() => TasksLists;

        public static TaskItem? GetTaskItem(int id) => TasksItems.FirstOrDefault(x => x.Id == id);

        public static TaskList? GetTaskList(int id) => TasksLists.FirstOrDefault(x => x.Id == id);

        public static void AddTaskList(TaskList taskList)
        {
            taskList.Id = idTaskList++;
            TasksLists.Add(taskList);
        }
        public static void AddTaskItem(TaskItem taskItem)
        {
            taskItem.Id = idTaskItem++;
            TasksItems.Add(taskItem);
        }

        public static void DeleteTaskItem(int id)
        {
            var taskItem = GetTaskItem(id);
            if (taskItem != null)
                return;
            TasksItems.Remove(taskItem);

        }
        public static void DeleteTaskList(int id)
        {
            var taskList = GetTaskList(id);
            if (taskList != null)
                return;
            TasksLists.Remove(taskList);

        }
        public static void UpdateTaskItem(TaskItem taskItem)
        {
            var index = TasksItems.FindIndex(index => index.Id == taskItem.Id);

            if (index == -1)
                return;

            TasksItems[index] = taskItem;

        }
        public static void UpdateTaskList(TaskList taskList)
        {
            var index = TasksLists.FindIndex(index => index.Id == taskList.Id);

            if (index == -1)
                return;

            TasksLists[index] = taskList;

        }

        public static List<TaskItem> GetAllDailyTask()
        {
            var list = GetAllTasksLists();
            List<TaskItem> todayList = new List<TaskItem>();
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            foreach(TaskList taskList in list)
            {
                foreach(TaskItem task in taskList.TasksItem)
                {
                    CalculateMyDay(ref todayList, task, currentDate);
                }
            }

            return todayList;
        }
        private static void CalculateMyDay(ref List<TaskItem> todayList, TaskItem task,DateOnly currentDate)
        {
            if (task.MyDayDate == currentDate)
                todayList.Add(task);
            if (task.Frecuency == Frecuency.Daily)
                todayList.Add(task);


            if (task.DueDate.HasValue)
            {
                var dueDate = DateOnly.FromDateTime(task.DueDate.Value);
                if (dueDate == currentDate)
                    todayList.Add(task);
                else if (dueDate < currentDate && task.CompletedAt != dueDate)
                    todayList.Add(task);

            }
            if (task.SubTasks.Count > 0)
            {
                foreach(TaskItem subTask in task.SubTasks)
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

    }
}
