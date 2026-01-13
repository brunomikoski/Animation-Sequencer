#if DOTWEEN_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    [CustomPropertyDrawer(typeof(AnimationStepBase), true)]
    public class AnimationStepBasePropertyDrawer : PropertyDrawer
    {
        protected void DrawBaseGUI(Rect position, SerializedProperty property, GUIContent label, params string[] excludedPropertiesNames)
        {
            if (GUI.Button(new Rect(position.width - 40, position.y+2, 80, EditorGUIUtility.singleLineHeight - 1), "Duplicate"))
            {
                DuplicateProperty(property);
            }

            float originY = position.y;

            position.height = EditorGUIUtility.singleLineHeight;
            
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true, EditorStyles.foldout);

            if (property.isExpanded)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel--;
                
                position.height = EditorGUIUtility.singleLineHeight;
                position.y +=  EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                foreach (SerializedProperty serializedProperty in property.GetChildren())
                {
                    bool shouldDraw = true;
                    for (int i = 0; i < excludedPropertiesNames.Length; i++)
                    {
                        string excludedPropertyName = excludedPropertiesNames[i];
                        if (serializedProperty.name.Equals(excludedPropertyName, StringComparison.Ordinal))
                        {
                            shouldDraw = false;
                            break;
                        }
                    }

                    if (!shouldDraw)
                        continue;

                    EditorGUI.PropertyField(position, serializedProperty);
                    position.y += EditorGUI.GetPropertyHeight(serializedProperty) + EditorGUIUtility.standardVerticalSpacing;

                }
                
                if (EditorGUI.EndChangeCheck())
                    property.serializedObject.ApplyModifiedProperties();
            }
            
            property.SetPropertyDrawerHeight(position.y - originY + EditorGUIUtility.singleLineHeight);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawBaseGUI(position, property, label);
        }
    
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetPropertyDrawerHeight();
        }

        private void DuplicateProperty(SerializedProperty property)
        {
            SerializedProperty parentArray = GetParentArrayProperty(property);
            if (parentArray != null && parentArray.isArray)
            {
                int index = GetIndexInArray(property);

                object sourceObject = property.managedReferenceValue;
                object clonedObject = CloneManagedReference(sourceObject);

                if (clonedObject != null)
                {
                    parentArray.InsertArrayElementAtIndex(index);

                    var newElement = parentArray.GetArrayElementAtIndex(index + 1);
                    newElement.managedReferenceValue = clonedObject;

                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private SerializedProperty GetParentArrayProperty(SerializedProperty property)
        {
            string path = property.propertyPath;
            int lastDot = path.LastIndexOf('.');
            if (lastDot < 0)
                return null;

            string arrayPath = path.Substring(0, lastDot);
            return property.serializedObject.FindProperty(arrayPath);
        }

        private int GetIndexInArray(SerializedProperty property)
        {
            string path = property.propertyPath;
            int start = path.IndexOf("[") + 1;
            int end = path.IndexOf("]");
            string indexStr = path.Substring(start, end - start);
            return int.Parse(indexStr);
        }

        private FieldInfo[] GetAllFieldsIncludingBaseTypes(Type type, BindingFlags flags)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            while (type != null)
            {
                fields.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
                type = type.BaseType;
            }
            return fields.ToArray();
        }

        private object CloneManagedReference(object obj, int depth = 2)
        {
            if (obj == null) return null;

            if (depth == 0) return obj;

            Type type = obj.GetType();
            object clone = System.Activator.CreateInstance(type);

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] fields = GetAllFieldsIncludingBaseTypes(type , flags);// type.GetFields(flags);

            foreach (FieldInfo field in fields)
            {
                // Skip private/protected fields without [SerializeField] or [SerializeReference]
                if (!field.IsPublic)
                {
                    var isSeialized = field.GetCustomAttribute<SerializeField>() != null || field.GetCustomAttribute<SerializeReference>() != null;
                    if (!isSeialized)
                        continue;
                }

                var value = field.GetValue(obj);
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var list = (IList)obj;
                    var clonedList = (IList)Activator.CreateInstance(field.FieldType);
                    foreach (var item in list)
                    {
                        var clonedItem = CloneManagedReference(item, depth - 1);
                        clonedList.Add(clonedItem);
                    }
                    field.SetValue(clone, clonedList);
                }
                else if (field.FieldType.IsArray)
                {
                    var elementType = field.FieldType.GetElementType();
                    var array = (Array)value;
                    var clonedArray = Array.CreateInstance(elementType, array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        var clonedItem = CloneManagedReference(array.GetValue(i), depth - 1);
                        clonedArray.SetValue(clonedItem, i);
                    }
                    field.SetValue(clone, clonedArray);
                }
                else if (typeof(UnityEventBase).IsAssignableFrom(field.FieldType))
                {
                    var originalEvent = (UnityEventBase)value;
                    var clonedEvent = Activator.CreateInstance(field.FieldType) as UnityEventBase;

                    if (originalEvent is UnityEvent originalUnityEvent && clonedEvent is UnityEvent clonedUnityEvent)
                    {
                        int count = originalUnityEvent.GetPersistentEventCount();
                        for (int i = 0; i < count; i++)
                        {
                            var target = originalUnityEvent.GetPersistentTarget(i);
                            var methodName = originalUnityEvent.GetPersistentMethodName(i);

                            if (target != null && !string.IsNullOrEmpty(methodName))
                            {
                                var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                if (method != null)
                                {
                                    var action = Delegate.CreateDelegate(typeof(UnityAction), target, method, false) as UnityAction;
                                    if (action != null)
                                    {
                                        UnityEventTools.AddPersistentListener(clonedUnityEvent, action);
                                    }
                                }
                            }
                        }

                        field.SetValue(clone, clonedUnityEvent);
                    }
                    else
                    {
                        field.SetValue(clone, clonedEvent);
                    }
                }
                else if (IsManagedReferenceField(field))
                {
                    var duplicate = CloneManagedReference(value, depth - 1);
                    field.SetValue(clone, duplicate);
                }
                else
                {
                    field.SetValue(clone, value);
                }
                
            }

            return clone;
        }

        private bool IsManagedReferenceField(FieldInfo field)
        {
            Type fieldType = field.FieldType;

            //we are not cloning any unity object
            if (fieldType == typeof(UnityEngine.Object))
                return false;

            // If it's object, abstract class, interface, or not sealed
            if (fieldType == typeof(object))
                return true;

            if (fieldType.IsAbstract || fieldType.IsInterface)
                return true;

            if (!fieldType.IsSealed && !fieldType.IsValueType)
                return true; // non-sealed class (polymorphic)

            return false;
        }
    }
}
#endif