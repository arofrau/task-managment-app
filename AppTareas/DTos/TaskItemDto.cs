using AppTareas.Models;

namespace AppTareas.DTos
{
    
        public record TaskItemResponse(int Id,string Title,string? Description,DateTime CreatedTimeDate,
            DateTime? DueDate,DateTime? PreviousDueDate,DateOnly? MyDayDate,Frecuency Frecuency,
            DateOnly? CompletedAt,bool IsTaskOverDue,TaskItemDto subTaskItemDto);
        public record CreateTaskItem(string Title,string? Description,DateTime? DueDate,DateOnly? MyDayDate,Frecuency Frecuency);
        public record UpdateTaskItem(string Title,string? Description,DateTime? DueDate, 
            DateTime? PreviousDueDate, DateOnly? MyDayDate, Frecuency Frecuency, DateOnly? CompletedAt, 
            bool IsTaskOverDue, TaskItemDto subTaskItemDto);
    public record TaskItemDto(int Id, string Title, string? Description, DateTime CreatedTimeDate,
            DateTime? DueDate, DateTime? PreviousDueDate, DateOnly? MyDayDate, Frecuency Frecuency,
            DateOnly? CompletedAt, bool IsTaskOverDue, TaskItemDto subTaskItemDto);
}
