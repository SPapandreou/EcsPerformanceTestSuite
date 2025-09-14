using Latios;
using Latios.Authoring;
using Unity.Entities;

namespace Settings.LatiosSettings
{
    [UnityEngine.Scripting.Preserve]
    public class LatiosBakingBootstrap : ICustomBakingBootstrap
    {
        public void InitializeBakingForAllWorlds(ref CustomBakingBootstrapContext context)
        {
            Latios.Transforms.Authoring.TransformsBakingBootstrap.InstallLatiosTransformsBakers(ref context);
            Latios.Kinemation.Authoring.KinemationBakingBootstrap.InstallKinemation(ref context);
        }
    }

    [UnityEngine.Scripting.Preserve]
    public class LatiosEditorBootstrap : ICustomEditorBootstrap
    {
        public World Initialize(string defaultEditorWorldName)
        {
            var world = new LatiosWorld(defaultEditorWorldName, WorldFlags.Editor);

            var systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default, true);
            BootstrapTools.InjectUnitySystems(systems, world, world.simulationSystemGroup);

            Latios.Transforms.TransformsBootstrap.InstallTransforms(world, world.simulationSystemGroup);
            Latios.Kinemation.KinemationBootstrap.InstallKinemation(world);

            return world;
        }
    }
}