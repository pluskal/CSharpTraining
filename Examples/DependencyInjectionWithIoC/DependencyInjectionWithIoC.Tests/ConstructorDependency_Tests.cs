using System;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DependencyInjectionWithIoC.Fakes;
using Xunit;

namespace DependencyInjectionWithIoC.Tests
{
    public class ConstructorDependency_Tests
    {
        private readonly WindsorContainer _container;

        public ConstructorDependency_Tests()
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
        public void RegisterFooA_ConstructorDependency_Resolve_Resolved()
        {
            //Arrange
            this._container.Register(Component.For<FooA>());
            this._container.Register(Component.For<ConstructorDependency>());
            //Act
            var instance = this._container.Resolve<ConstructorDependency>();
            //Assert
            Assert.IsType<ConstructorDependency>(instance);
        }
    }
}
