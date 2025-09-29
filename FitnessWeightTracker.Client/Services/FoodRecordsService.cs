using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using FitnessWeightTracker.Client.Models;

namespace FitnessWeightTracker.Client.Services
{
    public sealed class FoodRecordsService
    {
        private readonly HttpClient _http;
        private const string ApiPath = "/api/FoodRecords";

        public FoodRecordsService(HttpClient http) => _http = http;

        private static async Task<ProblemDetails?> ReadProblemAsync(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: ct); }
            catch { return null; }
        }

        public async Task<ApiResult<List<FoodRecord>>> GetAsync(bool ascendingOrder, CancellationToken ct = default)
        {
            var url = QueryHelpers.AddQueryString(ApiPath, "ascendingOrder", ascendingOrder ? "true" : "false");
            using var resp = await _http.GetAsync(url, ct);
            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadFromJsonAsync<List<FoodRecord>>(cancellationToken: ct) ?? new();
                return ApiResult<List<FoodRecord>>.Ok(data, resp.StatusCode);
            }
            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<List<FoodRecord>>.Fail(resp.StatusCode, problem, $"GET {url} failed");
        }

        public async Task<ApiResult<FoodRecord>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            using var resp = await _http.GetAsync($"{ApiPath}/{id}", ct);
            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadFromJsonAsync<FoodRecord>(cancellationToken: ct);
                if (data is not null) return ApiResult<FoodRecord>.Ok(data, resp.StatusCode);
            }
            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<FoodRecord>.Fail(resp.StatusCode, problem, "GetById failed");
        }

        public async Task<ApiResult<FoodRecord>> AddAsync(FoodRecordDTO dto, CancellationToken ct = default)
        {
            using var resp = await _http.PostAsJsonAsync(ApiPath, dto, ct);
            if (resp.IsSuccessStatusCode)
            {
                var created = await resp.Content.ReadFromJsonAsync<FoodRecord>(cancellationToken: ct);
                if (created is not null) return ApiResult<FoodRecord>.Ok(created, resp.StatusCode);
            }
            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<FoodRecord>.Fail(resp.StatusCode, problem, "Create failed");
        }

        public async Task<ApiResult<bool>> UpdateAsync(int id, FoodRecordDTO dto, CancellationToken ct = default)
        {
            using var resp = await _http.PutAsJsonAsync($"{ApiPath}/{id}", dto, ct);
            if (resp.IsSuccessStatusCode) return ApiResult<bool>.Ok(true, resp.StatusCode);

            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<bool>.Fail(resp.StatusCode, problem, "Update failed");
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id, CancellationToken ct = default)
        {
            using var resp = await _http.DeleteAsync($"{ApiPath}/{id}", ct);
            if (resp.IsSuccessStatusCode) return ApiResult<bool>.Ok(true, resp.StatusCode);

            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<bool>.Fail(resp.StatusCode, problem, "Delete failed");
        }
    }
}