using DependencyInjectionWithIoC.Fakes.Models.Dependencies;

namespace DependencyInjectionWithIoC.Fakes.Models
{
    public class ConstructorDependency
    {
        private readonly FooA _fooA;

        public ConstructorDependency(FooA fooA)
        {
            _fooA = fooA;
        }
    }
}