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

    public partial class TaskResponse
    {
        /// <summary>
        /// Initializes a new instance of the TaskResponse class.
        /// </summary>
        public TaskResponse() { }

        /// <summary>
        /// Initializes a new instance of the TaskResponse class.
        /// </summary>
        public TaskResponse(int? id = default(int?), string taskName = default(string), bool? isCompleted = default(bool?), string dueDate = default(string))
        {
            Id = id;
            TaskName = taskName;
            IsCompleted = isCompleted;
            DueDate = dueDate;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taskName")]
        public string TaskName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isCompleted")]
        public bool? IsCompleted { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "dueDate")]
        public string DueDate { get; set; }

    }
}
