namespace check_in_out.Common.Responses
{
    public class Response
    {
        public static Response Instance { get; } = new Response();
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }

        public Response CreateResponseOK(string message, object checkInOutEntityEntity)
        {
            return new Response
            {
                IsSuccess = true,
                Message = message,
                Result = checkInOutEntityEntity
            };
        }
    }
}
