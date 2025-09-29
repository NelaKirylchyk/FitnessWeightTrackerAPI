using System.Linq;
using FitnessWeightTracker.Client.Models;
using FitnessWeightTracker.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace FitnessWeightTracker.Client.ViewModels
{
    public sealed class BodyWeightTargetsViewModel : IDisposable
    {
        private readonly BodyWeightTargetsService _service;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly CancellationTokenSource _cts = new();

        public BodyWeightTarget? Current { get; private set; }
        public BodyWeightTargetDTO EditModel { get; private set; } = new() { TargetDate = DateTime.Today.AddDays(1) };
        public bool IsEditing { get; private set; }
        public bool IsLoading { get; private set; } = true;
        public string? Message { get; private set; }

        public BodyWeightTargetsViewModel(BodyWeightTargetsService service, AuthenticationStateProvider authStateProvider)
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
                Current = null;
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
                var result = await _service.GetAsync(_cts.Token);
                if (result.Success)
                {
                    Current = result.Value!;
                }
                else if ((int)result.StatusCode == 404)
                {
                    Current = null;
                }
                else
                {
                    Message = BuildProblemMessage(result.Problem, result.ErrorMessage ?? "Failed to load target.");
                }
            }
            catch (Exception ex)
            {
                Message = $"Failed to load target: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void BeginCreate()
        {
            EditModel = new BodyWeightTargetDTO
            {
                TargetWeight = 70f,
                TargetDate = DateTime.Today.AddDays(1)
            };
            IsEditing = true;
            Message = null;
        }

        public void BeginEdit()
        {
            if (Current is null) return;

            EditModel = new BodyWeightTargetDTO
            {
                TargetWeight = Current.TargetWeight,
                TargetDate = Current.TargetDate
            };
            IsEditing = true;
            Message = null;
        }

        public void CancelEdit()
        {
            IsEditing = false;
            EditModel = new BodyWeightTargetDTO { TargetDate = DateTime.Today.AddDays(1) };
        }

        public async Task SaveAsync()
        {
            try
            {
                if (Current is null)
                {
                    var result = await _service.AddAsync(EditModel, _cts.Token);
                    if (result.Success && result.Value is not null)
                    {
                        Current = result.Value;
                        IsEditing = false;
                        Message = null;
                    }
                    else
                    {
                        Message = BuildProblemMessage(result.Problem, result.ErrorMessage ?? "Create failed.");
                    }
                }
                else
                {
                    var result = await _service.UpdateAsync(Current.Id, EditModel, _cts.Token);
                    if (result.Success)
                    {
                        // Try to refresh with canonical version
                        var refreshed = await _service.GetByIdAsync(Current.Id, _cts.Token);
                        if (refreshed.Success && refreshed.Value is not null)
                        {
                            Current = refreshed.Value;
                        }
                        else
                        {
                            Current.TargetWeight = EditModel.TargetWeight;
                            Current.TargetDate = EditModel.TargetDate;
                        }
                        IsEditing = false;
                        Message = null;
                    }
                    else
                    {
                        Message = BuildProblemMessage(result.Problem, result.ErrorMessage ?? "Update failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Save failed: {ex.Message}";
            }
        }

        public async Task DeleteAsync()
        {
            if (Current is null) return;

            try
            {
                var result = await _service.DeleteAsync(Current.Id, _cts.Token);
                if (result.Success)
                {
                    Current = null;
                    IsEditing = false;
                    Message = null;
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