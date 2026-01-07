namespace AboveOurHeads.Core.Common
{
    public class ServiceResult
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        protected ServiceResult(bool isSuccess, string error)
        {
            if (isSuccess && error != string.Empty)
                throw new InvalidOperationException("Un résultat réussi ne peut pas avoir d'erreur");
            if (!isSuccess && error == string.Empty)
                throw new InvalidOperationException("Un résultat échoué doit avoir un message d'erreur");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static ServiceResult Success() => new(true, string.Empty);
        public static ServiceResult Failure(string error) => new(false, error);

        public static ServiceResult<T> Success<T>(T value) => new(value, true, string.Empty);
        public static ServiceResult<T> Failure<T>(string error) => new(default, false, error);
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Value { get; }

        protected internal ServiceResult(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public T GetValueOrDefault(T defaultValue = default)
            => IsSuccess ? Value : defaultValue;
    }
}
