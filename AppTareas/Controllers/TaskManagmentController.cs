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
        private readonly TaskManagmentService _taskManagerService;
        public TaskManagmentController(TaskManagmentService taskManagerService) 
        {
            _taskManagerService = taskManagerService;
        }

        // TaskList

        // Get all TaskList
        [HttpGet("/api/liststasks")]
        public ActionResult<List<TaskListSumaryResponse>> GetAllTaskList()
        {
            var taskslists = _taskManagerService.GetAllTasksLists();
            
            return taskslists.Adapt<List<TaskListSumaryResponse>>();
        }

        // Get specific TaskList with its taskItems

        [HttpGet("/api/liststasks/{id}")]
        public ActionResult<TaskListResponse> GetTaskList(int id)
        {
           var taskList = _taskManagerService.GetTaskList(id);
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
            _taskManagerService.AddTaskList(taskList);

            var response = new TaskListResponse(taskList.Id, taskList.Name, taskList.Relevance, new List<TaskItemResponse>());
                
            return CreatedAtAction(nameof(GetTaskList), new { id = taskList.Id }, response);
        }
        // Put update an existing TaskList
        [HttpPut("/api/liststasks/{id}")]
        public IActionResult UpdateTaskList(int id,UpdateTaskList dto)
        {
            //-> TO DO. Add 403 Forbidden

            var existingTaskList = _taskManagerService.GetTaskList(id);
            if (existingTaskList == null)
                return NotFound();
            dto.Adapt(existingTaskList);
            
            _taskManagerService.UpdateTaskList(existingTaskList);

            

            return NoContent();

        }
        // Delete a specific TaskList
        [HttpDelete("/api/liststasks/{id}")]
        public IActionResult DeleteTaskList(int id)
        {
            //-> TO DO. Add 403 Forbidden
            var taskList = _taskManagerService.GetTaskList(id);
            if (taskList == null)
                return NotFound();

            _taskManagerService.DeleteTaskList(id);

            return NoContent();
        }

        //TaskItem
        // Get specific TaskItem
        [HttpGet("/api/tasks/{id}")]
        public ActionResult<TaskItemResponse> GetTaskItem(int id)
        {

            var taskItem = _taskManagerService.GetTaskItem(id);
            if (taskItem == null)
                return NotFound();
            
            return taskItem.Adapt<TaskItemResponse>();
        }
        // Post create a TaskItem
        [HttpPost("/api/tasks")]
        public IActionResult CreateTaskItem(CreateTaskItemDto dto)
        {
            var taskItem = dto.Adapt<TaskItem>();
            
            try
            {
                _taskManagerService.AddTaskItem(taskItem);
            }
            catch(InvalidOperationException ex)
            {
                return UnprocessableEntity(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return UnprocessableEntity(new { error = ex.Message });
            }
            var response = taskItem.Adapt<TaskItemResponse>();
            return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, response);
        }
        // Put Update specific TaskItem
        [HttpPut("/api/tasks/{id}")]
        public IActionResult UpdateTaskItem(int id,  UpdateTaskItem dto)
        {
            //-> TO DO. Add 403 Forbidden
            var taskItem = _taskManagerService.GetTaskItem(id);
            
            if (taskItem == null)
                return NotFound();

            
            dto.Adapt(taskItem);

            try
            {
                _taskManagerService.UpdateTaskItem(taskItem);

            }
            catch (InvalidOperationException ex)
            {
                return UnprocessableEntity(new { error = ex.Message });

            }

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
            _taskManagerService.DeleteTaskItem(id);

            return NoContent();
        }
        // Get all TaskItem that are due today
        [HttpGet("/api/myday")]
        public ActionResult<List<TaskItemResponse>> GetMyDayTask()
        {
            var allDailyTask = _taskManagerService.GetAllDailyTask();

            return allDailyTask.Adapt<List<TaskItemResponse>>();
        }
        // Get all subtasks
        [HttpGet("/api/tasks/{id}/subtasks")]
        public ActionResult<List<TaskItemResponse>> GetAllSubTask(int id)
        {
            var subtask = _taskManagerService.GetAllSubTask(id);
            if (subtask == null)
                return NotFound();
            
            return subtask.Adapt<List<TaskItemResponse>>();

        }

        // Put change a task  to completition
        [HttpPut("/api/tasks/{id}/complete")]
        public IActionResult UpdateTaskToCompleted(int id)
        {
            var taskItem = _taskManagerService.UpdateCompletedStatus(id,true);
            if (taskItem == null)
                return NotFound();
            
            return NoContent();
        }
        // Put change a task to uncompleted
        [HttpPut("/api/tasks/{id}/uncomplete")]
        public IActionResult UpdateTaskToUncompleted(int id)
        {
            var taskItem = _taskManagerService.UpdateCompletedStatus(id, false);
            if (taskItem == null)
                return NotFound();

            return NoContent();
        }



    }
}
