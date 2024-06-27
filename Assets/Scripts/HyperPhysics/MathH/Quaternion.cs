namespace HyperPhysics.MathH
{
    public unsafe struct Quaternion
    {
        public fixed float Value[4];

        public float X => Value[1];
        public float Y => Value[2];
        public float Z => Value[3];
        public float W => Value[0];

        public Quaternion(float w, float x, float y, float z)
        {
            Value[0] = w;
            Value[1] = x;
            Value[2] = y;
            Value[3] = z;
        }

        public static Quaternion RotateByVector(Quaternion a, Vector3 eulerAnglesInRadian)
        {
            var rotationQuaternion = new Quaternion(0, eulerAnglesInRadian.X, eulerAnglesInRadian.Y, eulerAnglesInRadian.Z);
            return a * rotationQuaternion;
        }

        public static Quaternion RotateByScaledVector(Quaternion a, Vector3 eulerAnglesInRadian, float scale)
        {
            var rotationQuaternion = new Quaternion(0, eulerAnglesInRadian.X * scale, eulerAnglesInRadian.Y * scale, eulerAnglesInRadian.Z * scale);
            rotationQuaternion *= a;

            a.Value[0] += rotationQuaternion.W * 0.5f;
            a.Value[1] += rotationQuaternion.X * 0.5f;
            a.Value[2] += rotationQuaternion.Y * 0.5f;
            a.Value[3] += rotationQuaternion.Z * 0.5f;

            return a;
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            Quaternion result;
            result.Value[0] = a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z;
            result.Value[1] = a.W * b.X + a.X * b.W - a.Y * b.Z - a.Z * b.Y;
            result.Value[2] = a.W * b.Y - a.X * b.Z + a.Y * b.W - a.Z * b.X;
            result.Value[3] = a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W;

            return result;
        }

        public static Quaternion operator *(float a, Quaternion b)
        {
            for (int i = 0; i < 4; i++)
            {
                b.Value[i] *= a;
            }

            return b;
        }

        public static Quaternion operator *(Quaternion b, float a)
        {
            for (int i = 0; i < 4; i++)
            {
                b.Value[i] *= a;
            }

            return b;
        }

        public static Quaternion operator /(Quaternion b, float a)
        {
            for (int i = 0; i < 4; i++)
            {
                b.Value[i] /= a;
            }

            return b;
        }

        public static Quaternion operator /(float a, Quaternion b)
        {
            for (int i = 0; i < 4; i++)
            {
                b.Value[i] /= a;
            }

            return b;
        }
    }
}