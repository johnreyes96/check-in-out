using check_in_out.Common.Models;
using check_in_out.Common.Utils;
using check_in_out.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;

namespace check_in_out.Test.Helpers
{
    internal class TestFactory
    {
        public static CheckInOutEntity GetCheckInEntity()
        {
            return new CheckInOutEntity
            {
                ETag = "*",
                PartitionKey = "CHECKINOUT",
                RowKey = Guid.NewGuid().ToString(),
                IsConsolidated = false,
                EmployeeId = 1,
                DateCheck = DateTime.UtcNow,
                Type = CheckInOutTypeFactory.Instance.CheckIn.Code
            };
        }
        public static ConsolidatedEntity GetConsolidatedEntity()
        {
            return new ConsolidatedEntity
            {
                ETag = "*",
                PartitionKey = "CONSOLIDATED",
                RowKey = Guid.NewGuid().ToString(),
                EmployeeId = 1,
                Workday = DateTime.UtcNow,
                MinutesWorked = 360
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(CheckInOut checkInOutRequest)
        {
            string request = JsonConvert.SerializeObject(checkInOutRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Consolidated consolidated)
        {
            string request = JsonConvert.SerializeObject(consolidated);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid checkInOutId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{checkInOutId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(DateTime consolidatedDate)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{consolidatedDate}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid checkInOutId, CheckInOut checkInOutRequest)
        {
            string request = JsonConvert.SerializeObject(checkInOutRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{checkInOutId}"
            };
        }

        public static CheckInOut GetCheckInOutRequest()
        {
            return new CheckInOut
            {
                IsConsolidated = false,
                EmployeeId = 1,
                DateCheck = DateTime.UtcNow,
                Type = CheckInOutTypeFactory.Instance.CheckIn.Code
            };
        }

        public static Consolidated GetConsolidatedRequest()
        {
            return new Consolidated
            {
                EmployeeId = 1,
                DateCheck = DateTime.UtcNow,
                MinutesWorked = 300
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
