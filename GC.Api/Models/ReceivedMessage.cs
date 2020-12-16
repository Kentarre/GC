using System;
using GC.Backend.Enums;

namespace GC.Api.Models
{
    public class ReceivedMessage
    {
        public AnswerType Answer { get; set; }
        public int UserScore { get; set; }
    }
}
