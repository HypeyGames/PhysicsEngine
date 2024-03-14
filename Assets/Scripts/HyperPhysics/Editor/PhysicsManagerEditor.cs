using UnityEditor;
using UnityEngine;

namespace HyperPhysics
{
    [CustomEditor(typeof(PhysicsManager))]
    public class PhysicsManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var physicsManager = (PhysicsManager)target;
            if (GUILayout.Button("Register Scene Colliders"))
            {
                physicsManager.RegisterSceneColliders();
            }
        }
    }
}