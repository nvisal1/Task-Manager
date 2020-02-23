using Microsoft.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary;
using RestSDKLibrary.Models;
using System;
using System.Net;

namespace RestFunctionalTests
{
    [TestClass]
    public class FunctionalTests
    {
        const string LocalEndpointUrl = "https://localhost:44309";
        static ServiceClientCredentials serviceClientCredentials = new TokenCredentials("FakeTokenValue");
        static readonly RestSDKLibraryClient client = new RestSDKLibraryClient(new Uri(LocalEndpointUrl), serviceClientCredentials);


        [TestMethod]
        public void VerifySucessfulTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse response = (TaskResponse)client.CreateTask(body: payload);

            Assert.IsNotNull(response.Id);
            Assert.AreEqual(response.TaskName, payload.TaskName);
            Assert.AreEqual(response.IsCompleted, payload.IsCompleted);
            Assert.IsTrue(response.DueDate.Equals(payload.DueDate));

            client.DeleteTask((int)response.Id);
        }

        [TestMethod]
        public void VerifyLongTaskNameTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            if (payload.TaskName.Length < 100)
            {
                Assert.Fail("This test requires the payload to have a TaskName of length 100 or greater");
            }

            ErrorResponse response = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 2);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter value is too large"));
            Assert.IsTrue(response.ParameterName.Equals("TaskName"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.TaskName));
        }

        [TestMethod]
        public void VerifyEmptyTaskNameTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            if (payload.TaskName.Length > 0)
            {
                Assert.Fail("This test requires the payload to have an empty TaskName");
            }

            ErrorResponse response = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("TaskName"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.TaskName));
        }

         

        [TestMethod]
        public void VerifyNullTaskNameTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            if (!String.IsNullOrEmpty(payload.TaskName))
            {
                Assert.Fail("This test requires the payload to have a null TaskName");
            }

            ErrorResponse response = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("TaskName"));
            Assert.IsTrue(response.ParameterValue == null);
        }

        [TestMethod]
        public void VerifyNullIsCompletedTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                DueDate = "2012-04-23",
            };

            if (payload.IsCompleted != null)
            {
                Assert.Fail("This test requires the payload to have a null IsCompleted");
            }
             
            ErrorResponse response = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("IsCompleted"));
            Assert.IsTrue(response.ParameterValue == null);
        }

        [TestMethod]
        public void VerifyEmptyDueDateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                IsCompleted = false,
                DueDate = ""
            };

            if (payload.DueDate.Length > 0)
            {
                Assert.Fail("This test requires the payload to have an empty DueDate");
            }

            ErrorResponse response = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("DueDate"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.DueDate));
        }

        [TestMethod]
        public void VerifyNullDueDateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                IsCompleted = false,
            };

            if (!String.IsNullOrEmpty(payload.DueDate))
            {
                Assert.Fail("This test requires the payload to have a null DueDate");
            }

            ErrorResponse response = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 3);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter is required"));
            Assert.IsTrue(response.ParameterName.Equals("DueDate"));
            Assert.IsTrue(response.ParameterValue == null);
        }

        [TestMethod]
        public void VerifyIncorrectFormatDueDateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                IsCompleted = false,
                DueDate = "string"
            };

            ErrorResponse response = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 7);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter value is not valid"));
            Assert.IsTrue(response.ParameterName.Equals("DueDate"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.DueDate));
        }

        [TestMethod]
        public void VerifyShortDueDateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                IsCompleted = false,
                DueDate = "2012-04"
            };

            ErrorResponse response = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 6);
            Assert.IsTrue(response.ErrorDescription.Equals("The parameter value is too small"));
            Assert.IsTrue(response.ParameterName.Equals("DueDate"));
            Assert.IsTrue(response.ParameterValue.Equals(payload.DueDate));
        }

        [TestMethod]
        public void VerifyDuplicateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name duplicate",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse taskResponse = (TaskResponse)client.CreateTask(body: payload);

            ErrorResponse errorResponse = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(errorResponse.ErrorNumber, 1);
            Assert.IsTrue(errorResponse.ErrorDescription.Equals("The entity already exists"));
            Assert.IsTrue(errorResponse.ParameterName.Equals("TaskName"));
            Assert.IsTrue(errorResponse.ParameterValue.Equals(payload.TaskName));

            client.DeleteTask((int)taskResponse.Id);
        }

        [TestMethod]
        public void VerifySuccessfulTaskUpdate()
        {
           

        }
    }
}
