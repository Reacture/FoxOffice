namespace FoxOffice.Admin.Services
{
    public readonly struct Success<T> : IResult<T>
    {
        public Success(T value) => Value = value;

        public T Value { get; }
    }
}
