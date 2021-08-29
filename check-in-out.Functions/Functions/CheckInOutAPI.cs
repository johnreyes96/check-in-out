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

            if (!string.IsNullOrEmpty(messageError))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = messageError
                });
            }

            CheckInOutEntity checkInOutEntityEntity = CheckInOutEntity.Instance.CreateCheckInOutEntityFromCheckInOut(checkInOut);
            TableOperation addOperation = TableOperation.Insert(checkInOutEntityEntity);
            await checkInOutTable.ExecuteAsync(addOperation);
            string message = checkInOut.GetMessageNewCheckInOut();

            log.LogInformation(message);

            return new OkObjectResult(Response.Instance.CreateResponseOK(message, checkInOutEntityEntity));
        }
    }
}
