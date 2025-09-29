using System.Linq;
using FitnessWeightTracker.Client.Models;
using FitnessWeightTracker.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace FitnessWeightTracker.Client.ViewModels
{
    public sealed class FoodRecordsViewModel : IDisposable
    {
        private readonly FoodRecordsService _recordsService;
        private readonly FoodItemsService _itemsService;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly CancellationTokenSource _cts = new();

        public List<FoodItem> Items { get; private set; } = new();
        public List<FoodRecord> Records { get; private set; } = new();
        public FoodRecordDTO EditModel { get; private set; } = new() { ConsumptionDate = DateTime.Today };
        public bool IsEditing { get; private set; }
        public int EditingId { get; private set; }
        public bool Ascending { get; private set; } = false; // newest first by default
        public bool IsLoading { get; private set; } = true;
        public string? Message { get; private set; }

        public FoodRecordsViewModel(
            FoodRecordsService recordsService,
            FoodItemsService itemsService,
            AuthenticationStateProvider authStateProvider)
        {
            _recordsService = recordsService;
            _itemsService = itemsService;
            _authStateProvider = authStateProvider;
        }

        public async Task InitializeAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            if (authState.User?.Identity is null || !authState.User.Identity.IsAuthenticated)
            {
                IsLoading = false;
                Items.Clear();
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
                Message = null;

                // Load selectable items
                var itemsResult = await _itemsService.GetAsync(_cts.Token);
                if (itemsResult.Success)
                    Items = itemsResult.Value ?? new();

                // Load records
                var recResult = await _recordsService.GetAsync(Ascending, _cts.Token);
                if (recResult.Success)
                {
                    Records = recResult.Value ?? new();
                    SortLocal();
                }
                else
                {
                    Message = BuildProblemMessage(recResult.Problem, recResult.ErrorMessage ?? "Failed to load records.");
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

        public void BeginEdit(FoodRecord r)
        {
            IsEditing = true;
            EditingId = r.Id;
            EditModel = new FoodRecordDTO
            {
                FoodItemId = r.FoodItemId,
                Quantity = r.Quantity,
                ConsumptionDate = r.ConsumptionDate
            };
            Message = null;
        }

        public void CancelEdit()
        {
            IsEditing = false;
            EditingId = 0;
            EditModel = new FoodRecordDTO { ConsumptionDate = DateTime.Today };
            Message = null;
        }

        public async Task SaveAsync()
        {
            try
            {
                if (IsEditing)
                {
                    var result = await _recordsService.UpdateAsync(EditingId, EditModel, _cts.Token);
                    if (result.Success)
                    {
                        // Optimistic local update
                        var idx = Records.FindIndex(x => x.Id == EditingId);
                        if (idx >= 0)
                        {
                            Records[idx].FoodItemId = EditModel.FoodItemId;
                            Records[idx].Quantity = EditModel.Quantity;
                            Records[idx].ConsumptionDate = EditModel.ConsumptionDate;
                            Records[idx].FoodItem = Items.FirstOrDefault(i => i.Id == EditModel.FoodItemId);
                        }

                        // Optional canonical refresh
                        var refreshed = await _recordsService.GetByIdAsync(EditingId, _cts.Token);
                        if (refreshed.Success && refreshed.Value is not null)
                            Records[Records.FindIndex(r => r.Id == EditingId)] = refreshed.Value;

                        SortLocal();
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
                    var result = await _recordsService.AddAsync(EditModel, _cts.Token);
                    if (result.Success && result.Value is not null)
                    {
                        result.Value.FoodItem ??= Items.FirstOrDefault(i => i.Id == result.Value.FoodItemId);
                        Records.Add(result.Value);
                        SortLocal();
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
                var result = await _recordsService.DeleteAsync(id, _cts.Token);
                if (result.Success)
                {
                    Records.RemoveAll(r => r.Id == id);
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

        public async Task ToggleOrderAsync()
        {
            Ascending = !Ascending;
            await ReloadWithOrderAsync();
        }

        public async Task ReloadWithOrderAsync()
        {
            var recResult = await _recordsService.GetAsync(Ascending, _cts.Token);
            if (recResult.Success)
            {
                Records = recResult.Value ?? new();
                SortLocal();
            }
            else
            {
                Message = BuildProblemMessage(recResult.Problem, recResult.ErrorMessage ?? "Failed to load records.");
            }
        }

        private void SortLocal()
        {
            if (Ascending)
                Records = Records.OrderBy(r => r.ConsumptionDate).ThenBy(r => r.Id).ToList();
            else
                Records = Records.OrderByDescending(r => r.ConsumptionDate).ThenByDescending(r => r.Id).ToList();
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