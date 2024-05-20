namespace AskJavra.ViewModels.Dto
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ResponseDto() { }

        public ResponseDto(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
    public class ResponseListDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; }
        public ResponseListDto() { }
        public ResponseListDto(bool success, string message, List<T> data)
        {
            Success=success;
            Message = message;  
            Data = data;
        }
    }
}
