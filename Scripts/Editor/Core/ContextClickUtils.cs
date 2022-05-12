using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    public static class ContextClickUtils
    {
        private static object source;

        public static void SetSource(object targetSource)
        {
            source = targetSource;
        }

        public static bool CanPasteToTarget(object target)
        {
            if (source == null)
                return false;

            return target.GetType() == source.GetType();
        }
        
        public static void ApplySourceToTarget(object target)
        {
            if (source == null)
                return;

            EditorUtility.CopySerializedManagedFieldsOnly(source, target);
        }
        
        public static void CopyPropertyValue(SerializedProperty source, SerializedProperty dest)
		{
			EditorUtility.CopySerializedManagedFieldsOnly(source, dest);
		}

		public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property)
		{
			System.Type type;
			return GetFieldInfoFromProperty(property, out type);
		}

		public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out System.Type type)
		{
			var scriptTypeFromProperty = GetScriptTypeFromProperty(property);
			if (scriptTypeFromProperty == null)
			{
				type = null;
				return null;
			}
			return GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath, out type);
		}

		private static System.Type GetScriptTypeFromProperty(SerializedProperty property)
		{
			SerializedProperty serializedProperty = property.serializedObject.FindProperty("m_Script");
			if (serializedProperty == null)
			{
				return null;
			}
			MonoScript monoScript = serializedProperty.objectReferenceValue as MonoScript;
			if (monoScript == null)
			{
				return null;
			}
			return monoScript.GetClass();
		}

		public static bool IsArrayOrList(System.Type listType)
		{
			return listType.IsArray || (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>));
		}

		private static System.Type GetArrayOrListElementType(System.Type listType)
		{
			if (listType.IsArray)
			{
				return listType.GetElementType();
			}
			if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
			{
				return listType.GetGenericArguments()[0];
			}
			return null;
		}

		private static FieldInfo GetFieldInfoFromPropertyPath(System.Type host, string path, out System.Type type)
		{
			FieldInfo fieldInfo = null;
			type = host;
			string[] array = path.Split(new char[] {
				'.'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (i < array.Length - 1 && text == "Array" && array[i + 1].StartsWith("data["))
				{
					if (IsArrayOrList(type))
					{
						type = GetArrayOrListElementType(type);
					}
					i++;
				}
				else
				{
					FieldInfo fieldInfo2 = null;
					System.Type type2 = type;
					while (fieldInfo2 == null && type2 != null)
					{
						fieldInfo2 = type2.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						type2 = type2.BaseType;
					}
					if (fieldInfo2 == null)
					{
						type = null;
						return null;
					}
					fieldInfo = fieldInfo2;
					type = fieldInfo.FieldType;
				}
			}
			return fieldInfo;
		}

		static public object GetPropertyValue(SerializedProperty prop)
		{
			if (prop == null)
				return null;
			switch (prop.propertyType)
			{
				case SerializedPropertyType.Integer:
					return prop.intValue;
				case SerializedPropertyType.Boolean:
					return prop.boolValue;
				case SerializedPropertyType.Float:
					return prop.floatValue;
				case SerializedPropertyType.String:
					return prop.stringValue;
				case SerializedPropertyType.Color:
					return prop.colorValue;
				case SerializedPropertyType.ObjectReference:
					return prop.objectReferenceValue;
				case SerializedPropertyType.LayerMask:
					return prop.intValue;
				case SerializedPropertyType.Enum:
					return prop.enumValueIndex;
				case SerializedPropertyType.Vector2:
					return prop.vector2Value;
				case SerializedPropertyType.Vector3:
					return prop.vector3Value;
				case SerializedPropertyType.Quaternion:
					return prop.quaternionValue;
				case SerializedPropertyType.Rect:
					return prop.rectValue;
				case SerializedPropertyType.ArraySize:
					return prop.intValue;
				case SerializedPropertyType.Character:
					return prop.intValue;
				case SerializedPropertyType.AnimationCurve:
					return prop.animationCurveValue;
				case SerializedPropertyType.Bounds:
					return prop.boundsValue;
				case SerializedPropertyType.Gradient:
					break;
			}
			return null;
		}

		static public void SetPropertyValue(SerializedProperty prop, object value)
		{
			switch (prop.propertyType)
			{
				case SerializedPropertyType.Integer:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.Boolean:
					prop.boolValue = (bool)value;
					break;
				case SerializedPropertyType.Float:
					prop.floatValue = (float)value;
					break;
				case SerializedPropertyType.String:
					prop.stringValue = (string)value;
					break;
				case SerializedPropertyType.Color:
					prop.colorValue = (Color)value;
					break;
				case SerializedPropertyType.ObjectReference:
					prop.objectReferenceValue = (Object)value;
					break;
				case SerializedPropertyType.LayerMask:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.Enum:
					prop.enumValueIndex = (int)value;
					break;
				case SerializedPropertyType.Vector2:
					prop.vector2Value = (Vector2)value;
					break;
				case SerializedPropertyType.Vector3:
					prop.vector3Value = (Vector3)value;
					break;
				case SerializedPropertyType.Quaternion:
					prop.quaternionValue = (Quaternion)value;
					break;
				case SerializedPropertyType.Rect:
					prop.rectValue = (Rect)value;
					break;
				case SerializedPropertyType.ArraySize:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.Character:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.AnimationCurve:
					prop.animationCurveValue = (AnimationCurve)value;
					break;
				case SerializedPropertyType.Bounds:
					prop.boundsValue = (Bounds)value;
					break;
				case SerializedPropertyType.Gradient:
					break;
			}
		}
    }
}