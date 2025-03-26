using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FeedbacksEditor.Editor
{
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : UnityEditor.Editor
    {
        private SerializedProperty _effects;
        private ReorderableList _list;
        private Type[] _types;
        
        private void OnEnable()
        {
            _effects = serializedObject.FindProperty("Effects");
            
            _list = new ReorderableList(serializedObject, _effects, true, true, true, true);
            _list.drawElementCallback = DrawListItems;
            _list.drawHeaderCallback = DrawHeader;
            _list.onAddDropdownCallback = AddDropDown;

            _types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where assemblyType.IsSubclassOf(typeof(GameEffect))
                select assemblyType).ToArray();
        }
        
        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            GameEvent gameEvent = (GameEvent)target;
            if (gameEvent == null) return;
            
            GameEffect feedback = gameEvent.Effects[index];
            if (feedback == null) return;
            
            

            if (index > 0)
            {
                var line = rect;
                line.height = 1;
                line.x += 45;
                line.width -= 45;
                EditorGUI.DrawRect(line, new Color(0.2f,0.2f,0.2f));
            }
            
            var color = rect;
            color.width = 10;
            EditorGUI.DrawRect(color, feedback.GetColor());
            
            SerializedProperty element = _effects.GetArrayElementAtIndex(index);
            SerializedProperty enabledProperty = element.FindPropertyRelative("Enabled");
            rect.x += 15;
            enabledProperty.boolValue = EditorGUI.Toggle(rect, GUIContent.none, enabledProperty.boolValue);

            rect.x += 30;
            EditorGUI.LabelField(rect, gameEvent.Effects[index].ToString());

            if (isFocused == false && isActive == false) return;

            foreach (SerializedProperty child in GetChildren(element))
            {
                EditorGUILayout.PropertyField(child);
            }

        }

        private IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
        {
            SerializedProperty currentProperty = property.Copy();
            SerializedProperty nextProperty = property.Copy();
            nextProperty.Next(false);

            if (currentProperty.Next(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextProperty)) break;
                    yield return currentProperty;
                } while (currentProperty.Next(false));
            }
        }

        private void DrawHeader(Rect rect)
        {
        }
        
        private void AddDropDown(Rect rect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();

            foreach (Type type in _types)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    _effects.arraySize++;
                    SerializedProperty newProp = _effects.GetArrayElementAtIndex(_effects.arraySize - 1);
                    newProp.managedReferenceValue = Activator.CreateInstance(type);
                    serializedObject.ApplyModifiedProperties();
                });
            }
            
            menu.ShowAsContext();
        }
        
        public override void OnInspectorGUI()
        {
            GameEvent gameEvent = (GameEvent)target;
            //base.OnInspectorGUI();

            _list.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}