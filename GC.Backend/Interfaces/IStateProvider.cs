using System;
using System.Collections.Generic;
using GC.Backend.Models;

namespace GC.Backend.Interfaces
{
    public interface IStateProvider
    {
        GameState GetState();
        void ResetState();
        void ChangeState(GameState state);
        GameState CreateChallenge();
        void AddClient(Client client);
        void RemoveClient(string id);
        int GetClientsCount();
        List<Client> GetClients();
    }
}
