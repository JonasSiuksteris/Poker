using System;

namespace Poker.Client.Services
{
    public class StateService : IStateService
    {
        public event Action RefreshRequested;
        public void CallRequestRefresh()
        {
            RefreshRequested?.Invoke();
        }
    }
}
