using System;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DependencyInjectionWithIoC.FakesWithInterfaces;
using DependencyInjectionWithIoC.FakesWithInterfaces.Installers;
using DependencyInjectionWithIoC.FakesWithInterfaces.Models;
using DependencyInjectionWithIoC.FakesWithInterfaces.Models.Dependencies;
using Xunit;

namespace DependencyInjectionWithIoC.Tests
{
    public class ConstructorDependencyWithInterfaces_Tests
    {
        private readonly WindsorContainer _container;

        public ConstructorDependencyWithInterfaces_Tests()
        {
            _container = new WindsorContainer();
        }
        [Fact]
        public void RegisterConstructorDependency_Resolve_ResolutionFailed()
        {
            //Arrange
            this._container.Register(Component.For<ConstructorDependency>());
            //Act
            //Assert
            Assert.Throws<HandlerException>(()=> this._container.Resolve<ConstructorDependency>());
            
        }

        [Fact]
        public void RegisterFooA_ConstructorDependency_Resolve_ResolutionFailed()
        {
            //Arrange
            this._container.Register(Component.For<FooA>());
            this._container.Register(Component.For<ConstructorDependency>());
            //Act
            Assert.Throws<HandlerException>(() => this._container.Resolve<ConstructorDependency>());
        }

        [Fact]
        public void RegisterIFoo_ConstructorDependency_Resolve_Resolved()
        {
            //Arrange
            this._container.Register(Component.For<IFoo, FooA>());
            this._container.Register(Component.For<ConstructorDependency>());
            //Act
            var instance = this._container.Resolve<ConstructorDependency>();
            //Assert
            Assert.IsType<ConstructorDependency>(instance);
            Assert.IsType<FooA>(instance.Foo);
        }

        [Fact]
        public void RegisterWithInstaller_Resolve_Resolved()
        {
            //Arrange
            this._container.Install(new FakesWithInterfacesInstaller());
            //Act
            var instance = this._container.Resolve<ConstructorDependency>();
            //Assert
            Assert.IsType<ConstructorDependency>(instance);
            Assert.IsType<FooA>(instance.Foo);
        }
    }
}
