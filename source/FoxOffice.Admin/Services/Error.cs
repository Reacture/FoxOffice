namespace FoxOffice.Admin.Services
{
    public readonly struct Error<T> : IResult<T>
    {
        public Error(string message) => Message = message;

        public string Message { get; }
    }
}
