using System.ComponentModel.DataAnnotations;
using TaskManager.Constants;

namespace TaskManager.DataTransferObjects
{
    public class ErrorResponse
    {
        /// <summary>
        /// Numeric error that represents the issue
        /// </summary>
        public int ErrorNumber { get; set; }

        /// <summary>
        /// The name of the parameter that has the issue
        /// 
        /// If the error is not tied to a specified parameter,
        /// then this value can be null
        /// </summary>
        [MaxLength(1024)]
        public string ParameterName { get; set; }

        /// <summary>
        /// The value of the parameter that caused the error
        /// 
        /// If the error is not tied to a specific parameter,
        /// then this value can be null
        /// </summary>
        [MaxLength(2048)]
        public string ParameterValue { get; set; }

        /// <summary>
        /// A description of the error, not localized,
        /// intended for developer consumption
        /// </summary>
        [MaxLength(1024)]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// This function is used to determine the error response number
        /// when given a model state error
        /// </summary>
        /// <param name="encodedErrorDescription"></param>
        /// <returns></returns>
        public static int GetErrorNumberFromDescription(string encodedErrorDescription)
        {
            if (encodedErrorDescription.Contains("field is required")) return ErrorNumbers.IS_REQUIRED;
            else if (encodedErrorDescription.Contains("maximum length")) return ErrorNumbers.TOO_LARGE;
            else if (encodedErrorDescription.Contains("JSON value could not be converted")) return ErrorNumbers.NOT_VALID;
            else if (encodedErrorDescription.Contains("minimum length")) return ErrorNumbers.TOO_SMALL;

            return 0;
        }

        /// <summary>
        /// This function is used to determine the error response message
        /// when given a model state error
        /// </summary>
        /// <param name="encodedErrorDescription"></param>
        /// <returns></returns>
        public static (string decodedErrorMessage, int decodedErrorNumber) GetErrorMessage(string encodedErrorDescription)
        {
            int errorNumber = GetErrorNumberFromDescription(encodedErrorDescription);

            switch(errorNumber)
            {
                case 1:
                    {
                        return (ErrorMessages.ALREADY_EXISTS, errorNumber);
                    }
                case 2:
                    {
                        return (ErrorMessages.TOO_LARGE, errorNumber);
                    }
                case 3:
                    {
                        return (ErrorMessages.IS_REQUIRED, errorNumber);
                    }
                case 4:
                    {
                        return (ErrorMessages.AT_CAPACITY, errorNumber);
                    }
                case 5:
                    {
                        return (ErrorMessages.NOT_FOUND, errorNumber);
                    }
                case 6:
                    {
                        return (ErrorMessages.TOO_SMALL, errorNumber);
                    }
                case 7:
                    {
                        return (ErrorMessages.NOT_VALID, errorNumber);
                    }
                default:
                    {
                        return ($"Raw Error: { encodedErrorDescription }", errorNumber);
                    }
            }
        }

        /// <summary>
        /// This function is meant to provide 
        /// an easy way for the controllers to create error responses
        /// </summary>
        /// <param name="errorNumber"></param>
        /// <param name="errorDescription"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public static ErrorResponse NewErrorResponse(int errorNumber, string errorDescription, string parameterName, string parameterValue)
        {
            return new ErrorResponse()
            {
                ErrorNumber = errorNumber,
                ErrorDescription = errorDescription,
                ParameterName = parameterName,
                ParameterValue = parameterValue,
            };
        }

    }
}
