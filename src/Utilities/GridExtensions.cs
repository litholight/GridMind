// src/Utilities/GridExtensions.cs
using GridMind.Environment;

namespace GridMind.Utilities
{
    public static class GridExtensions
    {
        public static bool IsWithinRadius(this GridCell cell, GridCell origin, int radius)
        {
            int rowDist = Math.Abs(cell.Row - origin.Row);
            int colDist = Math.Abs(cell.Column - origin.Column);
            return rowDist <= radius && colDist <= radius;
        }
    }
}