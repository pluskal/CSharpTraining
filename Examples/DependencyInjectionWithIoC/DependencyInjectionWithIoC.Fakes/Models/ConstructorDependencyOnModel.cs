using DependencyInjectionWithIoC.Fakes.Models.Dependencies;

namespace DependencyInjectionWithIoC.Fakes.Models
{
    public class ConstructorDependencyOnModel
    {
        private readonly FooA _fooA;

        public ConstructorDependencyOnModel(FooA fooA, Model model)
        {
            _fooA = fooA;
        }
    }
}