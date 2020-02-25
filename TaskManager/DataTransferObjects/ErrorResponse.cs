using System;

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
        public string ParameterName { get; set; }

        /// <summary>
        /// The value of the parameter that caused the error
        /// 
        /// If the error is not tied to a specific parameter,
        /// then this value can be null
        /// </summary>
        public string ParameterValue { get; set; }

        /// <summary>
        /// A description of the error, not localized,
        /// intended for developer consumption
        /// </summary>
        public string ErrorDescription { get; set; }

        public static int GetErrorNumberFromDescription(string encodedErrorDescription)
        {
            if (encodedErrorDescription.Contains("field is required")) return 3;
            else if (encodedErrorDescription.Contains("maximum length")) return 2;
            else if (encodedErrorDescription.Contains("JSON value could not be converted")) return 7;
            else if (encodedErrorDescription.Contains("minimum length")) return 6;

            return 0;
        }

        public static (string decodedErrorMessage, int decodedErrorNumber) GetErrorMessage(string encodedErrorDescription)
        {
            int errorNumber = GetErrorNumberFromDescription(encodedErrorDescription);

            switch(errorNumber)
            {
                case 1:
                    {
                        return ("The entity already exists", errorNumber);
                    }
                case 2:
                    {
                        return ("The parameter value is too large", errorNumber);
                    }
                case 3:
                    {
                        return ("The parameter is required", errorNumber);
                    }
                case 4:
                    {
                        return ("The maximum number of entities have been created. No further entities can be created at this time.", errorNumber);
                    }
                case 5:
                    {
                        return ("The entity could not be found", errorNumber);
                    }
                case 6:
                    {
                        return ("The parameter value is too small", errorNumber);
                    }
                case 7:
                    {
                        return ("The parameter value is not valid", errorNumber);
                    }
                default:
                    {
                        return ($"Raw Error: { encodedErrorDescription }", errorNumber);
                    }
            }
        }

        internal static ErrorResponse NewErrorResponse(object aT_)
        {
            throw new NotImplementedException();
        }

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
