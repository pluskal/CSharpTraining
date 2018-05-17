namespace DependencyInjectionWithIoC.Fakes
{
    public class ConstructorDependency
    {
        private readonly FooA _fooA;

        public ConstructorDependency(FooA fooA)
        {
            _fooA = fooA;
        }
    }
}