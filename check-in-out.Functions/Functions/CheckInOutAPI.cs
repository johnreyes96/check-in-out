using check_in_out.Common.Models;
using check_in_out.Common.Responses;
using check_in_out.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace check_in_out.Functions.Functions
{
    public class CheckInOutAPI
    {
        [FunctionName(nameof(CreateCheckInOut))]
        public static async Task<IActionResult> CreateCheckInOut(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "checkInOut")] HttpRequest req,
            [Table("checkInOut", Connection = "AzureWebJobsStorage")] CloudTable checkInOutTable,
            ILogger log)
        {
            log.LogInformation("Received a new check in or check out.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CheckInOut checkInOut = JsonConvert.DeserializeObject<CheckInOut>(requestBody);
            string messageError = checkInOut.ValidateRequiredFields();
            Response response = Response.Instance;

            if (!string.IsNullOrEmpty(messageError))
            {
                return new BadRequestObjectResult(response.CreateResponseError(messageError));
            }

            CheckInOutEntity checkInOutEntity = CheckInOutEntity.Instance.CreateCheckInOutEntityFromCheckInOut(checkInOut);
            await checkInOutTable.ExecuteAsync(TableOperation.Insert(checkInOutEntity));
            string typeDescription = CheckInOutType.Instance.GetDescription(checkInOutEntity.Type);
            string message = $"New {typeDescription} stored in table.";

            log.LogInformation(message);

            return new OkObjectResult(response.CreateResponseOK(message, checkInOutEntity));
        }

        [FunctionName(nameof(GetAllCheckInOut))]
        public static async Task<IActionResult> GetAllCheckInOut(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "checkInOut")] HttpRequest req,
            [Table("checkInOut", Connection = "AzureWebJobsStorage")] CloudTable checkInOutTable,
            ILogger log)
        {
            log.LogInformation("Get all check ins and check outs received.");

            TableQuery<CheckInOutEntity> query = new TableQuery<CheckInOutEntity>();
            TableQuerySegment<CheckInOutEntity> checkInOuts = await checkInOutTable.ExecuteQuerySegmentedAsync(query, null);
            string message = "Retrieved all check ins and check outs.";

            log.LogInformation(message);

            return new OkObjectResult(Response.Instance.CreateResponseOK(message, checkInOuts));
        }

        [FunctionName(nameof(GetCheckInOutById))]
        public static IActionResult GetCheckInOutById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "checkInOut/{id}")] HttpRequest req,
            [Table("checkInOut", "CHECKINOUT", "{id}", Connection = "AzureWebJobsStorage")] CheckInOutEntity checkInOutEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"Get check in or check out by id: {id}, received.");

            Response response = Response.Instance;
            if (checkInOutEntity == null)
            {
                return new BadRequestObjectResult(response.CreateResponseError("Check in or check out not found."));
            }
            string typeDescription = CheckInOutType.Instance.GetDescription(checkInOutEntity.Type);
            string message = $"{typeDescription}: {checkInOutEntity.RowKey}. retrieved.";

            log.LogInformation(message);

            return new OkObjectResult(response.CreateResponseOK(message, checkInOutEntity));
        }

        [FunctionName(nameof(UpdateCheckInOutById))]
        public static async Task<IActionResult> UpdateCheckInOutById(
                [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "checkInOut/{id}")] HttpRequest req,
                [Table("checkInOut", Connection = "AzureWebJobsStorage")] CloudTable checkInOutTable,
                string id,
                ILogger log)
        {
            log.LogInformation($"Update for check in or check out: {id}, received.");

            TableOperation findOperation = TableOperation.Retrieve<CheckInOutEntity>("CHECKINOUT", id);
            TableResult findResult = await checkInOutTable.ExecuteAsync(findOperation);
            Response response = Response.Instance;

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(response.CreateResponseError("Check in or check out not found."));
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CheckInOut checkInOut = JsonConvert.DeserializeObject<CheckInOut>(requestBody);
            string messageError = checkInOut.ValidateType(string.Empty);

            if (!string.IsNullOrEmpty(messageError))
            {
                return new BadRequestObjectResult(response.CreateResponseError(messageError));
            }

            CheckInOutEntity checkInOutEntity = (CheckInOutEntity)findResult.Result;
            checkInOutEntity.PrepareCheckInOutEntityToUpdate(checkInOut);
            await checkInOutTable.ExecuteAsync(TableOperation.Replace(checkInOutEntity));
            string typeDescription = CheckInOutType.Instance.GetDescription(checkInOut.Type);
            string message = $"{typeDescription}: {id}, updated in table.";

            log.LogInformation(message);

            return new OkObjectResult(response.CreateResponseOK(message, checkInOutEntity));
        }

        [FunctionName(nameof(DeleteCheckInOut))]
        public static async Task<IActionResult> DeleteCheckInOut(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "checkInOut/{id}")] HttpRequest req,
            [Table("checkInOut", "CHECKINOUT", "{id}", Connection = "AzureWebJobsStorage")] CheckInOutEntity checkInOutEntity,
            [Table("checkInOut", Connection = "AzureWebJobsStorage")] CloudTable checkInOutTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete check in or check out: {id}, received.");

            Response response = Response.Instance;
            if (checkInOutEntity == null)
            {
                string messageError = "Check in or check out not found.";
                return new BadRequestObjectResult(response.CreateResponseError(messageError));
            }
            await checkInOutTable.ExecuteAsync(TableOperation.Delete(checkInOutEntity));
            string typeDescription = CheckInOutType.Instance.GetDescription(checkInOutEntity.Type);
            string message = $"{typeDescription}: {checkInOutEntity.RowKey}. deleted.";

            log.LogInformation(message);

            return new OkObjectResult(response.CreateResponseOK(message, checkInOutEntity));
        }
    }
}
