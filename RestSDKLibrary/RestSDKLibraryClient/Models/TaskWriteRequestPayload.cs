﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace RestSDKLibrary.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class TaskWriteRequestPayload
    {
        /// <summary>
        /// Initializes a new instance of the TaskWriteRequestPayload class.
        /// </summary>
        public TaskWriteRequestPayload() { }

        /// <summary>
        /// Initializes a new instance of the TaskWriteRequestPayload class.
        /// </summary>
        public TaskWriteRequestPayload(string taskName, bool isCompleted, string dueDate)
        {
            TaskName = taskName;
            IsCompleted = isCompleted;
            DueDate = dueDate;
        }

        /// <summary>
        /// name of the task
        /// </summary>
        [JsonProperty(PropertyName = "taskName")]
        public string TaskName { get; set; }

        /// <summary>
        /// true if completed, false if not completed
        /// </summary>
        [JsonProperty(PropertyName = "isCompleted")]
        public bool? IsCompleted { get; set; }

        /// <summary>
        /// The date the task is due to be completed
        /// 
        /// Use standard ISO 8601 format: "2012-04-23"
        /// </summary>
        [JsonProperty(PropertyName = "dueDate")]
        public string DueDate { get; set; }
    }
}
