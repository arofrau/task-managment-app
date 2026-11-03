namespace AppTareas.Models
{
    public class User
    {
        /// <summary>
        /// ID of a specific user
        /// </summary>
        public int Id { get; set;  }
        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Password from the user stored using hascode
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// List of tasks from the user.
        /// </summary>
        public List<TaskList> Taskslist { get; set; } = new List<TaskList>();


    }
}
