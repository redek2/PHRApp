namespace PHRApp.Services.Interfaces
{
    public interface IDataRefreshService
    {
        event Action? EntriesChanged;
        void NotifyEntriesChanged();
    }
}
