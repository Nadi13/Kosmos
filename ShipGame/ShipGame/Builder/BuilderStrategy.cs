using Hwdtech;
using ShipGame.Move;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShipGame.Builder
{
    public class BuilderStrategy: IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var dtype = (Type)args[0];
            var dtype1 = (Type)args[1];
            var builder = IoC.Resolve<IBuilder>("BuilderForAdaptationDtype1ToDtype2", dtype, dtype1);
            builder.addAnything("Dtype", dtype);
            var properties = dtype.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                builder.addAnything("Property", property);
            }
            var result = builder.Build();
            return result;
        }
    }
}
