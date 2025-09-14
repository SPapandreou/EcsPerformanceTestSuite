using Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PhysicsTest.OOP
{
    public class TestScope : LifetimeScope
    {
        public GameObject sphere;
        public GameObject cube;
        public GameObject capsule;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var primitives = new GameObject[4];
            primitives[(int)PrimitiveShape.Sphere] = sphere;
            primitives[(int)PrimitiveShape.Cube] = cube;
            primitives[(int)PrimitiveShape.Capsule] = capsule;
            
            builder.RegisterInstance(primitives);

            builder.RegisterEntryPoint<TestLogic>();
        }
    }
}