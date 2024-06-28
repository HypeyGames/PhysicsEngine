using System;
using UnityEngine;

namespace HyperPhysics
{
    public class Tester : MonoBehaviour
    {
        [SerializeField] private float[] values=new float[4];
        private void Update()
        {
            var rotation = transform.rotation;
            values[0] = rotation.w;
            values[1] = rotation.x;
            values[2] = rotation.y;
            values[3] = rotation.z;
        }
    }
    
    
}