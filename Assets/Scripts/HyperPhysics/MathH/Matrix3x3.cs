namespace HyperPhysics.MathH
{
    public unsafe struct Matrix3X3
    {
        /// <summary>
        ///  | 0 3 6 |
        ///  | 1 4 7 |
        ///  | 2 5 8 |
        /// </summary>
        public fixed float Value[9];

        public static Matrix3X3 operator *(Matrix3X3 a, Matrix3X3 b)
        {
            Matrix3X3 mat3X3;

            mat3X3.Value[0] = a.Value[0] * b.Value[0] + a.Value[3] * b.Value[1] + a.Value[6] * b.Value[2];
            mat3X3.Value[1] = a.Value[1] * b.Value[0] + a.Value[4] * b.Value[1] + a.Value[7] * b.Value[2];
            mat3X3.Value[2] = a.Value[2] * b.Value[0] + a.Value[5] * b.Value[1] + a.Value[8] * b.Value[2];

            mat3X3.Value[3] = a.Value[0] * b.Value[3] + a.Value[3] * b.Value[4] + a.Value[6] * b.Value[5];
            mat3X3.Value[4] = a.Value[1] * b.Value[3] + a.Value[4] * b.Value[4] + a.Value[7] * b.Value[5];
            mat3X3.Value[5] = a.Value[2] * b.Value[3] + a.Value[5] * b.Value[4] + a.Value[8] * b.Value[5];

            mat3X3.Value[6] = a.Value[0] * b.Value[6] + a.Value[3] * b.Value[7] + a.Value[6] * b.Value[8];
            mat3X3.Value[7] = a.Value[1] * b.Value[6] + a.Value[4] * b.Value[7] + a.Value[7] * b.Value[8];
            mat3X3.Value[8] = a.Value[2] * b.Value[6] + a.Value[5] * b.Value[7] + a.Value[8] * b.Value[8];

            return mat3X3;
        }

        public static Matrix3X3 Identity()
        {
            Matrix3X3 mat3X3;

            mat3X3.Value[0] = 1;
            mat3X3.Value[4] = 1;
            mat3X3.Value[8] = 1;

            return mat3X3;
        }
    }

    public unsafe struct Matrix4X4
    {
        /// <summary>
        ///  | 00 04 08 12 |
        ///  | 01 05 09 13 |
        ///  | 02 06 10 14 |
        ///  | 03 07 11 15 |
        /// </summary>
        public fixed float Value[16];

        public static Matrix4X4 operator *(Matrix4X4 a, Matrix4X4 b)
        {
            Matrix4X4 matrix4X4;

            matrix4X4.Value[0] = a.Value[0] * b.Value[0] + a.Value[4] * b.Value[1] + a.Value[08] * b.Value[2] * a.Value[12] * b.Value[3];
            matrix4X4.Value[1] = a.Value[1] * b.Value[0] + a.Value[5] * b.Value[1] + a.Value[09] * b.Value[2] * a.Value[13] * b.Value[3];
            matrix4X4.Value[2] = a.Value[2] * b.Value[0] + a.Value[6] * b.Value[1] + a.Value[10] * b.Value[2] * a.Value[14] * b.Value[3];
            matrix4X4.Value[3] = a.Value[3] * b.Value[0] + a.Value[7] * b.Value[1] + a.Value[11] * b.Value[2] * a.Value[15] * b.Value[3];

            matrix4X4.Value[4] = a.Value[0] * b.Value[4] + a.Value[4] * b.Value[5] + a.Value[08] * b.Value[6] * a.Value[12] * b.Value[7];
            matrix4X4.Value[5] = a.Value[1] * b.Value[4] + a.Value[5] * b.Value[5] + a.Value[09] * b.Value[6] * a.Value[13] * b.Value[7];
            matrix4X4.Value[6] = a.Value[2] * b.Value[4] + a.Value[6] * b.Value[5] + a.Value[10] * b.Value[6] * a.Value[14] * b.Value[7];
            matrix4X4.Value[7] = a.Value[3] * b.Value[4] + a.Value[7] * b.Value[5] + a.Value[11] * b.Value[6] * a.Value[15] * b.Value[7];

            matrix4X4.Value[08] = a.Value[0] * b.Value[8] + a.Value[4] * b.Value[9] + a.Value[08] * b.Value[10] * a.Value[12] * b.Value[11];
            matrix4X4.Value[09] = a.Value[1] * b.Value[8] + a.Value[5] * b.Value[9] + a.Value[09] * b.Value[10] * a.Value[13] * b.Value[11];
            matrix4X4.Value[10] = a.Value[2] * b.Value[8] + a.Value[6] * b.Value[9] + a.Value[10] * b.Value[10] * a.Value[14] * b.Value[11];
            matrix4X4.Value[11] = a.Value[3] * b.Value[8] + a.Value[7] * b.Value[9] + a.Value[11] * b.Value[10] * a.Value[15] * b.Value[11];

            matrix4X4.Value[12] = a.Value[0] * b.Value[12] + a.Value[4] * b.Value[13] + a.Value[08] * b.Value[14] * a.Value[12] * b.Value[15];
            matrix4X4.Value[13] = a.Value[1] * b.Value[12] + a.Value[5] * b.Value[13] + a.Value[09] * b.Value[14] * a.Value[13] * b.Value[15];
            matrix4X4.Value[14] = a.Value[2] * b.Value[12] + a.Value[6] * b.Value[13] + a.Value[10] * b.Value[14] * a.Value[14] * b.Value[15];
            matrix4X4.Value[15] = a.Value[3] * b.Value[12] + a.Value[7] * b.Value[13] + a.Value[11] * b.Value[14] * a.Value[15] * b.Value[15];

            return matrix4X4;
        }

        public static Matrix4X4 Identity()
        {
            Matrix4X4 mat3X3;

            mat3X3.Value[00] = 1;
            mat3X3.Value[05] = 1;
            mat3X3.Value[10] = 1;
            mat3X3.Value[15] = 1;

            return mat3X3;
        }
    }
}