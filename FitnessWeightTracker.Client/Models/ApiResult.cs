using System.Net;

namespace FitnessWeightTracker.Client.Models
{
    public sealed class ApiResult<T>
    {
        public bool Success { get; init; }
        public T? Value { get; init; }
        public ProblemDetails? Problem { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public string? ErrorMessage { get; init; }

        public static ApiResult<T> Ok(T value, HttpStatusCode status = HttpStatusCode.OK) =>
            new() { Success = true, Value = value, StatusCode = status };

        public static ApiResult<T> Fail(HttpStatusCode status, ProblemDetails? problem = null, string? error = null) =>
            new() { Success = false, Problem = problem, StatusCode = status, ErrorMessage = error };
    }
}