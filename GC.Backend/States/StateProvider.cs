using System;
using GC.Backend.Enums;
using GC.Backend.Interfaces;
using GC.Backend.Models;

namespace GC.Backend.States
{
    public class StateProvider : IStateProvider
    {
        private static GameState _state = new GameState();

        public GameState GetState() => _state;
        public void ChangeState(GameState state) => _state = state;
        public void ResetState() => _state = CreateChallenge();

        public GameState CreateChallenge()
        {
            var challenge = ChallengeGenerator.GetChallenge();

            return new GameState
            {
                Challenge = $"{challenge.A} {challenge.Operation} {challenge.B} = {challenge.Answer}",
                Clients = _state.Clients,
                IsRoundOpen = true,
                Answer = challenge.AnswerType
            };
        }

        public void AddClient(string id) => _state.Clients.Add(id);
        public void RemoveClient(string id) => _state.Clients.RemoveAll(x => x.Equals(id));
        public int GetClientsCount() => _state.Clients.Count;
    }
}
