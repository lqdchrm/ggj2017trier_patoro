using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes
{
    public interface Generator
    {
        float GetUpper(float t);
        float GetLower(float t);
    }
}
