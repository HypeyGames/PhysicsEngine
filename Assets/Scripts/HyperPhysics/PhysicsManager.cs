using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace HyperPhysics
{
    public class PhysicsManager : MonoBehaviour
    {
        public static PhysicsManager Instance;

        [SerializeField] private List<Collider> _colliders = new List<Collider>(256);

        [SerializeField] private int _boundingBoxSize;
        [SerializeField] private int _physicsWorldSize;
        [SerializeField] private List<AA3DBB> _bounds = new List<AA3DBB>();
        [SerializeField, Range(.01f, 10)] private float _sleepThreshold;
        [SerializeField, Range(1, 10)] private int _collisionSubstep;
        [SerializeField] private bool _debug;

        private List<Collision> _collisions = new List<Collision>(256);

        public event Action Update;
        public event Action PostUpdate;

        #region Setup

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

        #endregion

        private void Awake()
        {
            Instance = this;
            _colliders.Clear();
        }

        public void AddCollider(Collider collider)
        {
            _colliders.Add(collider);
        }

        public void RemoveCollider(Collider collider)
        {
            _colliders.Remove(collider);
        }

        private void FixedUpdate()
        {
            Update?.Invoke();

            _collisions.Clear();
            for (int i = 0; i < _colliders.Count; i++)
            {
                if (_colliders[i].Static)
                {
                    continue;
                }

                ApplyMotion(_colliders[i]);
                DetectCollision(i);
            }

            if (_collisions.Count > 0)
                ResolveCollision();

            UpdateVisual();
            PostUpdate?.Invoke();
        }

        private void ApplyMotion(Collider collider)
        {
            var rb = collider.Rigidbody;

            var acceleration = rb.Force;
            if (rb.Gravity)
            {
                acceleration += Physics.gravity;
            }

            collider.Position += rb.Velocity * Time.fixedDeltaTime + 0.5f * Time.fixedDeltaTime * Time.fixedDeltaTime * acceleration;
            rb.Velocity += acceleration * Time.fixedDeltaTime;
            rb.Acceleration = acceleration;
        }

        private void DetectCollision(int i)
        {
            for (int j = 0; j < _colliders.Count; j++)
            {
                if (i == j) continue;
                if (_colliders[i].CheckForOverlap(_colliders[j]) == false) continue;
                var collision = _colliders[i].CheckForCollision(_colliders[j]);
                if (collision.CollisionType == CollisionType.NotValid) continue;
                collision.SetCollisionVelocity();
                _collisions.Add(collision);
            }
        }

        private void ResolveCollision()
        {
            var stepCount = 0;
            bool fullCollided = true;
            do
            {
                stepCount++;
                if (_collisions.Count > 1)
                    _collisions.Sort(SortByCollisions);
                fullCollided = true;
                for (int i = 0; i < _collisions.Count; i++)
                {
                    ProcessCollision(_collisions[i]);
                    if (i < _collisions.Count - 1)
                    {
                        var collision = _collisions[i + 1];
                        collision.SetCollisionVelocity();
                        collision.UpdatePenetration();
                        _collisions[i + 1] = collision;
                    }
                }


                for (int i = 0; i < _collisions.Count; i++)
                {
                    var collision = _collisions[i];
                    collision.SetCollisionVelocity();
                    collision.UpdatePenetration();
                    _collisions[i] = collision;
                    fullCollided &= collision.CollisionVelocity < _sleepThreshold;
                    fullCollided &= collision.Penetration <= 0;
                }
            } while (fullCollided == false && stepCount < _collisionSubstep);
        }

        private int SortByCollisions(Collision x, Collision y)
        {
            if (y.CollisionVelocity < _sleepThreshold && y.CollisionVelocity >= _sleepThreshold) return -1;
            if (x.CollisionVelocity < _sleepThreshold && x.CollisionVelocity >= _sleepThreshold) return 1;

            if (x.CollisionVelocity < _sleepThreshold && x.CollisionVelocity < _sleepThreshold)
            {
                if (x.Penetration > y.Penetration) return -1;
                if (x.Penetration < y.Penetration) return 1;
            }

            if (x.CollisionVelocity > y.CollisionVelocity) return -1;
            if (x.CollisionVelocity < y.CollisionVelocity) return 1;

            return 0;
        }

        private void ProcessCollision(Collision collision)
        {
            if (collision.CollisionType != CollisionType.NotValid && (collision.Penetration > 0 || collision.CollisionVelocity > 0))
            {
                float accelerationAlongNormal;
                switch (collision.CollisionType)
                {
                    case CollisionType.StaticDynamic:

                        accelerationAlongNormal = Vector3.Dot(collision.Body2.Rigidbody.Acceleration, collision.Normal);
                        collision.Body2.Position += collision.Normal * collision.Penetration;
                        if (Mathf.Abs(collision.CollisionVelocity) < _sleepThreshold + accelerationAlongNormal * Time.fixedDeltaTime)
                        {
                            collision.Body2.Rigidbody.Velocity -= collision.CollisionVelocity * collision.Normal;
                        }
                        else
                        {
                            collision.Body2.Rigidbody.Velocity -= 2 * collision.CollisionVelocity * collision.Normal;
                        }

                        break;

                    case CollisionType.DynamicStatic:

                        accelerationAlongNormal = Vector3.Dot(collision.Body1.Rigidbody.Acceleration, collision.Normal);
                        collision.Body1.Position -= collision.Penetration * collision.Normal;
                        if (collision.CollisionVelocity < _sleepThreshold + Mathf.Abs(accelerationAlongNormal) * Time.fixedDeltaTime)
                        {
                            collision.Body1.Rigidbody.Velocity -= collision.CollisionVelocity * collision.Normal;
                        }
                        else
                        {
                            collision.Body1.Rigidbody.Velocity -= 2 * collision.CollisionVelocity * collision.Normal;
                        }

                        break;

                    case CollisionType.DynamicDynamic:

                        collision.Body1.Position -= collision.Normal * (collision.Penetration * collision.MassRatio21);
                        collision.Body2.Position += collision.Normal * (collision.Penetration * collision.MassRatio12);
                        
                        if (Mathf.Abs(collision.CollisionVelocity) < _sleepThreshold)
                        {
                            // TODO : Fix bodies under constant force penetrating each other.
                            break;
                        }

                        var velocity = collision.CollisionVelocity * collision.Normal;
                        collision.Body1.Rigidbody.Velocity = collision.Body2.Rigidbody.Velocity + (collision.MassRatio12 - collision.MassRatio21) * velocity;
                        collision.Body2.Rigidbody.Velocity += 2 * collision.MassRatio12 * velocity;
                        break;
                }
            }
        }

        private void UpdateVisual()
        {
            for (int i = 0; i < _colliders.Count; i++)
            {
                if (_colliders[i].Static)
                {
                    continue;
                }

                if (_colliders[i].Rigidbody.Velocity.magnitude < _sleepThreshold)
                {
                    _colliders[i].Rigidbody.Velocity = Vector3.zero;
                    _colliders[i].Position = _colliders[i].transform.position;
                }
                else
                {
                    _colliders[i].transform.position = _colliders[i].Position;
                }


                _colliders[i].Rigidbody.Acceleration = Vector3.zero;
            }
        }

        #region Debug

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

        #endregion
    }
}