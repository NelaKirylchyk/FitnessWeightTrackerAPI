using System.Linq;
using FitnessWeightTracker.Client.Models;
using FitnessWeightTracker.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace FitnessWeightTracker.Client.ViewModels
{
    public sealed class FoodItemsViewModel : IDisposable
    {
        private readonly FoodItemsService _service;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly CancellationTokenSource _cts = new();

        public List<FoodItem> Items { get; private set; } = new();
        public FoodItemDTO EditModel { get; private set; } = new();
        public bool IsEditing { get; private set; }
        public int EditingId { get; private set; }
        public bool IsLoading { get; private set; } = true;
        public string? Message { get; private set; }

        public FoodItemsViewModel(FoodItemsService service, AuthenticationStateProvider authStateProvider)
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
                Items.Clear();
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
                    Items = result.Value ?? new List<FoodItem>();
                    Sort();
                    Message = null;
                }
                else
                {
                    Message = BuildProblemMessage(result.Problem, result.ErrorMessage ?? "Failed to load items.");
                }
            }
            catch (Exception ex)
            {
                Message = $"Failed to load items: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void BeginEdit(FoodItem item)
        {
            IsEditing = true;
            EditingId = item.Id;
            EditModel = new FoodItemDTO
            {
                Name = item.Name,
                Calories = item.Calories,
                Carbohydrates = item.Carbohydrates,
                Protein = item.Protein,
                Fat = item.Fat,
                ServingSize = item.ServingSize
            };
            Message = null;
        }

        public void CancelEdit()
        {
            IsEditing = false;
            EditingId = 0;
            EditModel = new FoodItemDTO();
            Message = null;
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
                        var idx = Items.FindIndex(i => i.Id == EditingId);
                        if (idx >= 0)
                        {
                            Items[idx].Name = EditModel.Name;
                            Items[idx].Calories = EditModel.Calories;
                            Items[idx].Carbohydrates = EditModel.Carbohydrates;
                            Items[idx].Protein = EditModel.Protein;
                            Items[idx].Fat = EditModel.Fat;
                            Items[idx].ServingSize = EditModel.ServingSize;
                        }

                        var fresh = await _service.GetByIdAsync(EditingId, _cts.Token);
                        if (fresh.Success && fresh.Value is not null)
                            Items[Items.FindIndex(i => i.Id == EditingId)] = fresh.Value;

                        Sort();
                        Message = null;
                        CancelEdit();
                    }
                    else
                    {
                        Message = BuildProblemMessage(result.Problem, result.ErrorMessage ?? "Update failed.");
                    }
                }
                else
                {
                    var result = await _service.AddAsync(EditModel, _cts.Token);
                    if (result.Success && result.Value is not null)
                    {
                        Items.Add(result.Value);
                        Sort();
                        Message = null;
                        CancelEdit();
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
                    Items.RemoveAll(i => i.Id == id);
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

        private void Sort()
        {
            Items = Items.OrderBy(i => i.Name).ThenBy(i => i.Id).ToList();
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