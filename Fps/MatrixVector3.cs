using System;
using System.Runtime.InteropServices;

namespace Fps
{
    // some basic 3D constructs
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public Double X;
        public Double Y;
        public Double Z;

        public Vector3(Double x, Double y, Double z)
        {
            X = x; Y = y; Z = z;
        }
        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3 operator +(Vector3 a, Double c)
        {
            return new Vector3(a.X + c, a.Y + c, a.Z + c);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Double operator *(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public static Vector3 operator *(Vector3 a, Double c)
        {
            return new Vector3(a.X * c, a.Y * c, a.Z * c);
        }
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }
        public static Double Abs(Vector3 a)
        {
            return Math.Sqrt(a * a);
        }
        public static Double SquareNormDiff(Vector3 r1, Vector3 r2)
        {
            Double tx = r1.X - r2.X, ty = r1.Y - r2.Y, tz = r1.Z - r2.Z;
            return tx * tx + ty * ty + tz * tz;
        }
        public Double Normalize()
        {
            Double t = Math.Sqrt(X * X + Y * Y + Z * Z), rt = 1.0 / t;
            X *= rt; Y *= rt; Z *= rt;
            return t;
        }
        public override string ToString()
        {
            return string.Format("{0:F3}, {1:F3}, {2:F3}", X, Y, Z);
        }
        public string ToString(char separator)
        {
            return string.Format("{0:F3}{1}{2:F3}{3}{4:F3}", X, separator, Y, separator, Z);
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Matrix3
    {
        public Double XX;
        public Double XY;
        public Double XZ;
        public Double YX;
        public Double YY;
        public Double YZ;
        public Double ZX;
        public Double ZY;
        public Double ZZ;

        public Matrix3(Double xx, Double xy, Double xz, Double yx, Double yy, Double yz,
            Double zx, Double zy, Double zz)
        {
            XX = xx; XY = xy; XZ = xz;
            YX = yx; YY = yy; YZ = yz;
            ZX = zx; ZY = zy; ZZ = zz;
        }

        public static Vector3 operator *(Matrix3 m, Vector3 v)
        {
            return new Vector3(m.XX * v.X + m.XY * v.Y + m.XZ * v.Z, m.YX * v.X + m.YY * v.Y + m.YZ * v.Z, m.ZX * v.X + m.ZY * v.Y + m.ZZ * v.Z);
        }
        public static Matrix3 operator *(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix3(m1.XX * m2.XX + m1.XY * m2.YX + m1.XZ * m2.ZX, m1.XX * m2.XY + m1.XY * m2.YY + m1.XZ * m2.ZY, m1.XX * m2.XZ + m1.XY * m2.YZ + m1.XZ * m2.ZZ,
                               m1.YX * m2.XX + m1.YY * m2.YX + m1.YZ * m2.ZX, m1.YX * m2.XY + m1.YY * m2.YY + m1.YZ * m2.ZY, m1.YX * m2.XZ + m1.YY * m2.YZ + m1.YZ * m2.ZZ,
                               m1.ZX * m2.XX + m1.ZY * m2.YX + m1.ZZ * m2.ZX, m1.ZX * m2.XY + m1.ZY * m2.YY + m1.ZZ * m2.ZY, m1.ZX * m2.XZ + m1.ZY * m2.YZ + m1.ZZ * m2.ZZ);
        }
        public static Matrix3 operator *(Matrix3 m, Double c)
        {
            return new Matrix3(m.XX * c, m.XY * c, m.XZ * c, m.YX * c, m.YY * c, m.YZ * c, m.ZX * c, m.ZY * c, m.ZZ * c);
        }
        public static Matrix3 operator +(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix3(m1.XX + m2.XX, m1.XY + m2.XY, m1.XZ + m2.XZ, 
                m1.YX + m2.YX, m1.YY + m2.YY, m1.YZ + m2.YZ,
                m1.ZX + m2.ZX, m1.ZY + m2.ZY, m1.ZZ + m2.ZZ);
        }
        public static Matrix3 operator -(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix3(m1.XX - m2.XX, m1.XY - m2.XY, m1.XZ - m2.XZ,
                m1.YX - m2.YX, m1.YY - m2.YY, m1.YZ - m2.YZ,
                m1.ZX - m2.ZX, m1.ZY - m2.ZY, m1.ZZ - m2.ZZ);
        }

        public static Matrix3 Rotation(Vector3 u, Double angle)
        {
            Double c = Math.Cos(angle);
            Double c1 = 1 - c;
            Double s = Math.Sin(angle);
            return new Matrix3(c + c1 * u.X * u.X, u.X * u.Y * c1 - u.Z * s, u.X * u.Z * c1 + u.Y * s,
                               u.X * u.Y * c1 + u.Z * s, c + c1 * u.Y * u.Y, u.Y * u.Z * c1 - u.X * s,
                               u.X * u.Z * c1 - u.Y * s, u.Y * u.Z * c1 + u.X * s, c + c1 * u.Z * u.Z);
        }
        public static Double AngleAndAxis(Matrix3 R, out Vector3 u)
        {
            Double t, t1 = (R.ZY - R.YZ), t2 = (R.XZ - R.ZX), t3 = (R.YX - R.XY);
            Double angle = Math.Atan2(Math.Sqrt(t1*t1 + t2*t2 + t3*t3), (R.XX + R.YY + R.ZZ - 1));
            Double s = Math.Sin(angle);
            if (s < 1.0e-8) u = new Vector3(0.0, 0.0, 1.0);
            else
            {
                t = 0.5 / s;
                u = new Vector3(t * (R.ZY - R.YZ), t * (R.XZ - R.ZX), t * (R.YX - R.XY));
            }
            return angle;
        }
        public static Double Angle(Matrix3 R)
        {
            Double t1 = (R.ZY - R.YZ), t2 = (R.XZ - R.ZX), t3 = (R.YX - R.XY);
            return Math.Atan2(Math.Sqrt(t1 * t1 + t2 * t2 + t3 * t3), (R.XX + R.YY + R.ZZ - 1));
        }
        public static Matrix3 RepairRotation(Matrix3 R)
        {
            Vector3 u;
            Double theta = Matrix3.AngleAndAxis(R, out u);
            Double absu = Vector3.Abs(u);
            u = u * (1.0 / absu);
            return Matrix3.Rotation(u, theta);
        }
        public static Matrix3 Transpose(Matrix3 m)
        {
            return new Matrix3(m.XX, m.YX, m.ZX, m.XY, m.YY, m.ZY, m.XZ, m.YZ, m.ZZ);
        }

        public static Matrix3 E = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
    }

}