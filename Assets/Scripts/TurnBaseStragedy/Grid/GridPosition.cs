using System;

namespace TurnBaseStragedy.Grid
{
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int X { get; set; }
        public int Z { get; set; }

        public GridPosition(int x, int z)
        {
            X = x;
            Z = z;
        }

        public override string ToString()
        {
            return $"[X: {X}, Z: {Z}]";
        }
        
        public bool Equals(GridPosition other)
        {
            return X == other.X && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
        }

        public static bool operator ==(GridPosition left, GridPosition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GridPosition left, GridPosition right)
        {
            return !Equals(left, right);
        }

        public static GridPosition operator +(GridPosition left, GridPosition right)
        {
            return new GridPosition(left.X + right.X, left.Z + right.Z);
        }

        public static GridPosition operator -(GridPosition left, GridPosition right)
        {
            return new GridPosition(left.X - right.X, left.Z - right.Z);
        }
    }
}
