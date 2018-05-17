namespace DependencyInjectionWithIoC.Fakes
{
    public class ConstructorDependencyOnModel
    {
        private readonly FooA _fooA;

        public ConstructorDependencyOnModel(FooA fooA, Model model)
        {
            _fooA = fooA;
        }
    }
}