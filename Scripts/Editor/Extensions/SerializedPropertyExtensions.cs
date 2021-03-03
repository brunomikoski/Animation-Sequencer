using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace BrunoMikoski.AnimationSequencer
{
    public static class SerializedPropertyExtensions
    {
        private static Dictionary<string, float> propertyPathToHeight = new Dictionary<string, float>();
        private static Dictionary<string, Type> managedReferenceFullTypeNameToTypeCache = new Dictionary<string, Type>();

        private static Dictionary<string, object> propertyPathToObjectCache = new Dictionary<string, object>();
        
        public static Type GetTypeFromManagedFullTypeName(this SerializedProperty serializedProperty)
        {
            if (string.IsNullOrEmpty(serializedProperty.managedReferenceFullTypename))
                throw new Exception($"Serialized Property doesnt have managedReferenceFullTypename");
            
            if (managedReferenceFullTypeNameToTypeCache.TryGetValue(serializedProperty.managedReferenceFullTypename, out Type type))
                return type;
            
            string[] typeInfo = serializedProperty.managedReferenceFullTypename.Split(' ');
            type = Type.GetType($"{typeInfo[1]}, {typeInfo[0]}");
            managedReferenceFullTypeNameToTypeCache.Add(serializedProperty.managedReferenceFullTypename, type);

            return type;
        }
        
        public static float GetPropertyDrawerHeight(this SerializedProperty property, float defaultHeight = 18)
        {
            return GetPropertyDrawerHeight(property.propertyPath, defaultHeight);
        }
        
        public static float GetPropertyDrawerHeight(string propertyPath, float defaultHeight = 18)
        {
            if (propertyPathToHeight.TryGetValue(propertyPath, out float result))
                return result;

            result = defaultHeight;
            return result;
        }

        public static void SetPropertyDrawerHeight(this SerializedProperty property, float height)
        {
            SetPropertyDrawerHeight(property.propertyPath, height);
        }

        public static void SetPropertyDrawerHeight(string propertyPath, float height)
        {
            propertyPathToHeight[propertyPath] = height;
        }

        public static void ClearPropertyCache(string pathOrPartOfPath = "")
        {
            if (string.IsNullOrEmpty(pathOrPartOfPath))
            {
                propertyPathToObjectCache.Clear();
                return;
            }
            
            List<string> propertiesTobeRemoved = new List<string>();
            foreach (KeyValuePair<string,object> keyValuePair in propertyPathToObjectCache)
            {
                string key = keyValuePair.Key;
                if (key.IndexOf(pathOrPartOfPath, StringComparison.Ordinal) == -1)
                    continue;

                propertiesTobeRemoved.Add(key);
            }

            for (int i = 0; i < propertiesTobeRemoved.Count; i++)
                propertyPathToObjectCache.Remove(propertiesTobeRemoved[i]);
        }
        
        public static bool TryGetTargetObjectOfProperty<T>(this SerializedProperty prop, out T resultObject) where T : class
        {
            resultObject = null;
            if (prop == null) 
                return false;

            // if (propertyPathToObjectCache.TryGetValue(prop.propertyPath, out object result))
            // {
            //     if (result != null)
            //     {
            //         resultObject = result as T;
            //         return true;
            //     }
            //
            //     propertyPathToObjectCache.Remove(prop.propertyPath);
            // }
            
            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            string[] elements = path.Split('.');
            for (int i = 0; i < elements.Length; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            if (obj is T t)
            {
                resultObject = t;
                // propertyPathToObjectCache.Add(prop.propertyPath, resultObject);
                return true;
            }

            return false;
        }
        
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
            if (enumerable == null) 
                return null;
            IEnumerator enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) 
                    return null;
            }
            return enm.Current;
        }
        
        
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.Next(false);
            }
 
            if (currentProperty.Next(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;
 
                    yield return currentProperty;
                }
                while (currentProperty.Next(false));
            }
        }
        public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.NextVisible(false);
            }
 
            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;
 
                    yield return currentProperty;
                }
                while (currentProperty.NextVisible(false));
            }
        }
    }
}
