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

        [SerializeField] private int _boundingBoxSize;
        [SerializeField] private int _physicsWorldSize;
        [SerializeField] private List<AA3DBB> _bounds = new List<AA3DBB>();
        [SerializeField, Range(.01f, 10)] private float _sleepThreshold;
        [SerializeField, Range(1, 10)] private int _collisionSubstep;
        [SerializeField] private bool _debug;

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
            GenerateBoundingBoxes();
        }

        private void FixedUpdate()
        {
            Update?.Invoke();

            for (int i = 0; i < _colliders.Count; i++)
            {
                if (_colliders[i].Static)
                {
                    continue;
                }

                var acceleration = ApplyMotion(_colliders[i]);
                for (int c = 0; c < _collisionSubstep; c++)
                {
                    ResolveCollision(i, ref acceleration);
                }

                _colliders[i].Rigidbody.Velocity += acceleration * Time.fixedDeltaTime;
            }

            PostUpdate?.Invoke();
        }

        private void ResolveCollision(int i, ref Vector3 acceleration)
        {
            for (int j = 0; j < _colliders.Count; j++)
            {
                if (i == j) continue;

                if (_colliders[i].CheckForOverlap(_colliders[j]) == false) continue;
                var collision = _colliders[i].CheckForCollision(_colliders[j]);
                
                if (collision.CollisionType != CollisionType.NotValid && collision.Penetration >= 0)
                {
                    float velocityAlongNormal;
                    switch (collision.CollisionType)
                    {
                        case CollisionType.StaticDynamic:

                            velocityAlongNormal = Vector3.Dot(_colliders[j].Rigidbody.Velocity, collision.Normal);
                            if (Mathf.Abs(velocityAlongNormal) < _sleepThreshold)
                            {
                                _colliders[j].transform.position += collision.Normal * collision.Penetration;
                                acceleration -= Vector3.Dot(acceleration, collision.Normal) * collision.Normal;
                            }
                            else
                            {
                                _colliders[j].Rigidbody.Velocity -= 2 * velocityAlongNormal * collision.Normal;
                            }

                            break;

                        case CollisionType.DynamicStatic:

                            _colliders[i].transform.position -= (2 * collision.Penetration) * collision.Normal;
                            velocityAlongNormal = Vector3.Dot(_colliders[i].Rigidbody.Velocity, collision.Normal);

                            acceleration -= Vector3.Dot(acceleration, collision.Normal) * collision.Normal;

                            if (Mathf.Abs(velocityAlongNormal) < _sleepThreshold)
                            {
                                _colliders[i].Rigidbody.Velocity -= velocityAlongNormal * collision.Normal;
                            }
                            else
                            {
                                _colliders[i].Rigidbody.Velocity -= 2 * velocityAlongNormal * collision.Normal;
                            }


                            break;

                        case CollisionType.DynamicDynamic:
                            _colliders[i].transform.position -= collision.Normal * (collision.Penetration * collision.MassRatio21);
                            _colliders[j].transform.position += collision.Normal * (collision.Penetration * collision.MassRatio12);

                            var relativeVelocity12 = _colliders[i].Rigidbody.Velocity - _colliders[j].Rigidbody.Velocity;
                            velocityAlongNormal = Vector3.Dot(relativeVelocity12, collision.Normal);

                            var relativeForce = (acceleration * _colliders[i].Rigidbody.Mass - _colliders[j].Rigidbody.Acceleration * _colliders[j].Rigidbody.Mass);
                            var forceAlongNormal = Vector3.Dot(relativeForce, collision.Normal) * collision.Normal;
                            acceleration -= forceAlongNormal / _colliders[i].Rigidbody.Mass;
                            _colliders[j].Acceleration += relativeForce / _colliders[j].Rigidbody.Mass;

                            var velocity = velocityAlongNormal * collision.Normal;
                            _colliders[i].Rigidbody.Velocity = _colliders[j].Rigidbody.Velocity + (collision.MassRatio12 - collision.MassRatio21) * velocity;
                            _colliders[j].Rigidbody.Velocity += 2 * collision.MassRatio12 * velocity;

                            break;
                    }
                }
            }
        }

        private Vector3 ApplyMotion(Collider collider)
        {
            var rb = collider.Rigidbody;

            var acceleration = rb.Acceleration;
            if (rb.Gravity)
            {
                acceleration += Physics.gravity;
            }

            acceleration += collider.Acceleration;
            collider.Acceleration = Vector3.zero;
            
            collider.transform.position += rb.Velocity * Time.fixedDeltaTime + 0.5f * Time.fixedDeltaTime * Time.fixedDeltaTime * acceleration;
            return acceleration;
        }

        private void OnDrawGizmosSelected()
        {
            if (_debug && _boundingBoxSize > 0 && _physicsWorldSize > 0)
            {
                foreach (var bound in _bounds)
                {
                    var color = 2 * bound.Center / _physicsWorldSize;
                    Gizmos.color = new Color(color.x, color.y, color.z);
                    Gizmos.DrawWireCube(bound.Center, bound.Size);
                }
            }
        }

        private void GenerateBoundingBoxes()
        {
            if (_boundingBoxSize <= 0 || _physicsWorldSize <= 0) return;
            _bounds.Clear();
            for (int z = -_physicsWorldSize / 2; z < _physicsWorldSize / 2; z += _boundingBoxSize)
            {
                for (int y = -_physicsWorldSize / 2; y < _physicsWorldSize / 2; y += _boundingBoxSize)
                {
                    for (int x = -_physicsWorldSize / 2; x < _physicsWorldSize / 2; x += _boundingBoxSize)
                    {
                        Vector3 extents = new Vector3(_boundingBoxSize / 2, _boundingBoxSize / 2, _boundingBoxSize / 2);
                        Vector3 center = new Vector3(x, y, z) + extents;
                        var aabb = new AA3DBB(extents, center);
                        _bounds.Add(aabb);
                    }
                }
            }
        }
    }
}