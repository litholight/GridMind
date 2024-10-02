// src/Utilities/MovementDirection.cs
namespace GridMind.Utilities
{
    public struct MovementDirection
    {
        public int RowOffset { get; }
        public int ColOffset { get; }

        public MovementDirection(int rowOffset, int colOffset)
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }

        // Predefined directions for convenience
        public static readonly MovementDirection Up = new MovementDirection(-1, 0);
        public static readonly MovementDirection Down = new MovementDirection(1, 0);
        public static readonly MovementDirection Left = new MovementDirection(0, -1);
        public static readonly MovementDirection Right = new MovementDirection(0, 1);
        public static readonly MovementDirection None = new MovementDirection(0, 0);

        // Implement IEquatable<MovementDirection>
        public bool Equals(MovementDirection other)
        {
            return this.RowOffset == other.RowOffset && this.ColOffset == other.ColOffset;
        }

        public override bool Equals(object obj)
        {
            if (obj is MovementDirection other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        // Overload equality operators
        public static bool operator ==(MovementDirection left, MovementDirection right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MovementDirection left, MovementDirection right)
        {
            return !(left == right);
        }
    }
}