using AppTareas.Models;
using AppTareas.Service;
using AppTareas.DTos;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.WebSockets;
using Mapster;


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
        public ActionResult<List<TaskListResponse>> GetAllTaskList()
        {
            var taskslists = TaskManagmentService.GetAllTasksLists();
            
            return taskslists.Adapt<List<TaskListResponse>>();
        }

        // Get specific TaskList with its taskItems

        [HttpGet("/api/liststasks/{id}")]
        public ActionResult<TaskListResponse> GetTaskList(int id)
        {
           var taskList = TaskManagmentService.GetTaskList(id);
            if (taskList == null)
                return NotFound();
            return taskList.Adapt<TaskListResponse>();
        }



        // Post create TaskList
        [HttpPost("/api/liststasks")]
        public IActionResult CreateTaskList(CreateTaskList taskListDto)
        {
            var taskList = new TaskList
            {
                Name = taskListDto.Name,
                Relevance = taskListDto.Relevance
            };
            TaskManagmentService.AddTaskList(taskList);

            var response = new TaskListResponse(taskList.Id, taskList.Name, taskList.Relevance, new List<TaskItem>());
                
            return CreatedAtAction(nameof(GetTaskList), new { id = taskList.Id }, response);
        }
        // Put update an existing TaskList
        [HttpPut("/api/liststasks/{id}")]
        public IActionResult UpdateTaskList(int id,UpdateTaskList dto)
        {
            //-> TO DO. Add 403 Forbidden

            var existingTaskList = TaskManagmentService.GetTaskList(id);
            if (existingTaskList == null)
                return NotFound();
            dto.Adapt(existingTaskList);

            TaskManagmentService.UpdateTaskList(existingTaskList);

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
        public ActionResult<TaskItemResponse> GetTaskItem(int id)
        {

            var taskItem = TaskManagmentService.GetTaskItem(id);
            if (taskItem == null)
                return NotFound();
            
            return taskItem.Adapt<TaskItemResponse>();
        }
        // Post create a TaskItem
        [HttpPost("/api/tasks")]
        public IActionResult CreateTaskItem(CreateTaskItem dto)
        {
            var taskItem = dto.Adapt<TaskItem>();
            TaskManagmentService.AddTaskItem(taskItem);
            var response = taskItem.Adapt<TaskItemResponse>();
            return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, response);
        }
        // Put Update specific TaskItem
        [HttpPut("/api/tasks/{id}")]
        public IActionResult UpdateTaskItem(int id,  UpdateTaskItem dto)
        {
            //-> TO DO. Add 403 Forbidden
            var taskItem = TaskManagmentService.GetTaskItem(id);
            
            if (taskItem == null)
                return NotFound();
            dto.Adapt(taskItem);

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
        public ActionResult<List<TaskItemResponse>> GetMyDayTask()
        {
            var allDailyTask = TaskManagmentService.GetAllDailyTask();

            return allDailyTask.Adapt<List<TaskItemResponse>>();
        }
        // Get all subtasks
        [HttpGet("/api/tasks/{id}/subtasks")]
        public ActionResult<List<TaskItemResponse>> GetAllSubTask(int id)
        {
            var subtask = GetAllSubTask(id);
            if (subtask == null)
                return NotFound();
            
            return subtask.Adapt<List<TaskItemResponse>>();

        }

        // Put change a task  to completition
        //[HttpPut("/api/tasks/{id}/complete")]
        // Put change a task to uncompleted
        //[HttpPut("/api/tasks/{id}/uncomplete")]



    }
}
