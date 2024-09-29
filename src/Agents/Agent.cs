// src/Agents/Agent.cs
using GridMind.Environment;
using GridMind.Utilities;

namespace GridMind.Agents
{
    public class Agent
    {
        public string Name { get; }
        public GridCell Position { get; set; }
        
        public GridCell? Goal { get; set; }  // The agent's goal position

        // Available movement directions (no diagonals)
        private static readonly MovementDirection[] Directions = 
        { 
            MovementDirection.Up, 
            MovementDirection.Down, 
            MovementDirection.Left, 
            MovementDirection.Right 
        };

        public Agent(string name, GridCell startPosition, GridCell? goal = null)
        {
            Name = name;
            Position = startPosition;
            Goal = goal;
        }

        // Check if a move is valid (no out-of-bounds or obstacles)
        public bool CanMoveTo(Grid grid, MovementDirection direction)
        {
            int newRow = Position.Row + direction.RowOffset;
            int newCol = Position.Column + direction.ColOffset;

            // Check if within grid boundaries
            if (newRow < 0 || newRow >= grid.Rows || newCol < 0 || newCol >= grid.Columns)
                return false;

            // Check if the target cell is not an obstacle
            return grid.GetCell(newRow, newCol).Type != CellType.Obstacle;
        }

        // Move based on relative direction
        public void Move(Grid grid, MovementDirection direction)
        {
            if (CanMoveTo(grid, direction))
            {
                // Update position
                Position = grid.GetCell(Position.Row + direction.RowOffset, Position.Column + direction.ColOffset);
                System.Console.WriteLine($"{Name} moved to position ({Position.Row}, {Position.Column}).");
                GridVisualizer.PrintGrid(grid, this);  // Print the updated grid
            }
            else
            {
                System.Console.WriteLine($"{Name} cannot move {direction} (blocked or out of bounds).");
            }
        }

        // A basic exploration algorithm: try to move in a random valid direction
        public void Explore(Grid grid)
        {
            var random = new Random();
            MovementDirection randomDirection = Directions[random.Next(Directions.Length)];

            Move(grid, randomDirection);  // Attempt to move in a random direction
        }
    }
}
