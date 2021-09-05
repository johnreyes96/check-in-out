using check_in_out.Functions.Functions;
using check_in_out.Test.Helpers;
using System;
using Xunit;

namespace check_in_out.Test.Tests
{
    public class ScheduledConsolidatedTest
    {
        [Fact]
        public void ScheduledConsolidated_When_Time_Trigger_And_Invoke_Method_Should_Log_Message()
        {
            MockCloudTableCheckInOut mockCheckInOut = new MockCloudTableCheckInOut(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableConsolidated mockConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            _ = ScheduledConsolidated.Run(null, mockCheckInOut, mockConsolidated, logger);
            string message = logger.Logs[0];

            Assert.Contains("Consolidating check ins and check outs function executed at", message);
        }
    }
}
