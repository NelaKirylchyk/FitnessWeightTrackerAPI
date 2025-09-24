using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using FitnessWeightTracker.Client.Models;

namespace FitnessWeightTracker.Client.Services
{
    public sealed class BodyWeightRecordsService
    {
        private readonly HttpClient _http;
        private const string ApiPath = "/api/BodyWeightRecords";

        public BodyWeightRecordsService(HttpClient http)
        {
            _http = http;
        }

        private static async Task<ProblemDetails?> ReadProblemAsync(HttpResponseMessage resp, CancellationToken ct)
        {
            try
            {
                return await resp.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: ct);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ApiResult<List<BodyWeightRecord>>> GetAsync(bool ascendingOrder, CancellationToken ct = default)
        {
            var url = QueryHelpers.AddQueryString(ApiPath, "ascendingOrder", ascendingOrder ? "true" : "false");
            using var resp = await _http.GetAsync(url, ct);

            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadFromJsonAsync<List<BodyWeightRecord>>(cancellationToken: ct) ?? new List<BodyWeightRecord>();
                return ApiResult<List<BodyWeightRecord>>.Ok(data, resp.StatusCode);
            }

            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<List<BodyWeightRecord>>.Fail(resp.StatusCode, problem, error: $"GET {url} failed");
        }

        public async Task<ApiResult<BodyWeightRecord>> AddAsync(BodyWeightRecordDTO dto, CancellationToken ct = default)
        {
            using var resp = await _http.PostAsJsonAsync(ApiPath, dto, ct);
            if (resp.IsSuccessStatusCode)
            {
                // API returns the created entity in the body
                var created = await resp.Content.ReadFromJsonAsync<BodyWeightRecord>(cancellationToken: ct);
                if (created is not null)
                    return ApiResult<BodyWeightRecord>.Ok(created, resp.StatusCode);
            }

            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<BodyWeightRecord>.Fail(resp.StatusCode, problem, error: "Create failed");
        }

        public async Task<ApiResult<bool>> UpdateAsync(int id, BodyWeightRecordDTO dto, CancellationToken ct = default)
        {
            using var resp = await _http.PutAsJsonAsync($"{ApiPath}/{id}", dto, ct);
            if (resp.IsSuccessStatusCode)
                return ApiResult<bool>.Ok(true, resp.StatusCode);

            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<bool>.Fail(resp.StatusCode, problem, error: "Update failed");
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id, CancellationToken ct = default)
        {
            using var resp = await _http.DeleteAsync($"{ApiPath}/{id}", ct);
            if (resp.IsSuccessStatusCode)
                return ApiResult<bool>.Ok(true, resp.StatusCode);

            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<bool>.Fail(resp.StatusCode, problem, error: "Delete failed");
        }
    }
}