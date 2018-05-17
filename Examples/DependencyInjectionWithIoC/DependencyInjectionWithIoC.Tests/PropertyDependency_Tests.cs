using System;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DependencyInjectionWithIoC.Fakes;
using Xunit;

namespace DependencyInjectionWithIoC.Tests
{
    public class PropertyDependency_Tests
    {
        private readonly WindsorContainer _container;

        public PropertyDependency_Tests()
        {
            _container = new WindsorContainer();
        }
        [Fact]
        public void RegisterPropertyDependency_Resolve_PropertyNotResolved()
        {
            //Arrange
            this._container.Register(Component.For<PropertyDependency>());
            //Act
            var instance = this._container.Resolve<PropertyDependency>();
           //Assert
           Assert.Null(instance.FooA);
        }

        [Fact]
        public void RegisterPropertyDependency_Resolve_PropertyResolved()
        {
            //Arrange
            this._container.Register(Component.For<FooA>());
            this._container.Register(Component.For<PropertyDependency>());
            //Act
            var instance = this._container.Resolve<PropertyDependency>();
            //Assert
            Assert.NotNull(instance.FooA);
        }
    }
}
