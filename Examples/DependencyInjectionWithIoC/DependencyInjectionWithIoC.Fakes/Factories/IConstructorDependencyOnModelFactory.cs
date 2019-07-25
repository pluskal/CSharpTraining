using DependencyInjectionWithIoC.Fakes.Models;
using DependencyInjectionWithIoC.Fakes.Models.Dependencies;

namespace DependencyInjectionWithIoC.Fakes.Factories
{
    public interface IConstructorDependencyOnModelFactory
    {
        ConstructorDependencyOnModel Create(Model model);
    }
}