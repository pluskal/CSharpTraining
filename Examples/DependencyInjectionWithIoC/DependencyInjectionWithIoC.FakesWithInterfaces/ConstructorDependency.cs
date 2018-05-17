namespace DependencyInjectionWithIoC.FakesWithInterfaces
{
    public class ConstructorDependency
    {
        public IFoo Foo { get; }

        public ConstructorDependency(IFoo foo)
        {
            Foo = foo;
        }
    }
}