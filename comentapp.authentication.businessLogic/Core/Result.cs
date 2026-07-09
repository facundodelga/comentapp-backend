namespace comentapp.authentication.businessLogic.Core
{
    /// <summary>
    /// Represents the outcome of an authentication business-logic operation:
    /// either a success carrying a <typeparamref name="T"/> value, or a failure
    /// carrying an error message and optional error code.
    /// </summary>
    /// <typeparam name="T">The type of value returned on success.</typeparam>
    public class Result<T>
    {
        /// <summary>The value produced by a successful operation. Undefined when <see cref="IsSuccess"/> is <c>false</c>.</summary>
        public T Value { get; }

        /// <summary>Whether the operation succeeded.</summary>
        public bool IsSuccess { get; }

        /// <summary>Human-readable error message. Undefined when <see cref="IsSuccess"/> is <c>true</c>.</summary>
        public string ErrorMessage { get; }

        /// <summary>Optional numeric error code (e.g. an <see cref="UserServiceErrorCodes"/> value or HTTP status code) further identifying the failure.</summary>
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

        /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
        public static Result<T> Success(T value) => new Result<T>(value);

        /// <summary>Creates a failure result with the given error message.</summary>
        public static Result<T> Failure(string errorMessage) => new Result<T>(errorMessage);

        /// <summary>Creates a failure result with the given error message and error code.</summary>
        public static Result<T> Failure(string errorMessage, int errorCode) => new Result<T>(errorMessage, errorCode);
    }
}
