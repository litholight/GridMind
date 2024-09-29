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
    }
}