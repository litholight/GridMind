// src/Program.cs
using System;
using GridMind.Environment;
using GridMind.Agents;

namespace GridMind
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a 5x5 grid
            Grid grid = new Grid(5, 5);

            // Set up start and goal positions
            GridCell startCell = grid.GetCell(0, 0);
            GridCell goalCell = grid.GetCell(4, 4);
            
            startCell.Type = CellType.Start;
            goalCell.Type = CellType.Goal;

            // Place some obstacles
            grid.GetCell(2, 2).Type = CellType.Obstacle;
            grid.GetCell(3, 2).Type = CellType.Obstacle;

            // Create an agent and assign the goal position
            Agent agent = new Agent("Explorer", startCell, goalCell);

            // Print the initial grid
            Console.WriteLine("Initial Grid:");
            GridMind.Utilities.GridVisualizer.PrintGrid(grid, agent);

            // Let the agent explore a few moves
            for (int i = 0; i < 10; i++)
            {
                agent.Explore(grid);
                System.Threading.Thread.Sleep(500);  // Pause briefly to simulate real-time movement
            }
        }
    }
}