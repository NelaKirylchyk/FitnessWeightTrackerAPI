using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using FitnessWeightTracker.Client.Models;
using FitnessWeightTracker.Client.ViewModels;

namespace FitnessWeightTracker.Client.Pages;

public partial class BodyWeightTargets : ComponentBase, IDisposable
{
    private BodyWeightTargetsViewModel _vm = default!;

    private BodyWeightTarget? current => _vm.Current;
    private BodyWeightTargetDTO editModel => _vm.EditModel;
    private bool isLoading => _vm.IsLoading;
    private bool isEditing => _vm.IsEditing;
    private string? message => _vm.Message;

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _vm = new BodyWeightTargetsViewModel(TargetsService, AuthStateProvider);
        await _vm.InitializeAsync();
    }

    private void BeginCreate()
    {
        _vm.BeginCreate();
        StateHasChanged();
    }

    private void BeginEdit()
    {
        _vm.BeginEdit();
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

    private async Task DeleteAsync()
    {
        await _vm.DeleteAsync();
        StateHasChanged();
    }

    public void Dispose()
    {
        _vm?.Dispose();
    }
}