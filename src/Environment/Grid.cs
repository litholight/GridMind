// src/Environment/Grid.cs
using System.Collections.Generic;

namespace GridMind.Environment
{
    public class Grid
    {
        public int Rows { get; }
        public int Columns { get; }
        private readonly GridCell[,] cells;

        public Grid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            cells = new GridCell[rows, columns];
            InitializeCells();
        }

        private void InitializeCells()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    cells[row, col] = new GridCell(row, col);
                }
            }
        }

        public GridCell GetCell(int row, int col)
        {
            return cells[row, col];
        }

        public GridCell GetWrappedCell(int row, int col)
        {
            int wrappedRow = GetWrappedRow(row);
            int wrappedCol = GetWrappedColumn(col);
            return GetCell(wrappedRow, wrappedCol);
        }

        public int GetWrappedRow(int row)
        {
            return (row + Rows) % Rows; // Wrap around vertically
        }

        public int GetWrappedColumn(int col)
        {
            return (col + Columns) % Columns; // Wrap around horizontally
        }
    }
}
