using UnityEditor;
using UnityEngine;

namespace Garbage.Editor
{
    [CustomEditor(typeof(TotalCleaningManager))]
    public class TotalCleaningManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var data = (TotalCleaningManager)target;
            
            base.OnInspectorGUI();

            if (GUILayout.Button("Assign All Garbages"))
            {
                data.AssignGarbages();
            }
        }
    }
}