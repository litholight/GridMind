// src/Agents/Agent.cs
using System.Collections.Generic;
using GridMind.Environment;
using GridMind.Navigation;

namespace GridMind.Agents
{
    public class Agent
    {
        public string Name { get; }
        public GridCell Position { get; set; }
        public GridCell? Goal { get; set; }
        public FogOfWar FogOfWar { get; }
        private IMovementStrategy movementStrategy;

        // New property to track explored cells
        public HashSet<GridCell> ExploredCells { get; }

        public Agent(string name, GridCell startPosition, GridCell? goal = null)
        {
            Name = name;
            Position = startPosition;
            Goal = goal;
            movementStrategy = new RandomWalkStrategy();  // Default strategy

            // Initialize ExploredCells with the starting position
            ExploredCells = new HashSet<GridCell> { startPosition };
            
            // Initialize FogOfWar
            FogOfWar = new FogOfWar();
            FogOfWar.CellExplored(startPosition);
        }

        public void SetMovementStrategy(IMovementStrategy strategy)
        {
            movementStrategy = strategy;
        }

        public void Move(Grid grid)
        {
            Position = movementStrategy.NextMove(grid, this);

            // Add the new position to ExploredCells
            if (ExploredCells.Add(Position))
            {
                FogOfWar.CellExplored(Position); // Notify observers
            }

            // Get visible cells and mark them as explored
            var visibleCells = GetVisibleCells(grid);
            foreach (var cell in visibleCells)
            {
                if (ExploredCells.Add(cell))
                {
                    FogOfWar.CellExplored(cell); // Notify observers
                }
            }

            // Check if the agent has reached the goal
            if (Position == Goal)
            {
                System.Console.WriteLine($"{Name} has reached the goal at ({Position.Row}, {Position.Column})!");
            }
        }

        // Method to get visible cells
        public List<GridCell> GetVisibleCells(Grid grid, int visibilityRadius = 1)
        {
            var visibleCells = new List<GridCell>();
            for (int row = Position.Row - visibilityRadius; row <= Position.Row + visibilityRadius; row++)
            {
                for (int col = Position.Column - visibilityRadius; col <= Position.Column + visibilityRadius; col++)
                {
                    if (row >= 0 && row < grid.Rows && col >= 0 && col < grid.Columns)
                    {
                        var cell = grid.GetCell(row, col);
                        visibleCells.Add(cell);
                        ExploredCells.Add(cell); // Mark the cell as explored
                    }
                }
            }
            return visibleCells;
        }
    }
}
