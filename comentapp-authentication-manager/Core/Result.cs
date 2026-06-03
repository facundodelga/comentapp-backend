namespace comentapp_authentication_manager.Core
{
    public class Result<T>
    {
        public T Value { get; }
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public int? ErrorCode { get; set; }

        private Result(T value)
        {
            Value = value;
            IsSuccess = true;
        }

        private Result(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsSuccess = false;
        }

        private Result(string errorMessage, int errorCode)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            IsSuccess = false;
        }

        public static Result<T> Success(T value) => new Result<T>(value);
        public static Result<T> Failure(string errorMessage) => new Result<T>(errorMessage);
        public static Result<T> Failure(string errorMessage, int errorCode) => new Result<T>(errorMessage, errorCode);
    }
}
