using System.Net;
using Newtonsoft.Json;
using WearFitter.Web.Infrastructure.Serialization;

namespace WearFitter.Web.Infrastructure.Exceptions
{
    public class ApiExceptionDetails
    {
        public string Title { get; set; } = default!;

        public string? Detail { get; set; }
    }

    public class ApiException : Exception
    {
        private readonly ApiExceptionDetails? _details;
        private readonly string? _response;

        private string? message;
        private string? stackTrace;

        public ApiException(HttpStatusCode statusCode)
            : this($"Unexpected status code {statusCode}", (int)statusCode, null, null, null)
        {
        }

        public ApiException(string message, HttpStatusCode statusCode)
            : this(message, (int)statusCode, null, null, null)
        {
        }

        public ApiException(
            string message,
            int statusCode,
            string? response,
            IReadOnlyDictionary<string, IEnumerable<string>>? headers,
            Exception? innerException
        )
            : base(message, innerException)
        {
            StatusCode = (HttpStatusCode)statusCode;

            SetCorrelationId(headers);

            if (!string.IsNullOrWhiteSpace(response))
            {
                _details = JsonConvert.DeserializeObject<ApiExceptionDetails>(response, JsonSettingsConfigurator.Get());

                //try to deserialize response to ApiExceptionDetails. If can't response is unknown and we will add it to message.
                if (_details == null)
                {
                    _response = response;
                }
            }
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string? CorrelationId { get; private set; }

        public override string Message
        {
            get
            {
                if (message == null)
                {
                    message = string.Empty;

                    if (InnerException != null)
                    {
                        message += $"{InnerException.Message}\n";
                    }

                    if (_details != null)
                    {
                        message += $"{_details.Title}\n";
                    }

                    if (_response != null)
                    {
                        message += $"{_response}\n";
                    }

                    message += base.Message;
                }

                return message;
            }
        }

        public override string? StackTrace
        {
            get
            {
                if (stackTrace == null)
                {
                    stackTrace = string.Empty;

                    if (InnerException?.StackTrace != null)
                    {
                        stackTrace += $"{InnerException.StackTrace}\n";
                    }

                    if (_details != null)
                    {
                        if (_details.Detail != null)
                        {
                            stackTrace += $"{_details.Detail}\n";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(stackTrace))
                    {
                        stackTrace += "\n\n";
                    }

                    stackTrace += base.StackTrace;
                }

                return stackTrace;
            }
        }

        private void SetCorrelationId(IReadOnlyDictionary<string, IEnumerable<string>>? headers)
        {
            if (headers is null) return;

            bool headerExists = headers.TryGetValue(Constants.CorrelationId, out var correlationValue);

            if (!headerExists)
            {
                //Headers are case insensitive
                var items = headers
                    .Where(it => it.Key.Equals(Constants.CorrelationId, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                headerExists = items.Any();

                if (headerExists)
                {
                    correlationValue = items.Single().Value;
                }
            }

            CorrelationId = correlationValue?.FirstOrDefault();
        }
    }

    public class ApiException<TResult> : ApiException
    {
        public TResult Result { get; private set; }

        public ApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }
}
