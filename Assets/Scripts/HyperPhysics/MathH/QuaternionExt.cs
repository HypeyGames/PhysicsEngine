using System;

namespace HyperPhysics.MathH
{
    public static class QuaternionExt
    {
        public static unsafe Quaternion Normalize(this Quaternion input)
        {
            var sqrMagnitude = input.Z * input.Z + input.Y * input.Y + input.X * input.X + input.W * input.W;
            return input / MathF.Sqrt(sqrMagnitude);
        }
    }
}