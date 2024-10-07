// src/Configuration/GridInitializer.cs
using System;
using GridMind.Agents;
using GridMind.Environment;
using GridMind.Navigation;

namespace GridMind.Configuration
{
    public class GridInitializer
    {
        private readonly Random random = new Random();

        public (Grid, Agent) InitializeGrid(
            int rows,
            int columns,
            int numberOfObstacles,
            IMovementStrategy strategy
        )
        {
            // Create the grid
            var grid = new Grid(rows, columns);

            // Set up random start and goal cells
            var startCell = GetRandomCell(grid);
            var goalCell = GetRandomCell(grid);

            // Ensure start and goal are not the same
            while (startCell == goalCell)
            {
                goalCell = GetRandomCell(grid);
            }

            // Set the start and goal cells
            startCell.Type = CellType.Start;
            goalCell.Type = CellType.Goal;

            // Randomly place obstacles
            PlaceObstacles(grid, startCell, goalCell, numberOfObstacles);

            // Create and initialize the agent
            var agent = new Agent("Explorer", startCell, goalCell);
            agent.SetMovementStrategy(strategy);

            return (grid, agent);
        }

        private GridCell GetRandomCell(Grid grid)
        {
            int row = random.Next(0, grid.Rows);
            int col = random.Next(0, grid.Columns);
            return grid.GetCell(row, col);
        }

        private void PlaceObstacles(
            Grid grid,
            GridCell startCell,
            GridCell goalCell,
            int numberOfObstacles
        )
        {
            for (int i = 0; i < numberOfObstacles; i++)
            {
                int obstacleRow,
                    obstacleCol;
                GridCell cell;

                do
                {
                    obstacleRow = random.Next(0, grid.Rows);
                    obstacleCol = random.Next(0, grid.Columns);
                    cell = grid.GetCell(obstacleRow, obstacleCol);
                } while (cell == startCell || cell == goalCell || cell.Type == CellType.Obstacle);

                // Place obstacle
                cell.Type = CellType.Obstacle;
            }
        }
    }
}
