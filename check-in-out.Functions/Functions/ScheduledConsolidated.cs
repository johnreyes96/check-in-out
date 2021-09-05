using check_in_out.Common.Utils;
using check_in_out.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace check_in_out.Functions.Functions
{
    public static class ScheduledConsolidated
    {
        [FunctionName("ScheduledConsolidated")]
        public static async Task Run(
            [TimerTrigger("0 */60 * * * *")] TimerInfo myTimer,
            [Table("checkInOut", Connection = "AzureWebJobsStorage")] CloudTable checkInOutTable,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            ILogger log)
        {
            log.LogInformation($"Consolidating check ins and check outs function executed at: {DateTime.Now}");

            string filter = TableQuery.GenerateFilterConditionForBool("IsConsolidated", QueryComparisons.Equal, false);
            TableQuery<CheckInOutEntity> queryCheckInOutEntity = new TableQuery<CheckInOutEntity>().Where(filter);
            TableQuerySegment<CheckInOutEntity> checkInOutEntities = await checkInOutTable.ExecuteQuerySegmentedAsync(queryCheckInOutEntity, null);
            Dictionary<string, CheckInOutEntity> checkInsOutsEntityMap = GetCheckInsOutsEntityOrdered(checkInOutEntities);
            int updatedChecks = 0;
            int newsConsolidated = 0;
            int updatedConsolidated = 0;
            string checkInRowKey = string.Empty;
            List<ConsolidatedEntity> consolidatedEntityList = new List<ConsolidatedEntity>();

            foreach (string checkInOutEntityRowKey in checkInsOutsEntityMap.Keys)
            {
                CheckInOutEntity checkInOutEntity = checkInsOutsEntityMap[checkInOutEntityRowKey];
                if (CheckInOutTypeFactory.Instance.CheckIn.Code.Equals(checkInOutEntity.Type))
                {
                    checkInRowKey = checkInOutEntityRowKey;
                    continue;
                }
                if (CheckInOutTypeFactory.Instance.CheckOut.Code.Equals(checkInOutEntity.Type) && !string.Empty.Equals(checkInRowKey))
                {
                    CheckInOutEntity checkInEntity = checkInsOutsEntityMap[checkInRowKey];
                    checkInEntity.UpdateCheckInOutEntityForConsolidated();
                    checkInOutEntity.UpdateCheckInOutEntityForConsolidated();
                    await checkInOutTable.ExecuteAsync(TableOperation.Replace(checkInOutEntity));
                    await checkInOutTable.ExecuteAsync(TableOperation.Replace(checkInEntity));

                    int[] totalMinutesForEachDay = DateUtils.GetTotalMinutesForEachDayBetweenTwoDateTime(checkInEntity.DateCheck, checkInOutEntity.DateCheck);
                    consolidatedEntityList.Add(ConsolidatedEntity.CreateConsolidatedEntityFromCheckInOutEntity(checkInEntity, totalMinutesForEachDay[0]));
                    if (totalMinutesForEachDay.Length == 2)
                    {
                        consolidatedEntityList.Add(ConsolidatedEntity.CreateConsolidatedEntityFromCheckInOutEntity(checkInOutEntity, totalMinutesForEachDay[1]));
                    }

                    updatedChecks += 2;
                    checkInRowKey = string.Empty;
                    continue;
                }
                throw new ArgumentException($"There is not type with code {checkInOutEntity.Type}");
            }

            foreach (ConsolidatedEntity consolidatedEntity in consolidatedEntityList)
            {
                ConsolidatedEntity consolidatedEntityUpdated = consolidatedEntity;
                filter = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterConditionForInt("EmployeeId", QueryComparisons.Equal, consolidatedEntityUpdated.EmployeeId),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForDate("Workday", QueryComparisons.Equal, consolidatedEntityUpdated.Workday));
                TableQuery<ConsolidatedEntity> queryConsolidatedEntity = new TableQuery<ConsolidatedEntity>().Where(filter);
                TableQuerySegment<ConsolidatedEntity> queryResultconsolidatedEntity = await consolidatedTable.ExecuteQuerySegmentedAsync(queryConsolidatedEntity, null);
                List<ConsolidatedEntity> consolidatedEntityPersistedList = queryResultconsolidatedEntity.Results;

                if (consolidatedEntityPersistedList.Count == 0)
                {
                    updatedConsolidated++;
                }
                else
                {
                    int minutesWorked = consolidatedEntityUpdated.MinutesWorked;
                    consolidatedEntityUpdated = consolidatedEntityPersistedList.First();
                    consolidatedEntityUpdated.MinutesWorked += minutesWorked;
                    newsConsolidated++;
                }

                await consolidatedTable.ExecuteAsync(TableOperation.InsertOrReplace(consolidatedEntityUpdated));
            }

            log.LogInformation($"Consolidated summary: {updatedConsolidated} new consolidated, {newsConsolidated} updated consolidated, {updatedChecks} updated check ins and check outs at: {DateTime.Now}");
        }

        private static Dictionary<string, CheckInOutEntity> GetCheckInsOutsEntityOrdered(TableQuerySegment<CheckInOutEntity> checkInsOutsEntity)
        {
            return checkInsOutsEntity.Results.OrderBy(checkInOutEntity => checkInOutEntity.EmployeeId)
                .ThenBy(checkInOutEntity => checkInOutEntity.DateCheck)
                .ToDictionary(checkInOutEntity => checkInOutEntity.RowKey, checkInOutEntity => checkInOutEntity);
        }
    }
}
