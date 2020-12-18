using System;
using System.Collections.Generic;
using System.Linq;
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
        public readonly IGenerateNicknames _nicknameGenerator;

        public GameHub(IStateProvider stateProvider, IDeliverMessages delivery, IGenerateNicknames generator)
        {
            _stateProvider = stateProvider;
            _delivery = delivery;
            _nicknameGenerator = generator;
        }

        public async Task SendNewChallenge()
        {
            _stateProvider.ResetState();

            var state = _stateProvider.GetState();

            await _delivery.ChallengeDelivery(state);
        }

        public async Task OnMessageReceived(ReceivedMessage message)
        {
            var currentState = _stateProvider.GetState();
            var client = currentState.Clients.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();

            if (!currentState.IsRoundOpen)
                return;

            var isCorrectAnswer = currentState.Answer == message.Answer;

            if (isCorrectAnswer)
            {
                client.Score = isCorrectAnswer
                    ? message.UserScore + 1
                    : message.UserScore - 1;

                currentState.Clients[currentState.Clients.FindIndex(x => x.ConnectionId == Context.ConnectionId)] = client;
                currentState.IsRoundOpen = false;

                _stateProvider.ChangeState(currentState);

                SetScoreList(currentState.Clients);

                await RestartChallenge();
            }

            await _delivery.AnswerMessageDelivery(new Answer
            {
                IsRightAnswer = isCorrectAnswer,
                UserScore = client.Score,
            }, client.ConnectionId);
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

        public void SetScoreList(List<Client> clients)
        {
            _delivery.UpdateScoreList(clients);
        }

        #region counter
        public Task OnClientDisconnect(string connectionId)
        {
            return Clients.All.SendAsync("ClientDisconnected", connectionId);
        }

        public override Task OnConnectedAsync()
        {
            var currentState = _stateProvider.GetState();

            if (_stateProvider.GetClientsCount() > 9)
            {
                Context.Abort();
                return base.OnConnectedAsync();
            }

            var nickname = _nicknameGenerator.GenerateAsync()
                .GetAwaiter()
                .GetResult();

            _stateProvider.AddClient(new Client {
                ConnectionId = Context.ConnectionId,
                Score = 0,
                Nickname = nickname
            });

            SetScoreList(currentState.Clients);
            SetNickname(nickname, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _stateProvider.RemoveClient(Context.ConnectionId);
            OnClientDisconnect(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public void SetNickname(string nickname, string connectionId)
        {
            _delivery.SetNickname(nickname, connectionId);
        }
        #endregion
    }
}
