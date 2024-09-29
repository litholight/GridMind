// src/Environment/Grid.cs
using System;

namespace GridMind.Environment
{
    public class Grid
    {
        public int Rows { get; }
        public int Columns { get; }
        private GridCell[,] cells;

        // Constructor to initialize the grid with rows and columns
        public Grid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            cells = new GridCell[rows, columns];
            InitializeGrid();
        }

        // Initializes the grid with empty cells
        private void InitializeGrid()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    cells[i, j] = new GridCell(i, j);
                }
            }
        }

        // GetCell method to access specific cells in the grid
        public GridCell GetCell(int row, int col)
        {
            // Ensure row and col are within the grid boundaries
            if (row < 0 || row >= Rows || col < 0 || col >= Columns)
            {
                throw new ArgumentOutOfRangeException($"Cell ({row}, {col}) is outside the grid boundaries.");
            }
            return cells[row, col];
        }
    }
}