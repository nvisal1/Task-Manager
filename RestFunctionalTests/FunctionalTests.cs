using Microsoft.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary;
using RestSDKLibrary.Models;
using System;
using System.Collections.Generic;

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

            client.DeleteTask((int)response.Id);

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
                DueDate = "stringgggg"
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

            client.DeleteTask((int)taskResponse.Id);

            Assert.AreEqual(errorResponse.ErrorNumber, 1);
            Assert.IsTrue(errorResponse.ErrorDescription.Equals("The entity already exists"));
            Assert.IsTrue(errorResponse.ParameterName.Equals("TaskName"));
            Assert.IsTrue(errorResponse.ParameterValue.Equals(payload.TaskName));
        }

        [TestMethod]
        public void VerifySuccessfulTaskUpdate()
        {
            TaskWriteRequestPayload createPayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse creationResponse = (TaskResponse)client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Updated Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = client.UpdateTask((int)creationResponse.Id, updatePayload);

            client.DeleteTask((int)creationResponse.Id);

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

            TaskResponse creationResponse = (TaskResponse)client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = client.UpdateTask((int)creationResponse.Id, updatePayload);

            client.DeleteTask((int)creationResponse.Id);

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

            TaskResponse creationResponse = (TaskResponse)client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = client.UpdateTask((int)creationResponse.Id, updatePayload);

            client.DeleteTask((int)creationResponse.Id);

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

            TaskResponse creationResponse = (TaskResponse)client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = client.UpdateTask((int)creationResponse.Id, updatePayload);

            client.DeleteTask((int)creationResponse.Id);

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

            TaskResponse creationResponse = (TaskResponse)client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                DueDate = "2012-04-23",
            };

            ErrorResponse updateResponse = client.UpdateTask((int)creationResponse.Id, updatePayload);

            client.DeleteTask((int)creationResponse.Id);

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

            TaskResponse creationResponse = (TaskResponse)client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "",
            };

            ErrorResponse updateResponse = client.UpdateTask((int)creationResponse.Id, updatePayload);

            client.DeleteTask((int)creationResponse.Id);

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

            TaskResponse creationResponse = (TaskResponse)client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
            };

            ErrorResponse updateResponse = client.UpdateTask((int)creationResponse.Id, updatePayload);

            client.DeleteTask((int)creationResponse.Id);

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

            TaskResponse creationResponse = (TaskResponse)client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "stringgggg",
            };

            ErrorResponse updateResponse = client.UpdateTask((int)creationResponse.Id, updatePayload);

            client.DeleteTask((int)creationResponse.Id);

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

            TaskResponse creationResponse = (TaskResponse)client.CreateTask(body: createPayload);

            TaskWriteRequestPayload updatePayload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "string",
            };

            ErrorResponse updateResponse = client.UpdateTask((int)creationResponse.Id, updatePayload);

            client.DeleteTask((int)creationResponse.Id);

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

            ErrorResponse updateResponse = client.UpdateTask(15, updatePayload);

            Assert.AreEqual(updateResponse.ErrorNumber, 5);
            Assert.IsTrue(updateResponse.ErrorDescription.Equals("The entity could not be found"));
            Assert.IsTrue(updateResponse.ParameterName.Equals("TaskName"));
            Assert.IsTrue(updateResponse.ParameterValue.Equals(updatePayload.TaskName));
        }

        [TestMethod]
        public void VerifySuccessfulTaskDeletion()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse createResponse = (TaskResponse)client.CreateTask(body: payload);

            ErrorResponse deleteResponse = client.DeleteTask((int)createResponse.Id);

            Assert.IsNull(deleteResponse);
        }

        [TestMethod]
        public void VerifyNotFoundTaskDeletion()
        {
            ErrorResponse deleteResponse = client.DeleteTask(15);

            Assert.AreEqual(deleteResponse.ErrorNumber, 5);
            Assert.IsTrue(deleteResponse.ErrorDescription.Equals("The entity could not be found"));
            Assert.IsTrue(deleteResponse.ParameterName.Equals("Id"));
            Assert.IsTrue(deleteResponse.ParameterValue.Equals("15"));
        }

        [TestMethod]
        public void VerifySuccessfulTaskReadById()
        {
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = false,
                DueDate = "2012-04-23",
            };

            TaskResponse createResponse = (TaskResponse)client.CreateTask(body: payload);

            TaskResponse readResponse = (TaskResponse)client.GetTask((int)createResponse.Id);

            client.DeleteTask((int)createResponse.Id);

            Assert.AreEqual(readResponse.Id, createResponse.Id);
            Assert.AreEqual(readResponse.TaskName, createResponse.TaskName);
            Assert.AreEqual(readResponse.IsCompleted, createResponse.IsCompleted);
            Assert.IsTrue(readResponse.DueDate.Equals(createResponse.DueDate));
        }

        [TestMethod]
        public void VerifyNotFoundTaskReadById()
        {
            ErrorResponse readResponse = (ErrorResponse)client.GetTask(15);

            Assert.AreEqual(readResponse.ErrorNumber, 5);
            Assert.IsTrue(readResponse.ErrorDescription.Equals("The entity could not be found"));
            Assert.IsTrue(readResponse.ParameterName.Equals("Id"));
            Assert.IsTrue(readResponse.ParameterValue.Equals("15"));
        }

        [TestMethod]
        public void VerifySuccessfulReadAllTasks()
        {
            List<TaskResponse> response = (List<TaskResponse>)client.GetAllTasks();

            Assert.AreEqual(response.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[0].DueDate), Convert.ToDateTime(response[1].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[1].DueDate), Convert.ToDateTime(response[2].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[2].DueDate), Convert.ToDateTime(response[3].DueDate)) < 0);
        }

        [TestMethod]
        public void VerifySuccessfulAscReadAllTasks()
        {
            List<TaskResponse> response = (List<TaskResponse>)client.GetAllTasks(orderByDate: "Asc");

            Assert.AreEqual(response.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[0].DueDate), Convert.ToDateTime(response[1].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[1].DueDate), Convert.ToDateTime(response[2].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[2].DueDate), Convert.ToDateTime(response[3].DueDate)) < 0);
        }

        [TestMethod]
        public void VerifySuccessfulDescReadAllTasks()
        {
            List<TaskResponse> response = (List<TaskResponse>)client.GetAllTasks(orderByDate: "Desc");

            Assert.AreEqual(response.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[0].DueDate), Convert.ToDateTime(response[1].DueDate)) > 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[1].DueDate), Convert.ToDateTime(response[2].DueDate)) > 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[2].DueDate), Convert.ToDateTime(response[3].DueDate)) > 0);
        }

        [TestMethod]
        public void VerifySuccessfulCompletedReadAllTasks()
        {
            // Insert extra completed task to verify Asc order
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = true,
                DueDate = "2012-04-23",
            };

            TaskResponse createResponse = (TaskResponse)client.CreateTask(body: payload);

            List<TaskResponse> getAllResponse = (List<TaskResponse>)client.GetAllTasks(taskStatus: "Completed");

            client.DeleteTask((int)createResponse.Id);

            Assert.AreEqual(getAllResponse.Count, 2);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) < 0);

            foreach (TaskResponse taskResponse in getAllResponse)
            {
                Assert.IsTrue((bool)taskResponse.IsCompleted);
            }
        }

        [TestMethod]
        public void VerifySuccessfulNotCompletedReadAllTasks()
        {
            List<TaskResponse> getAllResponse = (List<TaskResponse>)client.GetAllTasks(taskStatus: "NotCompleted");

            Assert.AreEqual(getAllResponse.Count, 3);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[1].DueDate), Convert.ToDateTime(getAllResponse[2].DueDate)) < 0);

            foreach (TaskResponse taskResponse in getAllResponse)
            {
                Assert.IsFalse((bool)taskResponse.IsCompleted);
            }
        }

        [TestMethod]
        public void VerifySuccessfulAllReadAllTasks()
        {
            List<TaskResponse> getAllResponse = (List<TaskResponse>)client.GetAllTasks(taskStatus: "All");

            Assert.AreEqual(getAllResponse.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[1].DueDate), Convert.ToDateTime(getAllResponse[2].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[2].DueDate), Convert.ToDateTime(getAllResponse[3].DueDate)) < 0);
        }

        [TestMethod]
        public void VerifySuccessfulAscCompletedReadAllTasks()
        {
            // Insert extra completed task to verify Asc order
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = true,
                DueDate = "2012-04-23",
            };

            TaskResponse createResponse = (TaskResponse)client.CreateTask(body: payload);

            List<TaskResponse> getAllResponse = (List<TaskResponse>)client.GetAllTasks(taskStatus: "Completed", orderByDate: "Asc");

            client.DeleteTask((int)createResponse.Id);

            Assert.AreEqual(getAllResponse.Count, 2);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) < 0);

            foreach (TaskResponse taskResponse in getAllResponse)
            {
                Assert.IsTrue((bool)taskResponse.IsCompleted);
            }
        }

        [TestMethod]
        public void VerifySuccessfulDescCompletedReadAllTasks()
        {
            // Insert extra completed task to verify Asc order
            TaskWriteRequestPayload payload = new TaskWriteRequestPayload()
            {
                TaskName = "Valid Task Name",
                IsCompleted = true,
                DueDate = "2012-04-23",
            };

            TaskResponse createResponse = (TaskResponse)client.CreateTask(body: payload);

            List<TaskResponse> getAllResponse = (List<TaskResponse>)client.GetAllTasks(taskStatus: "Completed", orderByDate: "Desc");

            client.DeleteTask((int)createResponse.Id);

            Assert.AreEqual(getAllResponse.Count, 2);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) > 0);

            foreach (TaskResponse taskResponse in getAllResponse)
            {
                Assert.IsTrue((bool)taskResponse.IsCompleted);
            }
        }

        [TestMethod]
        public void VerifySuccessfulAscNotCompletedReadAllTasks()
        {
            List<TaskResponse> getAllResponse = (List<TaskResponse>)client.GetAllTasks(taskStatus: "NotCompleted", orderByDate: "Asc");

            Assert.AreEqual(getAllResponse.Count, 3);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[1].DueDate), Convert.ToDateTime(getAllResponse[2].DueDate)) < 0);

            foreach (TaskResponse taskResponse in getAllResponse)
            {
                Assert.IsFalse((bool)taskResponse.IsCompleted);
            }
        }

        [TestMethod]
        public void VerifySuccessfulDescNotCompletedReadAllTasks()
        {
            List<TaskResponse> getAllResponse = (List<TaskResponse>)client.GetAllTasks(taskStatus: "NotCompleted", orderByDate: "Desc");

            Assert.AreEqual(getAllResponse.Count, 3);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) > 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[1].DueDate), Convert.ToDateTime(getAllResponse[2].DueDate)) > 0);

            foreach (TaskResponse taskResponse in getAllResponse)
            {
                Assert.IsFalse((bool)taskResponse.IsCompleted);
            }
        }

        [TestMethod]
        public void VerifySuccessfulAscAllReadAllTasks()
        {
            List<TaskResponse> getAllResponse = (List<TaskResponse>)client.GetAllTasks(taskStatus: "All", orderByDate: "Asc");

            Assert.AreEqual(getAllResponse.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[1].DueDate), Convert.ToDateTime(getAllResponse[2].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[2].DueDate), Convert.ToDateTime(getAllResponse[3].DueDate)) < 0);
        }

        [TestMethod]
        public void VerifySuccessfulDescAllReadAllTasks()
        {
            List<TaskResponse> getAllResponse = (List<TaskResponse>)client.GetAllTasks(taskStatus: "All", orderByDate: "Desc");

            Assert.AreEqual(getAllResponse.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) > 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[1].DueDate), Convert.ToDateTime(getAllResponse[2].DueDate)) > 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[2].DueDate), Convert.ToDateTime(getAllResponse[3].DueDate)) > 0);
        }

        [TestMethod]
        public void VerifyInvalidOrderByDateReadAllTasks()
        {
            ErrorResponse getAllResponse = (ErrorResponse)client.GetAllTasks(taskStatus: "All", orderByDate: "Invalid");

            Assert.AreEqual(getAllResponse.ErrorNumber, 7);
            Assert.IsTrue(getAllResponse.ErrorDescription.Equals("The parameter value is not valid"));
            Assert.IsTrue(getAllResponse.ParameterName.Equals("orderByDate"));
            Assert.IsTrue(getAllResponse.ParameterValue.Equals("Invalid"));
        }

        [TestMethod]
        public void VerifyInvalidTaskStatusReadAllTasks()
        {
            ErrorResponse getAllResponse = (ErrorResponse)client.GetAllTasks(taskStatus: "Invalid", orderByDate: "Asc");

            Assert.AreEqual(getAllResponse.ErrorNumber, 7);
            Assert.IsTrue(getAllResponse.ErrorDescription.Equals("The parameter value is not valid"));
            Assert.IsTrue(getAllResponse.ParameterName.Equals("taskStatus"));
            Assert.IsTrue(getAllResponse.ParameterValue.Equals("Invalid"));
        }

    }
}
