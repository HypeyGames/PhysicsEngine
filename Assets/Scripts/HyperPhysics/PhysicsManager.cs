using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace HyperPhysics
{
    public class PhysicsManager : MonoBehaviour
    {
        [SerializeField] private List<Collider> _colliders;

        public void RegisterSceneColliders()
        {
            _colliders.Clear();
            var scene = SceneManager.GetActiveScene();
            ListPool<GameObject>.Get(out var rootGameObjects);
            ListPool<Collider>.Get(out var colliders);
            scene.GetRootGameObjects(rootGameObjects);
            foreach (var rootGameObject in rootGameObjects)
            {
                rootGameObject.GetComponentsInChildren(colliders);
                _colliders.AddRange(colliders);
            }

            ListPool<GameObject>.Release(rootGameObjects);
            ListPool<Collider>.Release(colliders);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _colliders.Count; i++)
            {
                for (int j = i + 1; j < _colliders.Count; j++)
                {
                    if (_colliders[i].CheckForCollision(_colliders[j]) <= 0)
                    {
                        
                    }
                }
            }
        }
    }
}