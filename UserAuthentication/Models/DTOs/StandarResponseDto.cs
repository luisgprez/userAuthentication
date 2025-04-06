namespace UserAuthentication.Models.DTOs
{
    public class StandarResponseDto
    {
        public string StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public dynamic? Result { get; set; }
        public StandarResponseDto(string statusCode, bool isSuccess, string message, dynamic? result)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            Message = message;
            Result = result;
        }
    }
}
