using System;
using Latios;
using Unity.Entities;
using UnityEngine;

namespace Core
{
    public class WorldContainer : IDisposable
    {
        public LatiosWorld World { get; }

        public WorldContainer(string worldName)
        {
            // workaround for unity memory leak
            DefaultWorldInitialization.Initialize("temp", true).Dispose();
            World = new LatiosWorld(worldName, WorldFlags.Game | WorldFlags.Editor);
            
        }

        public void InstallUnitySystems()
        {
            World.InstallUnitySystems();
        }

        public void InstallKinemation()
        {
            Latios.Transforms.TransformsBootstrap.InstallTransforms(World, World.simulationSystemGroup);
            Latios.Kinemation.KinemationBootstrap.InstallKinemation(World);
        }

        public void UpdateWorld()
        {
            World.initializationSystemGroup.SortSystems();
            World.simulationSystemGroup.SortSystems();
            World.presentationSystemGroup.SortSystems();
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(World);
        }

        public void Dispose()
        {
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(World);
            World.DestroyAllSystemsAndLogException(out _);
            if (!World.IsCreated)
                return;
            World.Dispose();
        }
    }
}