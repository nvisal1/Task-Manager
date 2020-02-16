using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.DataTransferObjects
{
    public class ErrorResponse
    {
        /// <summary>
        /// Numeric error that represents the issue
        /// </summary>
        public int errorNumber { get; set; }

        /// <summary>
        /// The name of the parameter that has the issue
        /// 
        /// If the error is not tied to a specified parameter,
        /// then this value can be null
        /// </summary>
        public string parameterName { get; set; }

        /// <summary>
        /// The value of the parameter that caused the error
        /// 
        /// If the error is not tied to a specific parameter,
        /// then this value can be null
        /// </summary>
        public string parameterValue { get; set; }

        /// <summary>
        /// A description of the error, not localized,
        /// intended for developer consumption
        /// </summary>
        public string errorDescription { get; set; }
    }
}
