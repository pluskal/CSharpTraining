namespace DependencyInjectionWithIoC.FakesWithInterfaces.Models.Dependencies
{
    public class FooBase : IFoo
    {
        public virtual string Name => "FooBase";

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}";
        }
    }
}