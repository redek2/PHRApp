namespace PHRApp.Models.Enums
{
    public static class EntryStatusExtensions
    {
        public static string ToDisplayString(this EntryStatus status) => status switch
        {
            EntryStatus.Planned => "Zaplanowany",
            EntryStatus.Completed => "Zakończony",
            EntryStatus.Cancelled => "Anulowany",
            _ => status.ToString()
        };
    }
}
