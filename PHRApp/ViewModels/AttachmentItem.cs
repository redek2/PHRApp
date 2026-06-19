namespace PHRApp.ViewModels
{
    public class AttachmentItem
    {
        public int? ExistingId { get; init; }
        public string? NewFilePath { get; init; }
        public string DisplayName { get; init; } = null!;

        public bool IsNew => ExistingId == null;
    }
}
