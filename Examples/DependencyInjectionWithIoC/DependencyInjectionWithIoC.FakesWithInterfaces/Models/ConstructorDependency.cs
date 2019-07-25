using DependencyInjectionWithIoC.FakesWithInterfaces.Models.Dependencies;

namespace DependencyInjectionWithIoC.FakesWithInterfaces.Models
{
    public class ConstructorDependency
    {
        public IFoo Foo { get; }

        public ConstructorDependency(IFoo foo)
        {
            Foo = foo;
        }
    }
}