using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary;
using RestSDKLibrary.Models;

namespace RestFunctionalTests
{
    /// <summary>
    /// This class contains tests for task deletion
    /// </summary>
    [TestClass]
    public class DeleteTasksTests
    {
        /// <summary>
        /// Attempts to make a valid request to delete a task
        /// 
        /// This test creates a task and then makes a second request
        /// to delete it
        /// 
        /// This expects the API to return a success response (which is null in this case)
        /// </summary>
        [TestMethod]
        public void VerifySuccessfulTaskDeletion()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse createResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            ErrorResponse deleteResponse = Constants.ClientConnectionConfig.client.DeleteTask((int)createResponse.Id);

            Assert.IsNull(deleteResponse);
        }

        /// <summary>
        /// Attemps to delete a task that does not exist
        /// 
        /// This test expects the API to return an error response
        /// with the correct information
        /// </summary>
        [TestMethod]
        public void VerifyNotFoundTaskDeletion()
        {
            ErrorResponse deleteResponse = Constants.ClientConnectionConfig.client.DeleteTask(15);

            Assert.AreEqual(deleteResponse.ErrorNumber, 5);
            Assert.IsTrue(deleteResponse.ErrorDescription.Equals("The entity could not be found"));
            Assert.IsTrue(deleteResponse.ParameterName.Equals("Id"));
            Assert.IsTrue(deleteResponse.ParameterValue.Equals("15"));
        }

    }
}
