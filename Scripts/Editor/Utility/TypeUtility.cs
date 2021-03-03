using System;
using System.Collections.Generic;
using System.Linq;

namespace BrunoMikoski.AnimationSequencer.Utility
{
    public static class TypeUtility
    {
        private static Dictionary<Type, List<Type>> typeToSubclasses = new Dictionary<Type, List<Type>>();

        private static List<Type> cachedAvailableTypes;
        private static List<Type> AvailableTypes
        {
            get
            {
                if (cachedAvailableTypes == null)
                    cachedAvailableTypes = GetTypesFromAssemblies();
                return cachedAvailableTypes;
            }
        }
        
        private static List<Type> GetTypesFromAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x != null)
                .SelectMany(x => x.GetTypes())
                .ToList();
        }

        
        public static List<Type> GetAllSubclasses(Type targetType)
        {
            if (typeToSubclasses.TryGetValue(targetType, out List<Type> results))
                return results;

            results = AvailableTypes.Where(t => t.IsClass && t.IsSubclassOf(targetType)).ToList();
            typeToSubclasses.Add(targetType, results);
            return results;
        }
    }
}
