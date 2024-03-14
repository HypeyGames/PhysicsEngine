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

        public event Action Update;
        public event Action PostUpdate;

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
            Update?.Invoke();
            foreach (var collider in _colliders)
            {
                if (collider.Static == false)
                {
                    var rb = collider.Rigidbody;

                    rb.Velocity += rb.Acceleration * Time.fixedDeltaTime;
                   
                    if (rb.Gravity)
                    {
                        rb.Velocity += Physics.gravity * Time.fixedDeltaTime;
                    }

                    collider.transform.position += rb.Velocity * Time.fixedDeltaTime;
                }
            }

            for (int i = 0; i < _colliders.Count; i++)
            {
                for (int j = i + 1; j < _colliders.Count; j++)
                {
                    var collision = _colliders[i].CheckForCollision(_colliders[j]);
                    if (collision.CollisionType != CollisionType.NotValid && collision.Penetration >= 0)
                    {
                        float velocityAlongNormal;
                        switch (collision.CollisionType)
                        {
                            case CollisionType.StaticDynamic:
                                _colliders[j].transform.position += collision.Normal * collision.Penetration;

                                velocityAlongNormal = Vector3.Dot(_colliders[j].Rigidbody.Velocity, collision.Normal);
                                _colliders[j].Rigidbody.Velocity -= 2 * velocityAlongNormal * collision.Normal;
                                break;

                            case CollisionType.DynamicStatic:
                                _colliders[i].transform.position -= collision.Normal * collision.Penetration;

                                velocityAlongNormal = Vector3.Dot(_colliders[i].Rigidbody.Velocity, collision.Normal);
                                _colliders[i].Rigidbody.Velocity -= 2 * velocityAlongNormal * collision.Normal;
                                break;

                            case CollisionType.DynamicDynamic:
                                _colliders[i].transform.position -= collision.Normal * (collision.Penetration * collision.MassRatio21);
                                _colliders[j].transform.position += collision.Normal * (collision.Penetration * collision.MassRatio12);

                                var relativeVelocity12 = _colliders[i].Rigidbody.Velocity - _colliders[j].Rigidbody.Velocity;
                                velocityAlongNormal = Vector3.Dot(relativeVelocity12, collision.Normal);

                                _colliders[i].Rigidbody.Velocity = _colliders[j].Rigidbody.Velocity + ((collision.MassRatio12 - collision.MassRatio21) * velocityAlongNormal * collision.Normal);
                                _colliders[j].Rigidbody.Velocity += 2 * collision.MassRatio12 * velocityAlongNormal * collision.Normal;

                                break;
                        }
                    }
                }
            }

            PostUpdate?.Invoke();
        }
    }
}