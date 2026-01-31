using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace intheclouds
{
    public static class MenuItems
    {
        [MenuItem("Tools/ITC Tools/Align View To Object %#l", false, 300)] // Ctrl + Shift + L
        private static void AlignViewToObject()
        {
            if (SceneView.lastActiveSceneView && Selection.activeTransform)
            {
                SceneView.lastActiveSceneView.AlignViewToObject(Selection.activeTransform);
            }
        }

        [MenuItem("Tools/ITC Tools/Align To View (no rotation) %#&f", false, 301)] // Ctrl + Shift + Alt + F
        private static void AlignToView_IgnoreRotation()
        {
            var activeTransform = Selection.activeTransform;
            Undo.RecordObject(activeTransform, "Align To View (no rotation)");
            SceneView.lastActiveSceneView.AlignWithView();
            activeTransform.rotation = Quaternion.identity;
        }

        [MenuItem("Tools/ITC Tools/Align with ground %#g", false, 302)] // Ctrl + Shift + G
        private static void AlignWithGround()
        {
            var activeTransform = Selection.activeTransform;
            if (Physics.Raycast(activeTransform.position, Vector3.down, out var hit, 50f, LayerMask.GetMask("Ground")))
            {
                Undo.RecordObject(activeTransform, "Align with ground");
                Debug.Log($"Aligned {activeTransform} with ground height", activeTransform);
                activeTransform.position = hit.point;
            }
        }
        
        [MenuItem("Tools/ITC Tools/Center Parent to Children %#&c", false, 303)] // Ctrl+Alt+Shift+C
        static void CenterSelectedParent()
        {
            if (!Selection.activeTransform) return;
            var parent = Selection.activeTransform;
            if (parent.childCount == 0) return;

            Vector3 avg = Vector3.zero;
            foreach (Transform child in parent)
                avg += child.position;
            avg /= parent.childCount;

            // Move parent and preserve children's world positions
            Undo.RecordObject(parent, "Center Parent to Children");
            Vector3 delta = avg - parent.position;
            parent.position = avg;
            foreach (Transform child in parent)
                child.position -= delta;
        }

        [MenuItem("Tools/ITC Tools/Zero Parent Rotation %#&r", false, 304)] // Ctrl+Alt+Shift+R
        static void ZeroParentRotation()
        {
            if (!Selection.activeTransform) return;
            var parent = Selection.activeTransform;
            if (parent.childCount == 0)
            {
                Undo.RecordObject(parent, "Zero Parent Rotation");

                parent.rotation = Quaternion.identity;
                return;
            }
            
            var toRecord = new List<Object> { parent };
            foreach (Transform child in parent)
                toRecord.Add(child);

            Undo.RecordObjects(toRecord.ToArray(), "Zero Parent Rotation");

            var rots = new List<Quaternion>();
            var positions = new List<Vector3>();
            foreach (Transform child in parent)
            {
                rots.Add(child.rotation);
                positions.Add(child.position);
            }
            
            parent.rotation = Quaternion.identity;
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                child.rotation = rots[i];
                child.position = positions[i];
            }
        }
    }
}