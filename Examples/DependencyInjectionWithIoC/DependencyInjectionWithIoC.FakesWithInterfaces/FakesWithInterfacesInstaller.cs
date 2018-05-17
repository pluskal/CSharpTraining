using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Diagnostics.Extensions;
using Castle.Windsor.Installer;

namespace DependencyInjectionWithIoC.FakesWithInterfaces
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