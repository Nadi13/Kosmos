using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipGame.Builder
{
    public interface IBuilder
    {
        IBuilder addAnything(string param, params object[] args);
        object Build();
    }
}
