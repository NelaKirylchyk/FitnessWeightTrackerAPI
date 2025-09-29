using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using FitnessWeightTracker.Client.Models;
using FitnessWeightTracker.Client.ViewModels;

namespace FitnessWeightTracker.Client.Pages;

public partial class FoodRecords : ComponentBase, IDisposable
{
    private FoodRecordsViewModel _vm = default!;
    private List<FoodItem> items => _vm.Items;
    private List<FoodRecord> records => _vm.Records;
    private FoodRecordDTO editModel => _vm.EditModel;
    private bool isLoading => _vm.IsLoading;
    private bool isEditing => _vm.IsEditing;
    private bool ascending => _vm.Ascending;
    private string? message => _vm.Message;

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _vm = new FoodRecordsViewModel(RecordsService, ItemsService, AuthStateProvider);
        await _vm.InitializeAsync();
    }

    private void BeginEdit(FoodRecord r)
    {
        _vm.BeginEdit(r);
        StateHasChanged();
    }

    private void CancelEdit()
    {
        _vm.CancelEdit();
        StateHasChanged();
    }

    private async Task SaveAsync()
    {
        await _vm.SaveAsync();
        StateHasChanged();
    }

    private async Task DeleteAsync(int id)
    {
        await _vm.DeleteAsync(id);
        StateHasChanged();
    }

    private async void ToggleOrder(ChangeEventArgs _)
    {
        await _vm.ToggleOrderAsync();
        StateHasChanged();
    }

    public void Dispose()
    {
        _vm?.Dispose();
    }
}