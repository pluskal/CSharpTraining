namespace DependencyInjectionWithIoC.Fakes.Models.Dependencies
{
    public class Base
    {
        public virtual string Name => "Base";

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}";
        }
    }
}
