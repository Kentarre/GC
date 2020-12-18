using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GC.Api.Models;
using GC.Backend.Models;

namespace GC.Api.Interfaces
{
    public interface IDeliverMessages
    {
        Task DelayedChallengeDelivery(Func<GameState> resetState);
        Task ChallengeDelivery(GameState state);
        Task AnswerMessageDelivery(Answer answer, string connectionId);
        Task UpdateScoreList(List<Client> clients);
        Task SetNickname(string nickname, string connectionId);
    }
}
