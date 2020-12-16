using System;
using System.Collections.Generic;
using GC.Backend.Enums;

namespace GC.Backend.Models
{
    public class GameState
    {
        public string Challenge { get; set; }
        public bool IsRoundOpen { get; set; }
        public AnswerType Answer { get; set; }
        public List<string> Clients { get; set; } = new List<string>();
    }
}
