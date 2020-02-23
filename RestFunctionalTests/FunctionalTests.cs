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

            try
            {
                var response = client.CreateTask(body: payload);

                Assert.Fail("The Task Manager API did not return an error code");
            } 
            catch (HttpOperationException error)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, error.Response.StatusCode);
            }
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

            try
            {
                var response = client.CreateTask(body: payload);

                Assert.Fail("The Task Manager API did not return an error code");
            }
            catch (HttpOperationException error)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, error.Response.StatusCode);
            }
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

            try
            {
                var response = client.CreateTask(body: payload);

                Assert.Fail("The Task Manager API did not return an error code");
            }
            catch (HttpOperationException error)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, error.Response.StatusCode);
            }
        }

        [TestMethod]
        public void VerifyNullIsCompletedTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                DueDate = "2012-04-23",
            };

            if (!payload.IsCompleted)
            {
                Assert.Fail("This test requires the payload to have a null IsCompleted");
            }

            try
            {
                var response = client.CreateTask(body: payload);

                Assert.Fail("The Task Manager API did not return an error code");
            }
            catch (HttpOperationException error)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, error.Response.StatusCode);
            }
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

            try
            {
                var response = client.CreateTask(body: payload);

                Assert.Fail("The Task Manager API did not return an error code");
            }
            catch (HttpOperationException error)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, error.Response.StatusCode);
            }
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

            try
            {
                var response = client.CreateTask(body: payload);

                Assert.Fail("The Task Manager API did not return an error code");
            }
            catch (HttpOperationException error)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, error.Response.StatusCode);
            }
        }

        [TestMethod]
        public void VerifyIncorrectFormatDueDateTaskCreation()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "valid name",
                IsCompleted = false,
                DueDate = "2012/04/20"
            };

            try
            {
                var response = client.CreateTask(body: payload);

                Assert.Fail("The Task Manager API did not return an error code");
            }
            catch (HttpOperationException error)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, error.Response.StatusCode);
            }
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

            try
            {
                var response = client.CreateTask(body: payload);

                Assert.Fail("The Task Manager API did not return an error code");
            }
            catch (HttpOperationException error)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, error.Response.StatusCode);
            }
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

            _ = client.CreateTask(body: payload);

            ErrorResponse response = (ErrorResponse)client.CreateTask(body: payload);

            Assert.AreEqual(response.ErrorNumber, 1);
        }

        [TestMethod]
        public void VerifySuccessfulTaskUpdate()
        {
           

        }
    }
}
