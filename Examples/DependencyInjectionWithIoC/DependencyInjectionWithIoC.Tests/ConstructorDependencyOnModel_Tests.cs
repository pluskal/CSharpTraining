using System;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DependencyInjectionWithIoC.Fakes;
using Xunit;

namespace DependencyInjectionWithIoC.Tests
{
    public class ConstructorDependencyOnModel_Tests
    {
        private readonly WindsorContainer _container;

        public ConstructorDependencyOnModel_Tests()
        {
            _container = new WindsorContainer();
        }
        [Fact]
        public void RegisterConstructorDependencyOnModel_Resolve_ResolutionFailed()
        {
            //Arrange
            this._container.Register(Component.For<ConstructorDependencyOnModel>());
            //Act
            //Assert
            Assert.Throws<HandlerException>(()=> this._container.Resolve<ConstructorDependencyOnModel>());
            
        }

        [Fact]
        public void RegisterFooA_ConstructorDependencyOnModel_Resolve_ResolutionFailed()
        {
            //Arrange
            this._container.Register(Component.For<FooA>());
            this._container.Register(Component.For<ConstructorDependencyOnModel>());
            //Act
            //Assert
            Assert.Throws<HandlerException>(() => this._container.Resolve<ConstructorDependencyOnModel>());
        }

        /// <summary>
        /// Example of Service Locator Pattern
        /// Do not use. Use Factory instead.
        /// </summary>
        [Fact]
        public void RegisterFooA_ConstructorDependencyOnModel_Resolve_Resolved()
        {
            //Arrange
            this._container.Register(Component.For<FooA>());
            this._container.Register(Component.For<ConstructorDependencyOnModel>());
            //Act
            var instance = this._container.Resolve<ConstructorDependencyOnModel>(new {Model = new Model()});
            //Assert
            Assert.IsType<ConstructorDependencyOnModel>(instance);
        }

        [Fact]
        public void RegisterFooA_ConstructorDependencyOnModel_Factory_Resolve_Resolved()
        {
            //Arrange
            this._container.AddFacility<TypedFactoryFacility>();
            this._container.Register(Component.For<FooA>());
            this._container.Register(Component.For<ConstructorDependencyOnModel>());
            this._container.Register(Component.For<IConstructorDependencyOnModelFactory>().AsFactory());

            //Act
            var factory = this._container.Resolve<IConstructorDependencyOnModelFactory>();
            var instance = factory.Create(new Model());
            //Assert
            Assert.IsType<ConstructorDependencyOnModel>(instance);
        }
    }
}
