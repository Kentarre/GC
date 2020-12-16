using System;
namespace GC.Api.Models
{
    public class Answer
    {
        public bool IsRightAnswer { get; set; }
        public bool IsRoundOpen { get; set; }
        public int UserScore { get; set; }
    }
}
