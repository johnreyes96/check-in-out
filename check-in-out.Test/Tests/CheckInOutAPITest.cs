using check_in_out.Common.Models;
using check_in_out.Functions.Entities;
using check_in_out.Functions.Functions;
using check_in_out.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace check_in_out.Test.Tests
{
    public class CheckInOutAPITest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateCheckInOut_When_Invoked_Method_Must_Return_200()
        {
            MockCloudTableCheckInOut mockCheckInOut = new MockCloudTableCheckInOut(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            CheckInOut checkInOutRequest = TestFactory.GetCheckInOutRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(checkInOutRequest);

            IActionResult response = await CheckInOutAPI.CreateCheckInOut(request, mockCheckInOut, logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateCheckInOutById_When_Invoked_Method_Should_Return_200()
        {
            MockCloudTableCheckInOut mockCheckInOut = new MockCloudTableCheckInOut(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            CheckInOut checkInOutRequest = TestFactory.GetCheckInOutRequest();
            Guid checkInOutId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(checkInOutId, checkInOutRequest);

            IActionResult response = await CheckInOutAPI.UpdateCheckInOutById(request, mockCheckInOut, checkInOutId.ToString(), logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetAllCheckInOut_When_Invoked_Method_Should_Return_200()
        {
            MockCloudTableCheckInOut mockCheckInOut = new MockCloudTableCheckInOut(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            IActionResult response = await CheckInOutAPI.GetAllCheckInOut(request, mockCheckInOut, logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public void GetCheckInOutById_When_Invoked_Method_Should_Return_200()
        {
            MockCloudTableCheckInOut mockCheckInOut = new MockCloudTableCheckInOut(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            CheckInOutEntity mockCheckInOutEntity = TestFactory.GetCheckInEntity();
            Guid checkInOutId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(checkInOutId);

            IActionResult response = CheckInOutAPI.GetCheckInOutById(request, mockCheckInOutEntity, checkInOutId.ToString(), logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void DeleteCheckInOut_When_Invoked_Method_Should_Return_200()
        {
            MockCloudTableCheckInOut mockCheckInOut = new MockCloudTableCheckInOut(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            CheckInOutEntity mockCheckInOutEntity = TestFactory.GetCheckInEntity();
            Guid checkInOutId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(checkInOutId);

            IActionResult response = await CheckInOutAPI.DeleteCheckInOut(request, mockCheckInOutEntity, mockCheckInOut, checkInOutId.ToString(), logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
