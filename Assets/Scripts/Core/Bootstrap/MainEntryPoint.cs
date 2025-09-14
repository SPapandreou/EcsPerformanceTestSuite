using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading;
using Core.Tests;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public class MainEntryPoint : IAsyncStartable
    {
        private readonly TestManager _testManager;
        
        public MainEntryPoint(TestManager testManager)
        {
            _testManager = testManager;
        }
        
        public void Start()
        {
            
        }

        private async UniTask RunInteractive()
        {
        }

        public UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}