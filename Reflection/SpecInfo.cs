using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Firelink.Todos.Reflection
{
    public class SpecInfo
    {
        public string Type { get; set; }
        public bool Optional { get; set; }
        public IEnumerable<string> Options { get; set; }
        public string Name { get; set; }
        public string ExclusiveWith {get; set;}

        public static SpecInfo FromParameterInfo(ParameterInfo parameter)
        {
            Type t = parameter.ParameterType;

            var si = new SpecInfo()
            {
                Type = t.Name,
                Optional = Nullable.GetUnderlyingType(t) != null,
                Name = parameter.Name,
            };

            if (t.IsEnum)
            {
                si.Options = t.GetEnumNames().ToList().AsReadOnly();
            }

            return si;
        }
    }
}
