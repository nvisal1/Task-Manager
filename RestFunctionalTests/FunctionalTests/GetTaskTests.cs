using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary;
using RestSDKLibrary.Models;

namespace RestFunctionalTests
{
    [TestClass]
    public class GetTaskTests
    {

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
