using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace DependencyInjectionWithIoC.FakesWithInterfaces.Installers
{
    public class FakesWithInterfacesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //Manual installation
            //container.Register(Component.For<ConstructorDependency>());
            //container.Register(Classes.FromThisAssembly().BasedOn<IFoo>().WithServiceDefaultInterfaces());

            //All components with Default interfaces
            container.Register(Classes.FromThisAssembly().Pick().WithServiceDefaultInterfaces());
        }
    }
}