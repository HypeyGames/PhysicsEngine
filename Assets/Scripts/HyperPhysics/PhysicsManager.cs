using System;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace HyperPhysics
{
    [BurstCompile]
    public class PhysicsManager : MonoBehaviour
    {
        [SerializeField] private int _boundingBoxSize;
        [SerializeField] private int _physicsWorldSize;
        [SerializeField] private List<AA3DBB> _bounds = new List<AA3DBB>();
        [SerializeField, Range(.01f, 10)] private float _sleepThreshold;
        [SerializeField, Range(1, 10)] private int _collisionSubstep;
        [SerializeField] private bool _debug;

        private readonly List<Collision> _collisions = new(512 * 512);

        private List<Collider> _colliders;

        public event Action Update;
        public event Action PostUpdate;

        #region Setup

        public void RegisterSceneColliders()
        {
            _colliders = new List<Collider>();
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
       //     GenerateBoundingBoxes();
        }

        // private void GenerateBoundingBoxes()
        // {
        //     if (_boundingBoxSize <= 0 || _physicsWorldSize <= 0) return;
        //     _bounds.Clear();
        //     for (int z = -_physicsWorldSize / 2; z < _physicsWorldSize / 2; z += _boundingBoxSize)
        //     {
        //         for (int y = -_physicsWorldSize / 2; y < _physicsWorldSize / 2; y += _boundingBoxSize)
        //         {
        //             for (int x = -_physicsWorldSize / 2; x < _physicsWorldSize / 2; x += _boundingBoxSize)
        //             {
        //                 Vector3 extents = new Vector3(_boundingBoxSize / 2, _boundingBoxSize / 2, _boundingBoxSize / 2);
        //                 Vector3 center = new Vector3(x, y, z) + extents;
        //                 var aabb = new AA3DBB(extents, center);
        //                 _bounds.Add(aabb);
        //             }
        //         }
        //     }
        // }

        #endregion

        private void Awake()
        {
            _colliders = HyperPhysics.Instance.GetCollider();
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

                _colliders[i].ResetCollider();
                ApplyMotion(_colliders[i]);
                DetectCollision(i);
            }

            if (_collisions.Count > 0)
                ResolveCollision();

            CollisionPostProcess();
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


            var rotation = rb.AngularVelocity * Time.fixedDeltaTime + 0.5f * Time.fixedDeltaTime * Time.fixedDeltaTime * collider.Rigidbody.AngularAcceleration;
            var rotationQuaternion = Quaternion.Euler(rotation);
            collider.Rotation *= rotationQuaternion;
        }

        private void DetectCollision(int i)
        {
            for (int j = 0; j < _colliders.Count; j++)
            {
                if (i == j) continue;
                if (_colliders[i].CheckForOverlap(_colliders[j]) == false) continue;
                var collision = _colliders[i].CheckForCollision(_colliders[j]);
                if (collision.CollisionType == CollisionType.NotValid) continue;

                _colliders[i].OnPostCollision(_colliders[j], collision);

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
                // TODO find a way to do this without GC allocation.
                // if (_collisions.Count > 1)
                //     _collisions.Sort(SortByCollisions);
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
            if (collision.CollisionType != CollisionType.NotValid)
            {
                if (collision.Penetration <= 0) return;
                float accelerationAlongNormal;
                var body2 = collision.Body2;
                var body1 = collision.Body1;
                switch (collision.CollisionType)
                {
                    case CollisionType.StaticDynamic:

                        accelerationAlongNormal = Vector3.Dot(body2.Rigidbody.Acceleration, collision.Normal);
                        // Resolving penetration.
                        body2.Position += collision.Normal * collision.Penetration;
                        if (collision.CollisionVelocity < 0) break;
                        if (Mathf.Abs(collision.CollisionVelocity) < _sleepThreshold + accelerationAlongNormal * Time.fixedDeltaTime)
                        {
                            body2.Rigidbody.Velocity -= collision.CollisionVelocity * collision.Normal;
                        }
                        else
                        {
                            body2.Rigidbody.Velocity -= 2 * body1.Bounciness * body2.Bounciness * collision.CollisionVelocity * collision.Normal;
                        }

                        break;

                    case CollisionType.DynamicStatic:

                        accelerationAlongNormal = Vector3.Dot(body1.Rigidbody.Acceleration, collision.Normal);
                        body1.Position -= collision.Penetration * collision.Normal;
                        if (collision.CollisionVelocity < 0) break;
                        if (collision.CollisionVelocity < _sleepThreshold + Mathf.Abs(accelerationAlongNormal) * Time.fixedDeltaTime)
                        {
                            body1.Rigidbody.Velocity -= collision.CollisionVelocity * collision.Normal;
                        }
                        else
                        {
                            body1.Rigidbody.Velocity -= 2 * body1.Bounciness * body2.Bounciness * collision.CollisionVelocity * collision.Normal;
                        }

                        break;

                    case CollisionType.DynamicDynamic:

                        body1.Position -= collision.Normal * (collision.Penetration * collision.MassRatio21);
                        body2.Position += collision.Normal * (collision.Penetration * collision.MassRatio12);

                        if (collision.CollisionVelocity < 0) break;

                        accelerationAlongNormal = Vector3.Dot(collision.Body1.Rigidbody.Acceleration, collision.Normal);
                        var velocityGained = Mathf.Abs(accelerationAlongNormal * Time.deltaTime);
                        accelerationAlongNormal = Vector3.Dot(collision.Body2.Rigidbody.Acceleration, collision.Normal);
                        velocityGained += Mathf.Abs(accelerationAlongNormal * Time.deltaTime);
                        var velocity = collision.CollisionVelocity * collision.Normal;
                        if (Mathf.Abs(collision.CollisionVelocity) < _sleepThreshold + velocityGained)
                        {
                            // Not an accurate inelastic collision but fair enough.
                            collision.Body1.Rigidbody.Velocity = collision.Body2.Rigidbody.Velocity + 0.5f * (collision.MassRatio12 - collision.MassRatio21) * velocity;
                            collision.Body2.Rigidbody.Velocity += collision.MassRatio12 * velocity;
                            break;
                        }

                        collision.Body1.Rigidbody.Velocity = collision.Body2.Rigidbody.Velocity + (collision.MassRatio12 - collision.MassRatio21) * velocity;
                        collision.Body2.Rigidbody.Velocity += 2 * collision.MassRatio12 * velocity;
                        break;
                }
            }
        }

        private void CollisionPostProcess()
        {
            for (int i = 0; i < _colliders.Count; i++)
            {
                if (_colliders[i].Static)
                {
                    continue;
                }

                var collisions = _colliders[i].Collisions;
                var maxPenetration = 0f;
                for (int j = 0; j < collisions.Count; j++)
                {
                    maxPenetration = Mathf.Max(collisions[j].Penetration, maxPenetration);
                }

                if (collisions.Count > 0 && _colliders[i].Rigidbody.Velocity.magnitude < _sleepThreshold)
                {
                    _colliders[i].Rigidbody.Velocity = Vector3.zero;
                    _colliders[i].Position = _colliders[i].transform.position;
                }
                else
                {
                    _colliders[i].transform.position = _colliders[i].Position;
                }

                _colliders[i].transform.rotation = _colliders[i].Rotation;

                _colliders[i].Rigidbody.Acceleration = Vector3.zero;
            }
        }

        #region Debug

        private void OnDrawGizmosSelected()
        {
            // if (_debug && _boundingBoxSize > 0 && _physicsWorldSize > 0)
            // {
            //     foreach (var bound in _bounds)
            //     {
            //         var color = (2 * bound.Center / _physicsWorldSize);
            //         Gizmos.color = new Color(color.x, color.y, color.z);
            //         Gizmos.DrawWireCube(bound.Center, bound.Size);
            //     }
            // }
        }

        #endregion
    }
}