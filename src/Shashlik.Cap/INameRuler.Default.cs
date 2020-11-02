using System;
using System.Reflection;
using DotNetCore.CAP;
using Microsoft.Extensions.Options;

namespace Shashlik.Cap
{
    public class DefaultNameRuler : INameRuler
    {
        public DefaultNameRuler(IOptions<CapOptions> capOptions)
        {
            CapOptions = capOptions;
        }

        private IOptions<CapOptions> CapOptions { get; }

        public string GetName(Type type)
        {
            string name = $"{type.Name}.{CapOptions.Value.Version}";
            var nameAttribute = type.GetCustomAttribute<CapNameAttribute>(true);
            if (nameAttribute != null)
                name = nameAttribute.Name;

            return name;
        }
    }
}