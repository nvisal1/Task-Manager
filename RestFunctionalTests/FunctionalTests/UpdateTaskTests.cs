using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary;
using RestSDKLibrary.Models;

namespace RestFunctionalTests
{
    /// <summary>
    /// This class contains test for updating a task
    /// </summary>
    [TestClass]
    public class UpdateTaskTests
    {
        /// <summary>
        /// Attempts to make a valid request to update a task
        /// 
        /// This test creates a new task and then makes a second request
        /// to update it
        /// 
        /// This test excpects the API to return a success response 
        /// (which is null in this case) on the second request
        /// 
        /// The task is deleted after creation
        /// </summary>
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

        /// <summary>
        /// Attempts to update a task with a task name that is too long
        /// 
        /// This test creates a valid task and then makes a second request
        /// to update the task with task name that is too long
        /// 
        /// This test expects the API to return an error response
        /// with the correct information on the second request
        /// 
        /// The task is deleted after it is created
        /// </summary>
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

        /// <summary>
        /// Attempts to update a task with an empty task name
        /// 
        /// This test creates a valid task and then makes a second request
        /// to update the task with an empty task name
        /// 
        /// This test expects the API to return an error response
        /// with the correct information on the second request
        /// 
        /// The task is deleted after it is created
        /// </summary>
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

        /// <summary>
        /// Attempts to update a task with a null task name
        /// 
        /// This test creates a valid task and then makes a second request
        /// to update the task with a null task name
        /// 
        /// This test expects the API to return an error response
        /// with the correct information on the second request
        /// 
        /// The task is deleted after it is created
        /// </summary>
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

        /// <summary>
        /// Attempts to update a task with a null IsCompleted property
        /// 
        /// This test creates a valid task and then makes a second request
        /// to update the task with IsCompleted: null
        /// 
        /// This test expects the API to return an error response
        /// with the correct information on the second request
        /// 
        /// The task is deleted after it is created
        /// </summary>
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

        /// <summary>
        /// Attempts to update a task with an empty due date
        /// 
        /// This test creates a valid task and then makes a second request
        /// to update the task with an empty due date
        /// 
        /// This test expects the API to return an error response
        /// with the correct information on the second request
        /// 
        /// The task is deleted after it is created
        /// </summary>
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

        /// <summary>
        /// Attempts to update a task with a null due date
        /// 
        /// This test creates a valid task and then makes a second request
        /// to update the task with a null due date
        /// 
        /// This test expects the API to return an error response
        /// with the correct information on the second request
        /// 
        /// The task is deleted after it is created
        /// </summary>
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

        /// <summary>
        /// Attempts to update a task with an invalid due date
        /// 
        /// This test creates a valid task and then makes a second request
        /// to update the task with an invalid due date
        /// 
        /// This test expects the API to return an error response
        /// with the correct information on the second request
        /// 
        /// The task is deleted after it is created
        /// </summary>
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

        /// <summary>
        /// Attempts to update a task with a short due date
        /// 
        /// This test creates a valid task and then makes a second request
        /// to update the task with a short due date
        /// 
        /// This test expects the API to return an error response
        /// with the correct information on the second request
        /// 
        /// The task is deleted after it is created
        /// </summary>
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

        /// <summary>
        /// Attempts to update a task that does not exist
        /// 
        /// This test expects the API to return an error response
        /// with the correct information
        /// </summary>
        [TestMethod]
        public void VerifyNotFoundTaskUpdate()
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

        /// <summary>
        /// Attempts to update a task with a name that is used by
        /// another saved task
        /// 
        /// This test creates two tasks and then makes a third request
        /// to update the first task's name to match the name of the
        /// second task
        /// 
        /// This test expects the API to return an error response with the
        /// correct information on the third request
        /// 
        /// Both tasks are deleted after creation
        /// </summary>
        [TestMethod]
        public void VerifyAlreadyExistsTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskWriteRequestPayload createPayload2 = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name update",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload);

            TaskResponse creationResponse2 = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: createPayload2);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name update",
                IsCompleted = false,
                DueDate = "2000-02-08",
            };

            ErrorResponse updateResponse = Constants.ClientConnectionConfig.client.UpdateTask((int)creationResponse.Id, updatePayload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse.Id);
            Constants.ClientConnectionConfig.client.DeleteTask((int)creationResponse2.Id);

            Assert.AreEqual(updateResponse.ErrorNumber, 1);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The entity already exists"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("TaskName"));
            Assert.IsTrue(updateResponse.ParameterValue.Equals(updatePayload.TaskName));
        }
    }
}
