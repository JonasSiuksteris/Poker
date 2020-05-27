using System;

namespace Poker.Client.Services
{
    public interface IStateService
    {
        event Action RefreshRequested;
        void CallRequestRefresh();
    }
}
