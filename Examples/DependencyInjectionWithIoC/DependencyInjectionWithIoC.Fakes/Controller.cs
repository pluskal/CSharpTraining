using DependencyInjectionWithIoC.Fakes.Factories;
using DependencyInjectionWithIoC.Fakes.Models;
using DependencyInjectionWithIoC.Fakes.Models.Dependencies;

namespace DependencyInjectionWithIoC.Fakes
{
    public class Controller
    {
        private readonly IConstructorDependencyOnModelFactory _constructorDependencyOnModelFactory;
        public ConstructorDependencyOnModel DependencyOnModel { get; private set; }

        public Controller(ConstructorDependencyOnModel dependencyOnModel)
        {
            DependencyOnModel = dependencyOnModel;
        }

        public Controller(IConstructorDependencyOnModelFactory constructorDependencyOnModelFactory)
        {
            _constructorDependencyOnModelFactory = constructorDependencyOnModelFactory;
        }

        public void CreateConstructorDependencyOnModel(Model model)
        {
            DependencyOnModel = _constructorDependencyOnModelFactory.Create(model);
        }
    }
}