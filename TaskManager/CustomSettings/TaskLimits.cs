namespace TaskManager.CustomSettings
{
    /// <summary>
    /// This class contains information about
    /// task storage limits
    /// </summary>
    public class TaskLimits
    {
        /// <summary>
        /// The max number of tasks that can be saved
        /// 
        /// Default value is 100
        /// </summary>
        public int MaxTaskEntries { get; set; } = 100;
    }
}
