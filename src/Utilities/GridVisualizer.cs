// src/Utilities/GridVisualizer.cs
using System;
using GridMind.Environment;
using GridMind.Agents;

namespace GridMind.Utilities
{
    public static class GridVisualizer
    {
        public static void PrintGrid(Grid grid, Agent agent)
        {
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Columns; col++)
                {
                    if (agent.Position.Row == row && agent.Position.Column == col)
                    {
                        Console.Write("A ");  // Represent the agent as "A"
                    }
                    else
                    {
                        var cell = grid.GetCell(row, col);
                        switch (cell.Type)
                        {
                            case CellType.Start:
                                Console.Write("S ");
                                break;
                            case CellType.Goal:
                                Console.Write("G ");
                                break;
                            case CellType.Obstacle:
                                Console.Write("X ");
                                break;
                            default:
                                Console.Write(". ");  // Empty cells
                                break;
                        }
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();  // Add some spacing
        }
    }
}