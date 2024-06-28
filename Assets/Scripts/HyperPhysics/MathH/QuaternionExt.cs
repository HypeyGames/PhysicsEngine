using System;

namespace HyperPhysics.MathH
{
    public static class QuaternionExt
    {
        public static Quaternion Normalize(this Quaternion input)
        {
            var sqrMagnitude = input.Z * input.Z + input.Y * input.Y + input.X * input.X + input.W * input.W;
            if (sqrMagnitude == 0)
            {
                return new Quaternion(1, 0, 0, 0);
            }

            return input / MathF.Sqrt(sqrMagnitude);
        }

        public static UnityEngine.Quaternion ToUnityQuaternion(this Quaternion input)
        {
            input = input.Normalize();
            var unityQuaternion = new UnityEngine.Quaternion(input.X, input.Y, input.Z, input.W);
            return unityQuaternion;
        }

        public static Quaternion FromUnityQuaternion(this UnityEngine.Quaternion input)
        {
            var unityQuaternion = new Quaternion(input.w, input.x, input.y, input.z);
            return unityQuaternion;
        }
    }
}