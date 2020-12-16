using System;
using GC.Backend.Models;

namespace GC.Backend.Interfaces
{
    public interface IStateProvider
    {
        GameState GetState();
        void ResetState();
        void ChangeState(GameState state);
        GameState CreateChallenge();
        void AddClient(string id);
        void RemoveClient(string id);
        int GetClientsCount();
    }
}
