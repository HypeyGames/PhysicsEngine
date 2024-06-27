namespace HyperPhysics.MathH
{
    public static class MatrixExt
    {
        public static unsafe Matrix3X3 Transpose(this Matrix3X3 input)
        {
            Matrix3X3 output;

            output.Value[1] = input.Value[3];
            output.Value[2] = input.Value[6];

            output.Value[3] = input.Value[1];
            output.Value[6] = input.Value[2];

            output.Value[7] = input.Value[5];
            output.Value[5] = input.Value[7];

            return output;
        }
        
        public static unsafe Matrix4X4 Transpose(this Matrix4X4 input)
        {
            Matrix4X4 output;

            output.Value[1] = input.Value[4];
            output.Value[4] = input.Value[1];

            output.Value[2] = input.Value[8];
            output.Value[8] = input.Value[2];

            output.Value[3] = input.Value[12];
            output.Value[12] = input.Value[3];

            output.Value[6] = input.Value[9];
            output.Value[9] = input.Value[6];

            output.Value[7] = input.Value[13];
            output.Value[13] = input.Value[7];

            output.Value[11] = input.Value[14];
            output.Value[14] = input.Value[11];

            return output;
        }
        
        public static unsafe float Determinant(this Matrix3X3 input)
        {
            var a = input.Value[0] * input.Value[4] * input.Value[8] + input.Value[3] * input.Value[7] * input.Value[2] + input.Value[6] * input.Value[1] * input.Value[5];
            var b = input.Value[2] * input.Value[4] * input.Value[6] + input.Value[5] * input.Value[7] * input.Value[0] + input.Value[8] * input.Value[1] * input.Value[3];

            return a - b;
        }
    }
}