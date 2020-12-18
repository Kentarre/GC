using System;
using System.Threading.Tasks;

namespace GC.Backend.Interfaces
{
    public interface IGenerateNicknames
    {
        Task<string> GenerateAsync();
    }
}
