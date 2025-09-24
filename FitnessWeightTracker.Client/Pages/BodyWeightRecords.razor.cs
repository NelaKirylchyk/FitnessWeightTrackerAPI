using Microsoft.AspNetCore.Components;
using FitnessWeightTracker.Client.Models;
using FitnessWeightTracker.Client.ViewModels;

namespace FitnessWeightTracker.Client.Pages
{
    public partial class BodyWeightRecords : IDisposable
    {
        [Inject] private BodyWeightRecordsViewModelFactory CreateVM { get; set; } = default!;
        private BodyWeightRecordsViewModel VM = default!;

        // Expose properties used by the .razor markup
        private List<BodyWeightRecord> Records => VM.Records;
        private BodyWeightRecordDTO EditModel => VM.EditModel;
        private bool IsEditing => VM.IsEditing;
        private int EditingId => VM.EditingId;
        private bool IsLoading => VM.IsLoading;
        private string? Message => VM.Message;

        protected override async Task OnInitializedAsync()
        {
            VM = CreateVM();
            await VM.InitializeAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task LoadAsync()
        {
            await VM.LoadAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void BeginEdit(BodyWeightRecord r) => VM.BeginEdit(r);

        private void CancelEdit() => VM.CancelEdit();

        private async Task SaveAsync()
        {
            await VM.SaveAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DeleteAsync(int id)
        {
            await VM.DeleteAsync(id);
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose() => VM.Dispose();
    }
}