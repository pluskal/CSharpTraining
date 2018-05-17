using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionWithIoC.Fakes
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
