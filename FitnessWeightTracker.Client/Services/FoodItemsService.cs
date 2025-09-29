using System.Net.Http.Json;
using FitnessWeightTracker.Client.Models;

namespace FitnessWeightTracker.Client.Services
{
    public sealed class FoodItemsService
    {
        private readonly HttpClient _http;
        private const string ApiPath = "/api/FoodItems";

        public FoodItemsService(HttpClient http) => _http = http;

        private static async Task<ProblemDetails?> ReadProblemAsync(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: ct); }
            catch { return null; }
        }

        public async Task<ApiResult<List<FoodItem>>> GetAsync(CancellationToken ct = default)
        {
            using var resp = await _http.GetAsync(ApiPath, ct);
            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadFromJsonAsync<List<FoodItem>>(cancellationToken: ct) ?? new();
                return ApiResult<List<FoodItem>>.Ok(data, resp.StatusCode);
            }
            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<List<FoodItem>>.Fail(resp.StatusCode, problem, "Get failed");
        }

        public async Task<ApiResult<FoodItem>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            using var resp = await _http.GetAsync($"{ApiPath}/{id}", ct);
            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadFromJsonAsync<FoodItem>(cancellationToken: ct);
                if (data is not null) return ApiResult<FoodItem>.Ok(data, resp.StatusCode);
            }
            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<FoodItem>.Fail(resp.StatusCode, problem, "GetById failed");
        }

        public async Task<ApiResult<FoodItem>> AddAsync(FoodItemDTO dto, CancellationToken ct = default)
        {
            using var resp = await _http.PostAsJsonAsync(ApiPath, dto, ct);
            if (resp.IsSuccessStatusCode)
            {
                var created = await resp.Content.ReadFromJsonAsync<FoodItem>(cancellationToken: ct);
                if (created is not null) return ApiResult<FoodItem>.Ok(created, resp.StatusCode);
            }
            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<FoodItem>.Fail(resp.StatusCode, problem, "Create failed");
        }

        public async Task<ApiResult<bool>> UpdateAsync(int id, FoodItemDTO dto, CancellationToken ct = default)
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