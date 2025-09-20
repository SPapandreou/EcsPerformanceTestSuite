#if UNITY_EDITOR
using Core.EcsWorld;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Core.Editor
{
    public static class EditorWorldCreator
    {
        static EditorWorldCreator()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += DisposeWorld;
            EditorApplication.quitting += DisposeWorld;
            if (World.DefaultGameObjectInjectionWorld == null)
            {
                World.DefaultGameObjectInjectionWorld = new World("EditorWorld");
                World.DefaultGameObjectInjectionWorld.InstallUnitySystems();
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredPlayMode)
            {
                DisposeWorld();
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (World.DefaultGameObjectInjectionWorld != null)
                {
                    Debug.LogWarning("Entering edit mode with existing world: " +
                                     World.DefaultGameObjectInjectionWorld.Name);
                    DisposeWorld();
                }

                World.DefaultGameObjectInjectionWorld = new World("EditorWorld");
            }
        }

        private static void DisposeWorld()
        {
            World.DefaultGameObjectInjectionWorld?.Dispose();
            World.DefaultGameObjectInjectionWorld = null;
        }
        
        
    }
}
#endif