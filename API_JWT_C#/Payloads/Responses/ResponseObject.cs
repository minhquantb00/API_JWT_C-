namespace API_JWT_C_.Payloads.Responses
{
    public class ResponseObject<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public DateTime ResponseDate { get; set; }
        public T Data { get; set; }

        public ResponseObject()
        {
        }

        public ResponseObject(int status, string message, int code, DateTime responseDate, T data)
        {
            Status = status;
            Message = message;
            Code = code;
            ResponseDate = responseDate;
            Data = data;
        }

        public ResponseObject<T> ResponseSuccess(string message, T data)
        {
            return new ResponseObject<T>(0, message, StatusCodes.Status200OK, DateTime.Now, data);
        }

        public ResponseObject<T> ResponseError(int statusCode, string message, T data)
        {
            return new ResponseObject<T>(0, message, statusCode, DateTime.Now, data);
        }
    }
}
