using check_in_out.Common.Responses;
using check_in_out.Common.Utils;
using check_in_out.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace check_in_out.Functions.Functions
{
    public static class ConsolidatedApi
    {
        [FunctionName("GetConsolidatedByDate")]
        public static async Task<IActionResult> GetConsolidatedByDate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidated/{date}")] HttpRequest req,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            DateTime date,
            ILogger log)
        {
            log.LogInformation($"Get all consolidated of the day: {date}");

            string filter = TableQuery.GenerateFilterConditionForDate("Workday", QueryComparisons.Equal, DateUtils.GetDateTimeMidnightInUniversalTime(date));
            TableQuery<ConsolidatedEntity> queryConsolidatedEntity = new TableQuery<ConsolidatedEntity>().Where(filter);
            TableQuerySegment<ConsolidatedEntity> queryResultconsolidatedEntity = await consolidatedTable.ExecuteQuerySegmentedAsync(queryConsolidatedEntity, null);
            List<ConsolidatedEntity> consolidatedEntities = queryResultconsolidatedEntity.Results;

            string message = "Retrieved all consolidated of the day.";
            log.LogInformation(message);

            return new OkObjectResult(Response.Instance.CreateResponseOK(message, consolidatedEntities));
        }
    }
}
