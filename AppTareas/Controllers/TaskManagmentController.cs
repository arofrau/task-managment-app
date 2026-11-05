using AppTareas.Models;
using AppTareas.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace AppTareas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskManagmentController : ControllerBase
    {
        public TaskManagmentController() { }

        // TaskList

        // Get all TaskList
        [HttpGet("/api/liststasks")] 
        public ActionResult<List<TaskList>> GetAllTaskList() => TaskManagmentService.GetAllTasksLists();

        // Get specific TaskList with its taskItems

        [HttpGet("/api/liststasks/{id}")]
        public ActionResult<TaskList> GetTaskList(int id)
        {
           var taskList = TaskManagmentService.GetTaskList(id);
            if (taskList == null)
                return NotFound();
            return taskList;
        }



        // Post create TaskList
        [HttpPost("/api/liststasks")]
        public IActionResult CreateTaskList(TaskList taskList)
        {
            TaskManagmentService.AddTaskList(taskList);
            return CreatedAtAction(nameof(GetTaskList), new { id = taskList.Id }, taskList);
        }
        // Put update an existing TaskList
        [HttpPut("/api/liststasks/{id}")]
        public IActionResult UpdateTaskList(int id,TaskList taskList)
        {
            //-> TO DO. Add 403 Forbidden
            if(id != taskList.Id)
                return BadRequest();
            var existingTaskList = TaskManagmentService.GetTaskList(id);
            if (existingTaskList == null)
                return NotFound();

            TaskManagmentService.UpdateTaskList(taskList);

            return NoContent();

        }
        // Delete a specific TaskList
        [HttpDelete("/api/liststasks/{id}")]
        public IActionResult DeleteTaskList(int id)
        {
            //-> TO DO. Add 403 Forbidden
            var taskList = TaskManagmentService.GetTaskList(id);
            if (taskList == null)
                return NotFound();

            TaskManagmentService.DeleteTaskList(id);

            return NoContent();
        }

        //TaskItem
        // Get specific TaskItem
        [HttpGet("/api/tasks/{id}")]
        public ActionResult<TaskItem> GetTaskItem(int id)
        {

            var taskItem = TaskManagmentService.GetTaskItem(id);
            if (taskItem == null)
                return NotFound();

            return taskItem;
        }
        // Post create a TaskItem
        [HttpPost("/api/tasks")]
        public IActionResult CreateTaskItem(TaskItem taskItem)
        {
            TaskManagmentService.AddTaskItem(taskItem);
            return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
        }
        // Put Update specific TaskItem
        [HttpPut("/api/tasks/{id}")]
        public IActionResult UpdateTaskItem(int id, TaskItem taskItem)
        {
            //-> TO DO. Add 403 Forbidden

            if (taskItem.Id != id)
                return BadRequest();
            if (taskItem == null)
                return NotFound();

            TaskManagmentService.UpdateTaskItem(taskItem);

            return NoContent();
        }
        // Delete Specific TaskItem
        [HttpDelete("/api/tasks/{id}")]
        public IActionResult DeleteTaskItem(int id)
        {
            //-> TO DO. Add 403 Forbidden

            var taskItem = GetTaskItem(id);
            if (taskItem == null)
                return NotFound();
            TaskManagmentService.DeleteTaskItem(id);

            return NoContent();
        }
        // Get all TaskItem that are due today
        [HttpGet("/api/myday")]
        public ActionResult<List<TaskItem>> GetMyDayTask() => TaskManagmentService.GetAllDailyTask();
        // Get all subtasks
        [HttpGet("/api/tasks/{id}/subtasks")]
        public ActionResult<List<TaskItem>> GetAllSubTask(int id)
        {
            var subtask = GetAllSubTask(id);
            if (subtask == null)
                return NotFound();

            return subtask;

        }

        // Put change a task  to completition
        //[HttpPut("/api/tasks/{id}/complete")]
        // Put change a task to uncompleted
        //[HttpPut("/api/tasks/{id}/uncomplete")]



    }
}
