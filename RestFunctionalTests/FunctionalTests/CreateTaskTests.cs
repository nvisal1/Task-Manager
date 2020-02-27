using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary.Models;
using RestSDKLibrary;
using System;

namespace RestFunctionalTests
{
    /// <summary>
    /// This class contains tests for task creation
    /// </summary>
    [TestClass]
    public class CreateTaskTests
    {
        /// <summary>
        /// Attemps to create a valid Task
        /// 
        /// This test expects the API to return a success response
        /// with the correct information
        /// 
        /// The task is deleted after it is created
        /// </summary>
        [TestMethod]
        public void VerifySucessfulTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse response = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)response.Id);

            Assert.IsNotNull(response.Id);
            Assert.AreEqual(response.TaskName, payload.TaskName);
            Assert.AreEqual(response.IsCompleted, payload.IsCompleted);
            Assert.IsTrue(response.DueDate.Equals(payload.DueDate));
        }

        /// <summary>
        /// Attempts to create a task that has a task name longer than 100 characters
        /// 
        /// This test expects the API to return an error response
        /// with the correct information
        /// </summary>
        [TestMethod]
        public void VerifyLongTaskNameTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            // If the test payload is not formatted correctly, fail the test before making a request to the API
            if (payload.TaskName.Length < 100)
            {
                Assert.Fail("This test requires the payload to have a TaskName of length 100 or greater");
            }

            ErrorResponse response = (ErrorResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 2);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter value is too large"));
            Assert.IsTrue(response.ParameterName.Equals("TaskName"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.TaskName));
        }

        /// <summary>
        /// Attemps to create a task with an empty task name
        /// 
        /// This test expects the API to return an error response
        /// with the correct information
        /// </summary>
        [TestMethod]
        public void VerifyEmptyTaskNameTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            // If the test payload is not formatted correctly, fail the test before making a request to the API
            if (payload.TaskName.Length > 0)
            {
                Assert.Fail("This test requires the payload to have an empty TaskName");
            }

            ErrorResponse response = (ErrorResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("TaskName"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.TaskName));
        }

        /// <summary>
        /// Attempts to create a task with a null task name
        /// 
        /// This test expects the API to return an error response
        /// with the correct information
        /// </summary>
        [TestMethod]
        public void VerifyNullTaskNameTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            // If the test payload is not formatted correctly, fail the test before making a request to the API
            if (!String.IsNullOrEmpty(payload.TaskName))
            {
                Assert.Fail("This test requires the payload to have a null TaskName");
            }

            ErrorResponse response = (ErrorResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("TaskName"));
            Assert.IsTrue(response.ParameterValue == null);
        }

        /// <summary>
        /// Attempts to create a task with a null IsCompleted property
        /// 
        /// This test expects the API to return an error response
        /// with the correct ionformation
        /// </summary>
        [TestMethod]
        public void VerifyNullIsCompletedTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                DueDate = "2012-04-23",
            };

            // If the test payload is not formatted correctly, fail the test before making a request to the API
            if (payload.IsCompleted != null)
            {
                Assert.Fail("This test requires the payload to have a null IsCompleted");
            }

            ErrorResponse response = (ErrorResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("IsCompleted"));
            Assert.IsTrue(response.ParameterValue == null);
        }

        /// <summary>
        /// Attempts to create a task with an empty due date
        /// 
        /// This test expects the API to return an error response
        /// with the correct information
        /// </summary>
        [TestMethod]
        public void VerifyEmptyDueDateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                IsCompleted = false,
                DueDate = ""
            };

            // If the test payload is not formatted correctly, fail the test before making a request to the API
            if (payload.DueDate.Length > 0)
            {
                Assert.Fail("This test requires the payload to have an empty DueDate");
            }

            ErrorResponse response = (ErrorResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("DueDate"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.DueDate));
        }

        /// <summary>
        /// Attempts to create a task with a null due date
        /// 
        /// This test expects the API to return an error response
        /// with the correct information
        /// </summary>
        [TestMethod]
        public void VerifyNullDueDateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                IsCompleted = false,
            };

            // If the test payload is not formatted correctly, fail the test before making a request to the API
            if (!String.IsNullOrEmpty(payload.DueDate))
            {
                Assert.Fail("This test requires the payload to have a null DueDate");
            }

            ErrorResponse response = (ErrorResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("DueDate"));
            Assert.IsTrue(response.ParameterValue == null);
        }

        /// <summary>
        /// Attempts to create a task with a due date that is too long
        /// 
        /// This test expects the API to return an error response
        /// with the correct information
        /// </summary>
        [TestMethod]
        public void VerifyIncorrectFormatDueDateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                IsCompleted = false,
                DueDate = "stringgggg"
            };

            // If the test payload is not formatted correctly, fail the test before making a request to the API
            if (payload.DueDate.Length < 10)
            {
                Assert.Fail("This test requires the payload to have a DueDate longer than 10 characters");
            }

            ErrorResponse response = (ErrorResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 7);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter value is not valid"));
            Assert.IsTrue(response.ParameterName.Equals("DueDate"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.DueDate));
        }

        /// <summary>
        /// Attempts to create a task with a due date that is too short
        /// 
        /// This test expects the API to return an error response
        /// with the correct information
        /// </summary>
        [TestMethod]
        public void VerifyShortDueDateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                IsCompleted = false,
                DueDate = "2012-04"
            };

            // If the test payload is not formatted correctly, fail the test before making a request to the API
            if (payload.DueDate.Length >= 10)
            {
                Assert.Fail("This test requires the payload to have a DueDate shorter than 10 characters");
            }

            ErrorResponse response = (ErrorResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 6);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter value is too small"));
            Assert.IsTrue(response.ParameterName.Equals("DueDate"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.DueDate));
        }

        /// <summary>
        /// Attempts to create a task that already exists
        /// 
        /// This test attempts to insert a task
        /// and then makes the same request again
        /// 
        /// This test expects the API to return an error response
        /// with the correct infromation on the second request 
        /// 
        /// The task is deleted after it is created
        /// </summary>
        [TestMethod]
        public void VerifyDuplicateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name duplicate",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse taskResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            ErrorResponse errorResponse = (ErrorResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            Constants.ClientConnectionConfig.client.DeleteTask((int)taskResponse.Id);

            Assert.AreEqual(errorResponse.ErrorNumber, 1);
            Assert.IsTrue(errorResponse.ErrorDescription.Equals("The entity already exists"));
            Assert.IsTrue(errorResponse.ParameterName.Equals("TaskName"));
            Assert.IsTrue(errorResponse.ParameterValue.Equals(payload.TaskName));
        }

    }
}
