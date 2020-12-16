using System;
using System.Threading.Tasks;
using GC.Api.Interfaces;
using GC.Api.Models;
using GC.Backend.Enums;
using GC.Backend.Interfaces;
using GC.Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace GC.Api.Hubs
{
    public class GameHub : Hub
    {
        public readonly IStateProvider _stateProvider;
        public readonly IDeliverMessages _delivery;

        public GameHub(IStateProvider stateProvider, IDeliverMessages delivery)
        {
            _stateProvider = stateProvider;
            _delivery = delivery;
        }

        public async Task SendNewChallenge()
        {
            _stateProvider.ResetState();

            var state = _stateProvider.GetState();

            await _delivery.ChallengeDelivery(state);
        }

        public async Task OnMessageReceived(ReceivedMessage message)
        {
            var connectionId = Context.ConnectionId;
            var currentState = _stateProvider.GetState();

            if (!currentState.IsRoundOpen)
                return;

            var isCorrectAnswer = currentState.Answer == message.Answer;

            if (isCorrectAnswer)
            {
                currentState.IsRoundOpen = false;
                _stateProvider.ChangeState(currentState);

                await RestartChallenge();
            }

            await _delivery.AnswerMessageDelivery(new Answer
            {
                IsRightAnswer = isCorrectAnswer,
                UserScore = isCorrectAnswer ? message.UserScore + 1 : message.UserScore - 1
            }, connectionId);
        }

        public async Task RestartChallenge()
        {
            await Task.Run(() =>
            {
                _delivery.DelayedChallengeDelivery(() =>
                {
                    _stateProvider.ResetState();
                    return _stateProvider.GetState();
                });
            });
        }

        #region counter
        public Task OnClientCounterChange()
        {
            var state = _stateProvider.GetState();

            return Clients.All.SendAsync("ClientCounterChanged", state.Clients.Count);
        }

        public override Task OnConnectedAsync()
        {
            if (_stateProvider.GetClientsCount() > 9)
            {
                Context.Abort();
                return base.OnConnectedAsync();
            }

            _stateProvider.AddClient(Context.ConnectionId);    

            OnClientCounterChange();

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _stateProvider.RemoveClient(Context.ConnectionId);
            OnClientCounterChange();

            return base.OnDisconnectedAsync(exception);
        }
        #endregion
    }
}
