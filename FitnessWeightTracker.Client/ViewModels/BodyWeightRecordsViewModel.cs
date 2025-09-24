using System.Linq;
using FitnessWeightTracker.Client.Models;
using FitnessWeightTracker.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace FitnessWeightTracker.Client.ViewModels
{
    public sealed class BodyWeightRecordsViewModel : IDisposable
    {
        private readonly BodyWeightRecordsService _service;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly CancellationTokenSource _cts = new();

        public List<BodyWeightRecord> Records { get; private set; } = new();
        public BodyWeightRecordDTO EditModel { get; private set; } = new() { Date = DateTime.Today };
        public bool IsEditing { get; private set; }
        public int EditingId { get; private set; }
        public bool IsLoading { get; private set; } = true;
        public string? Message { get; private set; }

        public BodyWeightRecordsViewModel(BodyWeightRecordsService service, AuthenticationStateProvider authStateProvider)
        {
            _service = service;
            _authStateProvider = authStateProvider;
        }

        public async Task InitializeAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            if (authState.User?.Identity is null || !authState.User.Identity.IsAuthenticated)
            {
                IsLoading = false;
                Records.Clear();
                Message = null;
                return;
            }

            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                var result = await _service.GetAsync(ascendingOrder: false, _cts.Token);
                if (result.Success)
                {
                    Records = result.Value ?? new List<BodyWeightRecord>();
                    // Keep Message unchanged; CRUD success clears it explicitly.
                }
                else
                {
                    Message = BuildProblemMessage(result.Problem, result.ErrorMessage ?? "Failed to load records.");
                }
            }
            catch (Exception ex)
            {
                Message = $"Failed to load records: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void BeginEdit(BodyWeightRecord r)
        {
            IsEditing = true;
            EditingId = r.Id;
            EditModel = new BodyWeightRecordDTO { Date = r.Date, Weight = r.Weight };
        }

        public void CancelEdit()
        {
            IsEditing = false;
            EditingId = 0;
            EditModel = new BodyWeightRecordDTO { Date = DateTime.Today };
        }

        public async Task SaveAsync()
        {
            try
            {
                if (IsEditing)
                {
                    var result = await _service.UpdateAsync(EditingId, EditModel, _cts.Token);
                    if (result.Success)
                    {
                        Message = null;
                        CancelEdit();
                        await LoadAsync();
                    }
                    else
                    {
                        Message = BuildProblemMessage(result.Problem, result.ErrorMessage ?? "Update failed.");
                    }
                }
                else
                {
                    var result = await _service.AddAsync(EditModel, _cts.Token);
                    if (result.Success)
                    {
                        Message = null;
                        CancelEdit();
                        await LoadAsync();
                    }
                    else
                    {
                        Message = BuildProblemMessage(result.Problem, result.ErrorMessage ?? "Create failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Save failed: {ex.Message}";
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id, _cts.Token);
                if (result.Success)
                {
                    Message = null;
                    await LoadAsync();
                }
                else
                {
                    Message = BuildProblemMessage(result.Problem, result.ErrorMessage ?? "Delete failed.");
                }
            }
            catch (Exception ex)
            {
                Message = $"Delete failed: {ex.Message}";
            }
        }

        private static string BuildProblemMessage(ProblemDetails? problem, string fallback)
        {
            if (problem is null) return fallback;

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(problem.Title)) parts.Add(problem.Title!);
            if (!string.IsNullOrWhiteSpace(problem.Detail)) parts.Add(problem.Detail!);

            if (problem.Errors is { Count: > 0 })
            {
                parts.AddRange(problem.Errors.SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}: {v}")));
            }

            return parts.Count > 0 ? string.Join(Environment.NewLine, parts) : fallback;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}