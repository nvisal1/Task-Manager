using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSDKLibrary;
using RestSDKLibrary.Models;
using System;
using System.Collections.Generic;

namespace RestFunctionalTests
{
    [TestClass]
    public class GetAllTasksTests
    {
        [TestMethod]
        public void VerifySuccessfulReadAllTasks()
        {
            List<TaskResponse> response = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks();

            Assert.AreEqual(response.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[0].DueDate), Convert.ToDateTime(response[1].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[1].DueDate), Convert.ToDateTime(response[2].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[2].DueDate), Convert.ToDateTime(response[3].DueDate)) < 0);
        }

        [TestMethod]
        public void VerifySuccessfulAscReadAllTasks()
        {
            List<TaskResponse> response = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(orderByDate: "Asc");

            Assert.AreEqual(response.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[0].DueDate), Convert.ToDateTime(response[1].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[1].DueDate), Convert.ToDateTime(response[2].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(response[2].DueDate), Convert.ToDateTime(response[3].DueDate)) < 0);
        }

        [TestMethod]
        public void VerifySuccessfulDescReadAllTasks()
        {
            List<TaskResponse> response = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(orderByDate: "Desc");

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

            TaskResponse createResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            List<TaskResponse> getAllResponse = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "Completed");

            Constants.ClientConnectionConfig.client.DeleteTask((int)createResponse.Id);

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
            List<TaskResponse> getAllResponse = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "NotCompleted");

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
            List<TaskResponse> getAllResponse = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "All");

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

            TaskResponse createResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            List<TaskResponse> getAllResponse = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "Completed", orderByDate: "Asc");

            Constants.ClientConnectionConfig.client.DeleteTask((int)createResponse.Id);

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

            TaskResponse createResponse = (TaskResponse)Constants.ClientConnectionConfig.client.CreateTask(body: payload);

            List<TaskResponse> getAllResponse = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "Completed", orderByDate: "Desc");

            Constants.ClientConnectionConfig.client.DeleteTask((int)createResponse.Id);

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
            List<TaskResponse> getAllResponse = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "NotCompleted", orderByDate: "Asc");

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
            List<TaskResponse> getAllResponse = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "NotCompleted", orderByDate: "Desc");

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
            List<TaskResponse> getAllResponse = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "All", orderByDate: "Asc");

            Assert.AreEqual(getAllResponse.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[1].DueDate), Convert.ToDateTime(getAllResponse[2].DueDate)) < 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[2].DueDate), Convert.ToDateTime(getAllResponse[3].DueDate)) < 0);
        }

        [TestMethod]
        public void VerifySuccessfulDescAllReadAllTasks()
        {
            List<TaskResponse> getAllResponse = (List<TaskResponse>)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "All", orderByDate: "Desc");

            Assert.AreEqual(getAllResponse.Count, 4);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[0].DueDate), Convert.ToDateTime(getAllResponse[1].DueDate)) > 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[1].DueDate), Convert.ToDateTime(getAllResponse[2].DueDate)) > 0);
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(getAllResponse[2].DueDate), Convert.ToDateTime(getAllResponse[3].DueDate)) > 0);
        }

        [TestMethod]
        public void VerifyInvalidOrderByDateReadAllTasks()
        {
            ErrorResponse getAllResponse = (ErrorResponse)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "All", orderByDate: "Invalid");

            Assert.AreEqual(getAllResponse.ErrorNumber, 7);
            Assert.IsTrue(getAllResponse.ErrorDescription.Equals("The parameter value is not valid"));
            Assert.IsTrue(getAllResponse.ParameterName.Equals("orderByDate"));
            Assert.IsTrue(getAllResponse.ParameterValue.Equals("Invalid"));
        }

        [TestMethod]
        public void VerifyInvalidTaskStatusReadAllTasks()
        {
            ErrorResponse getAllResponse = (ErrorResponse)Constants.ClientConnectionConfig.client.GetAllTasks(taskStatus: "Invalid", orderByDate: "Asc");

            Assert.AreEqual(getAllResponse.ErrorNumber, 7);
            Assert.IsTrue(getAllResponse.ErrorDescription.Equals("The parameter value is not valid"));
            Assert.IsTrue(getAllResponse.ParameterName.Equals("taskStatus"));
            Assert.IsTrue(getAllResponse.ParameterValue.Equals("Invalid"));
        }
    }
}
