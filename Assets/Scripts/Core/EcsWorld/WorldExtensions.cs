using Unity.Entities;

namespace Core
{
    public static class WorldExtensions
    {
        public static void InstallUnitySystems(this World world)
        {
            var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
        }
    }
}