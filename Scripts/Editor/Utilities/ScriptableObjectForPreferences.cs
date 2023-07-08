using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    //From https://github.com/baba-s/UniScriptableObjectForPreferences/blob/master/Editor/ScriptableObjectForPreferences.cs
    public abstract class ScriptableObjectForPreferences<T> : ScriptableObject
        where T : ScriptableObjectForPreferences<T>
    {
        private static string ConfigName => typeof(T).Name;
        private static T instance;

        public static T GetInstance()
        {
            if (instance != null)
            {
                return instance;
            }

            instance = CreateInstance<T>();
            string json = EditorUserSettings.GetConfigValue(ConfigName);
            EditorJsonUtility.FromJsonOverwrite(json, instance);
            if (instance == null)
            {
                instance = CreateInstance<T>();
            }

            return instance;
        }

        public static SettingsProvider CreateSettingsProvider(
            string settingsProviderPath = null,
            Action<SerializedObject> onGUI = null,
            Action<SerializedObject> onGUIExtra = null
        )
        {
            if (settingsProviderPath == null)
            {
                settingsProviderPath = $"{typeof(T).Name}";
            }

            T foundInstance = GetInstance();
            SerializedObject serializedObject = new SerializedObject(foundInstance);
            IEnumerable<string> keywords = SettingsProvider.GetSearchKeywordsFromSerializedObject(serializedObject);
            SettingsProvider provider = new SettingsProvider(settingsProviderPath, SettingsScope.User, keywords);
            provider.guiHandler += _ => OnGuiHandler(onGUI, onGUIExtra);
            return provider;
        }

        private static void OnGuiHandler(Action<SerializedObject> onGUI,
            Action<SerializedObject> onGUIExtra)
        {
            T foundInstance = GetInstance();
            Editor editor = Editor.CreateEditor(foundInstance);
            using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
            {
                SerializedObject serializedObject = editor.serializedObject;
                serializedObject.Update();
                if (onGUI != null)
                {
                    onGUI(serializedObject);
                }
                else
                {
                    editor.DrawDefaultInspector();
                }

                onGUIExtra?.Invoke(serializedObject);
                if (!scope.changed)
                {
                    return;
                }

                serializedObject.ApplyModifiedProperties();
                string json = EditorJsonUtility.ToJson(editor.target);
                EditorUserSettings.SetConfigValue(ConfigName, json);
            }
        }
    }
}
