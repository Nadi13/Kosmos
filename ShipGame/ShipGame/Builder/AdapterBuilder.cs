using Scriban;
using System.Reflection;

namespace ShipGame.Builder
{
    public class AdapterBuilder: IBuilder
    {
        private Template textOfClass = Template.Parse(text:
        @"
public class {{ class_name }} : {{ int_name }} {
    private System.Collections.Generic.IDictionary<string, object> target;
    public {{ class_name }}(System.Collections.Generic.IDictionary<string, object> target) {
        this.target = target;
    }
    {{for property in properties }}
    public {{ property.type }} {{ property.name }} {
            {{if property.readable}}
                get => IoC.Resolve<{{property.type}}>(""UObjectGetValue"", target, {{property.name}});
            {{end}}
            {{if property.writable}}
                set => IoC.Resolve<ICommand>(""UObjectSetValue"", target, {{property.name}}, propertyValue).Execute();
            {{end}}
    }
    {{end}}
        }
        ");
        private Type dtype;
        private IEnumerable<PropertyInfo> propertyInfos;
        private readonly IDictionary<string, Action<object[]>> configurationHandlers = new Dictionary<string, Action<object[]>>{
        {"Dtype",
            args => {
                var ch = (AdapterBuilder)args[0];
                ch.dtype = (Type)args[1];
            }
        },
        {"Property",
            args => {
                var ch = (AdapterBuilder)args[0];
                var propInfo = (PropertyInfo)args[1];
                ch.propertyInfos = ch.propertyInfos.Append(propInfo).ToArray();
            }
        }
        };
        public AdapterBuilder()
        {
            this.propertyInfos = new LinkedList<PropertyInfo>();
            this.dtype = null!;
        }
        public IBuilder addAnything(string param, params object[] args)
        {
            Action<object[]> handler = configurationHandlers[param];
            handler(new object[] { this }.Concat(args).ToArray());
            return this;
        }
        public object Build()
        {
            object model = new
            {
                class_name = dtype.Name + "_adapter",
                int_name = dtype.FullName,
                properties = this.propertyInfos.Select(
               (PropertyInfo p) =>
               {
                   object property = new
                   {
                       name = p.Name,
                       type = p.PropertyType.FullName,
                       readable = p.CanRead,
                       writable = p.CanWrite
                   };
                   return property;
               }
               ).ToList()
            };
            var result = this.textOfClass.Render(model);
            return result.Replace("\r", "").Replace("\r\n", "").Replace("    ", "").Replace("\n", "");
        }
    }
}
