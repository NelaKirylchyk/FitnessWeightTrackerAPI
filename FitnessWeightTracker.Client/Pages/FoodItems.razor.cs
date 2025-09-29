using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using FitnessWeightTracker.Client.Models;
using FitnessWeightTracker.Client.ViewModels;

namespace FitnessWeightTracker.Client.Pages;

public partial class FoodItems : ComponentBase, IDisposable
{
    private FoodItemsViewModel _vm = default!;

    private List<FoodItem> items => _vm.Items;
    private FoodItemDTO editModel => _vm.EditModel;
    private bool isLoading => _vm.IsLoading;
    private bool isEditing => _vm.IsEditing;
    private string? message => _vm.Message;

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _vm = new FoodItemsViewModel(ItemsService, AuthStateProvider);
        await _vm.InitializeAsync();
    }

    private void BeginEdit(FoodItem item)
    {
        _vm.BeginEdit(item);
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

    public void Dispose()
    {
        _vm?.Dispose();
    }
}