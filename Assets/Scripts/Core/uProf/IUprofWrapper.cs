using System;
using Core.Tests;
using Cysharp.Threading.Tasks;

namespace Core.uProf
{
    public interface IUprofWrapper : IDisposable
    {
        UniTask StartProfiling();
        UniTask StopProfiling(string outputDirectory, TestResults testResults);
    }
}