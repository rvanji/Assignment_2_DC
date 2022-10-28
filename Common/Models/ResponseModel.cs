namespace Common.Models
{
    public class ResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public ResponseModel()
        {
        }

        public ResponseModel(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}