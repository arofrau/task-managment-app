using AppTareas.Models;

namespace AppTareas.DTos
{

    public record TaskItemDto(
        int Id, string Title, string? Description, DateTime CreatedTimeDate,
        DateTime? DueDate, DateTime? PreviousDueDate, DateOnly? MyDayDate, Frecuency Frecuency,
        DateOnly? CompletedAt, bool IsTaskOverDue,
        List<CreateSubTaskItemDto>? SubTasks,
        int TaskListId, int? ParentTaskId);   

    public record TaskItemResponse(
        int Id, string Title, string? Description, DateTime CreatedTimeDate,
        DateTime? DueDate, DateTime? PreviousDueDate, DateOnly? MyDayDate, Frecuency Frecuency,
        DateOnly? CompletedAt, bool IsTaskOverdue,
        List<TaskItemResponse>? SubTasks,
        int TaskListId);

    public record UpdateTaskItem(
        string Title, string? Description, DateTime? DueDate,
        DateTime? PreviousDueDate, DateOnly? MyDayDate, Frecuency Frecuency,
        DateOnly? CompletedAt, bool IsTaskOverdue,
        List<CreateSubTaskItemDto>? SubTasks);
    public record CreateTaskItemDto(string Title, string? Description,
        DateTime? DueDate, DateOnly? MyDayDate, Frecuency Frecuency,
        int TaskListId,int? ParentTaskId = null);
    public record CreateSubTaskItemDto(string Title, string? Description,
        DateTime? DueDate);
}
