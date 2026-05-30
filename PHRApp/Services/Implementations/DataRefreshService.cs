using PHRApp.Services.Interfaces;

namespace PHRApp.Services.Implementations
{
    public class DataRefreshService : IDataRefreshService
    {
        public event Action? EntriesChanged;

        public void NotifyEntriesChanged()
        {
            EntriesChanged?.Invoke();
        }
    }
}