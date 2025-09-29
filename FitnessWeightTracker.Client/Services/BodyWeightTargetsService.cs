using System.Net.Http.Json;
using FitnessWeightTracker.Client.Models;

namespace FitnessWeightTracker.Client.Services
{
    public sealed class BodyWeightTargetsService
    {
        private readonly HttpClient _http;
        private const string ApiPath = "/api/BodyWeightTargets";

        public BodyWeightTargetsService(HttpClient http)
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

        // Returns the current user's target (or 404 if none)
        public async Task<ApiResult<BodyWeightTarget>> GetAsync(CancellationToken ct = default)
        {
            using var resp = await _http.GetAsync(ApiPath, ct);
            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadFromJsonAsync<BodyWeightTarget>(cancellationToken: ct);
                if (data is not null)
                    return ApiResult<BodyWeightTarget>.Ok(data, resp.StatusCode);
            }
            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<BodyWeightTarget>.Fail(resp.StatusCode, problem, "Get target failed");
        }

        public async Task<ApiResult<BodyWeightTarget>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            using var resp = await _http.GetAsync($"{ApiPath}/{id}", ct);
            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadFromJsonAsync<BodyWeightTarget>(cancellationToken: ct);
                if (data is not null)
                    return ApiResult<BodyWeightTarget>.Ok(data, resp.StatusCode);
            }
            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<BodyWeightTarget>.Fail(resp.StatusCode, problem, "GetById failed");
        }

        public async Task<ApiResult<BodyWeightTarget>> AddAsync(BodyWeightTargetDTO dto, CancellationToken ct = default)
        {
            using var resp = await _http.PostAsJsonAsync(ApiPath, dto, ct);
            if (resp.IsSuccessStatusCode)
            {
                var created = await resp.Content.ReadFromJsonAsync<BodyWeightTarget>(cancellationToken: ct);
                if (created is not null)
                    return ApiResult<BodyWeightTarget>.Ok(created, resp.StatusCode);
            }
            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<BodyWeightTarget>.Fail(resp.StatusCode, problem, "Create failed");
        }

        public async Task<ApiResult<bool>> UpdateAsync(int id, BodyWeightTargetDTO dto, CancellationToken ct = default)
        {
            using var resp = await _http.PutAsJsonAsync($"{ApiPath}/{id}", dto, ct);
            if (resp.IsSuccessStatusCode)
                return ApiResult<bool>.Ok(true, resp.StatusCode);

            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<bool>.Fail(resp.StatusCode, problem, "Update failed");
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id, CancellationToken ct = default)
        {
            using var resp = await _http.DeleteAsync($"{ApiPath}/{id}", ct);
            if (resp.IsSuccessStatusCode)
                return ApiResult<bool>.Ok(true, resp.StatusCode);

            var problem = await ReadProblemAsync(resp, ct);
            return ApiResult<bool>.Fail(resp.StatusCode, problem, "Delete failed");
        }
    }
}