using Microsoft.OpenApi;
using AppTareas.Models;

namespace AppTareas.DTos
{
    public record CreateTaskList(string Name,int Relevance);
    public record TaskListResponse(int Id,string Name,int Relevance, List<TaskItem> TaskItems);
    public record UpdateTaskList(string Name,int Relevance,List<TaskItem> TaskItems);
}
