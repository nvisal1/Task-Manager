using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary;
using RestSDKLibrary.Models;

namespace RestFunctionalTests
{
    [TestClass]
    public class DeleteTasksTests
    {
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
