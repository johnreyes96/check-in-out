using check_in_out.Common.Models;
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
    public class ConsolidatedApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateCheckInOut_When_Invoked_Method_Must_Return_200()
        {
            MockCloudTableConsolidated mockConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Consolidated consolidatedRequest = TestFactory.GetConsolidatedRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(consolidatedRequest);

            IActionResult response = await ConsolidatedApi.GetConsolidatedByDate(request, mockConsolidated, DateTime.Now, logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
