using check_in_out.Common.Models;
using check_in_out.Common.Responses;
using check_in_out.Common.Utils;
using check_in_out.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
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
            TableOperation addOperation = TableOperation.Insert(checkInOutEntity);
            await checkInOutTable.ExecuteAsync(addOperation);
            string message = "New " + checkInOut.GetTypeDescription() + " stored in table.";

            log.LogInformation(message);

            return new OkObjectResult(response.CreateResponseOK(message, checkInOutEntity));
        }

        [FunctionName(nameof(GetAllCheckInOut))]
        public static async Task<IActionResult> GetAllCheckInOut(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "checkInOut")] HttpRequest req,
            [Table("checkInOut", Connection = "AzureWebJobsStorage")] CloudTable checkInOutTable,
            ILogger log)
        {
            log.LogInformation("Get all check in and check out received.");

            TableQuery<CheckInOutEntity> query = new TableQuery<CheckInOutEntity>();
            TableQuerySegment<CheckInOutEntity> checkInOuts = await checkInOutTable.ExecuteQuerySegmentedAsync(query, null);
            string message = "Retrieved all check in and check out.";

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
            string typeDescription = TypeFactory.Instance.GetTypeDescription(checkInOutEntity.Type);
            string message = typeDescription + $": {checkInOutEntity.RowKey}. retrieved.";

            log.LogInformation(message);

            return new OkObjectResult(Response.Instance.CreateResponseOK(message, checkInOutEntity));
        }
    }
}
