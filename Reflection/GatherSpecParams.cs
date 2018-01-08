using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Firelink.Todos.Reflection
{

    using SpecMap = Dictionary<string, SpecInfo>;

    public class SpecParamGather<T>
    {
        private readonly Type specbag;

        public SpecParamGather()
        {
            specbag = typeof(T);
        }

        public SpecMap Gather()
        {
            return null;
        }

        private SpecMap GetRawMap()
        {
            var members = specbag.GetFields(BindingFlags.Public);
            return null;
        }
    }
}
