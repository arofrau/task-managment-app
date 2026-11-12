namespace AppTareas.Models
{
    public class TaskList
    {


        public TaskList()
        {
        }

        public TaskList(int id, string name, int relevance, List<TaskItem> items)
        {
            Id = id;
            Name = name;
            Relevance = relevance;
            this.TasksItem = items;
        }

        /// <summary>
        /// Id from an specific TasksList
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the taskslist
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Variable that stores the importances of a taskslist, 
        /// 1-more impotant to less important.
        /// </summary>
        public int Relevance { get; set; }
        /// <summary>
        /// List of the task in the TasksList
        /// </summary>
        public List<TaskItem> TasksItem { get; set; } = new List<TaskItem>();


        
        /// <summary>
        /// Foreign Key of User
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// User of this tasklist
        /// </summary>
        public User? User { get; set; }



    }
}
