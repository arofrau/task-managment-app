using Microsoft.OpenApi;
using AppTareas.Models;

namespace AppTareas.DTos
{
    public record CreateTaskList(string Name,int Relevance);
    public record TaskListSumaryResponse(int Id, string Name, int Relevance);
    public record TaskListResponse(int Id,string Name,int Relevance, List<TaskItemResponse> TasksItem);
    public record UpdateTaskList(string Name,int Relevance,List<CreateTaskItemDto>? TasksItem = null);
}
