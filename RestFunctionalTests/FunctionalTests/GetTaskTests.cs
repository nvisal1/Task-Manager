using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary;
using RestSDKLibrary.Models;

namespace RestFunctionalTests
{
    /// <summary>
    /// This class contains tests for getting a single task
    /// </summary>
    [TestClass]
    public class GetTaskTests
    {
        /// <summary>
        /// Attempts to make a valid request to read a task
        /// 
        /// This test inserts a task and then makes a second
        /// request to read that task
        /// 
        /// This test expects the API to return a success response
        /// with the correct information on the second request
        /// 
        /// The task is deleted after it is created
        /// </summary>
        [TestMethod]
        public void VerifySuccessfulTaskReadById()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse createResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            TaskResponse readResponse = (TaskResponse)Constants.ClientConnectionConfig.client.GetTask((int)createResponse.Id);

            Constants.ClientConnectionConfig.client.DeleteTask((int)createResponse.Id);

            Assert.AreEqual(readResponse.Id, createResponse.Id);
            Assert.AreEqual(readResponse.TaskName, createResponse.TaskName);
            Assert.AreEqual(readResponse.IsCompleted, createResponse.IsCompleted);
            Assert.IsTrue(readResponse.DueDate.Equals(createResponse.DueDate));
        }

        /// <summary>
        /// Attempts to read a task that does not exist
        /// 
        /// This test expects the API to return an error response with
        /// the correct information
        /// </summary>
        [TestMethod]
        public void VerifyNotFoundTaskReadById()
        {
            ErrorResponse readResponse = (ErrorResponse)Constants.ClientConnectionConfig.client.GetTask(15);

            Assert.AreEqual(readResponse.ErrorNumber, 5);
            Assert.IsTrue(readResponse.ErrorDescription.Equals("The entity could not be found"));
            Assert.IsTrue(readResponse.ParameterName.Equals("Id"));
            Assert.IsTrue(readResponse.ParameterValue.Equals("15"));
        }
    }
}
