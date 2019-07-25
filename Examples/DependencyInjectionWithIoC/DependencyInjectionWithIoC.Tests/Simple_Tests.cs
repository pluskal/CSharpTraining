using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DependencyInjectionWithIoC.Fakes;
using DependencyInjectionWithIoC.Fakes.Models.Dependencies;
using Xunit;

namespace DependencyInjectionWithIoC.Tests
{
    public class Simple_Tests
    {
        private readonly WindsorContainer _container;

        public Simple_Tests()
        {
            _container = new WindsorContainer();
        }
        [Fact]
        public void RegisterBase_Resolve_BaseResolved()
        {
            //Arrange
            this._container.Register(Component.For<Base>());
            //Act
            var baseInstance = this._container.Resolve<Base>();
            //Assert
            Assert.NotNull(baseInstance);
        }

        [Fact]
        public void RegisterBase_ResolveTwice_ResolvedSameInstance()
        {
            //Arrange
            this._container.Register(Component.For<Base>());
            //Act
            var baseInstance1 = this._container.Resolve<Base>();
            var baseInstance2 = this._container.Resolve<Base>();
            //Assert
            Assert.Same(baseInstance1, baseInstance2);
        }

        [Fact]
        public void RegisterBaseSingleton_Resolve_ResolvedSameInstance()
        {
            //Arrange
            this._container.Register(Component.For<Base>().LifestyleSingleton());
            //Act
            var baseInstance1 = this._container.Resolve<Base>();
            var baseInstance2 = this._container.Resolve<Base>();
            //Assert
            Assert.Same(baseInstance1, baseInstance2);
        }

        [Fact]
        public void RegisterBaseTransient_Resolve_ResolvedNewInstances()
        {
            //Arrange
            this._container.Register(Component.For<Base>().LifestyleTransient());
            //Act
            var baseInstance1 = this._container.Resolve<Base>();
            var baseInstance2 = this._container.Resolve<Base>();
            //Assert
            Assert.NotSame(baseInstance1, baseInstance2);
        }

        [Fact]
        public void RegisterFooA_ResolveByBase_FooAResolved()
        {
            //Arrange
            this._container.Register(Component.For<Base,FooA>());
            //Act
            var fooA = this._container.Resolve<Base>();
            //Assert
            Assert.IsType<FooA>(fooA);
        }

        [Fact]
        public void RegisterFooA_ResolveByFooA_FooAResolved()
        {
            //Arrange
            this._container.Register(Component.For<Base, FooA>());
            //Act
            var fooA = this._container.Resolve<FooA>();
            //Assert
            Assert.IsType<FooA>(fooA);
        }

        [Fact]
        public void RegisterBase_FooA_FooB_ResolveBy_Base_FooA_FooB_FooBResolved()
        {
            //Arrange
            this._container.Register(Component.For<Base, FooA, FooB>());
            //Act
            var baseInstance = this._container.Resolve<Base>();
            var fooA = this._container.Resolve<FooA>();
            var fooB = this._container.Resolve<FooB>();
            //Assert
            Assert.IsType<FooB>(baseInstance);
            Assert.IsType<FooB>(fooA);
            Assert.IsType<FooB>(fooB);
        }
    }
}
