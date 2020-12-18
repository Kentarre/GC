using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using GC.Api.Hubs;
using GC.Api.Models;
using GC.Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace GC.Api.Interfaces
{
    public class MessagesDelivery : IDeliverMessages
    {
        private readonly IHubContext<GameHub> _hubContext;

        public MessagesDelivery(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task AnswerMessageDelivery(Answer answer, string connectionId)
        {
           await _hubContext.Clients.Client(connectionId).SendAsync("OnMessageCheck", answer);
        }

        public async Task ChallengeDelivery(GameState state)
        {
           await _hubContext.Clients.All.SendAsync("NewChallenge", state.Challenge);
        }

        public async Task DelayedChallengeDelivery(Func<GameState> resetState)
        {
            await Task.Delay(5000);

            var state = resetState();
            await ChallengeDelivery(state);
        }

        public async Task UpdateScoreList(List<Client> clients)
        {
            await _hubContext.Clients.All.SendAsync("ScoreChange", clients.OrderByDescending(x => x.Score));
        }

        public async Task SetNickname(string nickname, string connectionId)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("SetNickname", nickname);
        }
    }
}
