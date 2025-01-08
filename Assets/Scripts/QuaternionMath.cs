
using System.Numerics;

internal class QuaternionMath
{
    public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount)
    {
        float w = 0.0f;
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        if(!AreQuaternionsClose(newRotation, firstRotation))
        {
            newRotation = InverseSignQuaternion(newRotation);
        }

        float addDet = 1f / (float)addAmount;
        cumulative.W += newRotation.W;
        w = cumulative.W * addDet;
        cumulative.X += newRotation.X;
        x = cumulative.X * addDet;
        cumulative.Y += newRotation.Y;
        y = cumulative.Y * addDet;
        cumulative.Z += newRotation.Z;
        z = cumulative.Z * addDet;

        //return NormalizeQuaternion(x, y, z, w);
        return new Quaternion(x, y, z, w);
    }

    public static Quaternion NormalizeQuaternion(float x, float y, float z, float w)
    {
        float lengthD = 1.0f / (w * w + x * x + y * y + z * z);
        w *= lengthD;
        x *= lengthD;
        y *= lengthD;
        z *= lengthD;

        return new Quaternion(x, y, z, w);
    }

    public static Quaternion InverseSignQuaternion(Quaternion q)
    {
        return new Quaternion(-q.X, -q.Y, -q.Z, -q.W);
    }

    public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2)
    {
        float dot = Quaternion.Dot(q1, q2);

        if(dot < 0.0f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

