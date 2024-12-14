namespace FunPokedex.Api.Models
{
    public class ErrorDto
    {
        public string Code { get; }
        public string Message { get; }

        public Dictionary<string, string> Details { get; }

        private ErrorDto(string code, string message)
        {
            Code = code;
            Message = message;
            Details = [];
        }

        internal ErrorDto AddDetails(string key, string value)
        {
            Details.Add(key, value);
            return this;
        }

        internal static ErrorDto NotFoundError(string message) => new ErrorDto("100", "Resource not found").AddDetails("message", message);
        internal static ErrorDto InternalServerError(string message) => new ErrorDto("500", "Internal server error").AddDetails("message", message);

    }
}
