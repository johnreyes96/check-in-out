using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace check_in_out.Test.Helpers
{
    internal class MockCloudTableCheckInOut : CloudTable
    {
        public MockCloudTableCheckInOut(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableCheckInOut(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableCheckInOut(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetCheckInEntity()
            });
        }

        public override async Task<TableQuerySegment<CheckInOutEntity>> ExecuteQuerySegmentedAsync<CheckInOutEntity>(TableQuery<CheckInOutEntity> query, TableContinuationToken token)
        {
            ConstructorInfo constructor = typeof(TableQuerySegment<CheckInOutEntity>)
                   .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            return await Task.FromResult(constructor.Invoke(new object[] { new List<CheckInOutEntity>() }) as TableQuerySegment<CheckInOutEntity>);
        }
    }
}
