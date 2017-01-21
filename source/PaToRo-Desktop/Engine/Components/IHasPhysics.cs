using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine.Components
{
    public interface IHasPhysics
    {
        Physics Phy { get; }
    }
}
