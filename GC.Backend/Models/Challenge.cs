using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GC.Backend.Enums;

namespace GC.Backend.Models
{
    public class Challenge
    {
        public int A { get; internal set; }
        public int B { get; internal set; }
        public char Operation { get; internal set; }
        public double Answer { get; internal set; }
        public AnswerType AnswerType { get; internal set; }

        public List<char> Operators => new List<char> { '+', '-', '/', '*' };
    }

    public static class ChallengeGenerator
    {
        private static readonly Random _rnd = new Random();

        public static Challenge GetChallenge()
        {
            var challenge = new Challenge
            {
                A = _rnd.Next(1, 10),
                B = _rnd.Next(1, 10)
            };

            challenge.Operation = challenge.Operators[_rnd.Next(0, challenge.Operators.Count - 1)];

            challenge.AnswerType = _rnd.Next(1, 100) % 2 == 0
                ? AnswerType.Right
                : AnswerType.Wrong;

            challenge.Answer = challenge.AnswerType == AnswerType.Right
                ? GetChallngeValue(challenge.A, challenge.B, challenge.Operation)
                : _rnd.Next(1, 100);

            return challenge;
        }

        private static double GetChallngeValue(int a, int b, char operation)
        {
            switch (operation) {
                case '+':
                    return a + b;
                case '-':
                    return a - b;
                case '*':
                    return a * b;
                case '/':
                    return a / b;
                default:
                    throw new InvalidOperationException();
            }

        }
    }
}
