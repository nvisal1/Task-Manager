using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary;
using RestSDKLibrary.Models;

namespace RestFunctionalTests
{
    [TestClass]
    public class UpdateTaskTests
    {
        [TestMethod]
        public void VerifySuccessfulTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Updated Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);

            Assert.IsNull(updateResponse);
        }

        [TestMethod]
        public void VerifyLongTaskNameTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);

            Assert.AreEqual(updateResponse.ErrorNumber, 2);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The parameter value is too large"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("TaskName"));
            Assert.IsTrue(updateResponse.ParameterValue.Equals(updatePayload.TaskName));
        }

        [TestMethod]
        public void VerifyEmptyTaskNameTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);

            Assert.AreEqual(updateResponse.ErrorNumber, 3);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("TaskName"));
            Assert.IsTrue(updateResponse.ParameterValue.Equals(updatePayload.TaskName));
        }

        [TestMethod]
        public void VerifyNullTaskNameTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);

            Assert.AreEqual(updateResponse.ErrorNumber, 3);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("TaskName"));
            Assert.IsTrue(updateResponse.ParameterValue == null);
        }

        [TestMethod]
        public void VerifyNullIsCompletedTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);

            Assert.AreEqual(updateResponse.ErrorNumber, 3);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("IsCompleted"));
            Assert.IsTrue(updateResponse.ParameterValue == null);
        }

        [TestMethod]
        public void VerifyEmptyDueDateTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);

            Assert.AreEqual(updateResponse.ErrorNumber, 3);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("DueDate"));
            Assert.IsTrue(updateResponse.ParameterValue.Equals(updatePayload.DueDate));
        }

        [TestMethod]
        public void VerifyNullDueDateTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);

            Assert.AreEqual(updateResponse.ErrorNumber, 3);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("DueDate"));
            Assert.IsTrue(updateResponse.ParameterValue == null);
        }

        [TestMethod]
        public void VerifyIncorrectFormatDueDateTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "stringgggg",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);

            Assert.AreEqual(updateResponse.ErrorNumber, 7);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The parameter value is not valid"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("DueDate"));
            Assert.IsTrue(updateResponse.ParameterValue.Equals(updatePayload.DueDate));
        }

        [TestMethod]
        public void VerifyShortDueDateTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "string",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);

            Assert.AreEqual(updateResponse.ErrorNumber, 6);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The parameter value is too small"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("DueDate"));
            Assert.IsTrue(updateResponse.ParameterValue.Equals(updatePayload.DueDate));
        }

        [TestMethod]
        public void VerifyNotFoundTaskupdate()
        {
            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "1010-10-10",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask(15, updatePayload);

            Assert.AreEqual(updateResponse.ErrorNumber, 5);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The entity could not be found"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("TaskName"));
            Assert.IsTrue(updateResponse.ParameterValue.Equals(updatePayload.TaskName));
        }
    }
}
