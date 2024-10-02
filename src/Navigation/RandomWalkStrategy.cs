// src/Navigation/RandomWalkStrategy.cs
using GridMind.Environment;
using GridMind.Agents;
using GridMind.Utilities;

namespace GridMind.Navigation
{
    public class RandomWalkStrategy : IMovementStrategy
    {
        private static readonly MovementDirection[] Directions =
        {
            MovementDirection.Up,
            MovementDirection.Down,
            MovementDirection.Left,
            MovementDirection.Right
        };

        private readonly Random random = new Random();

        public GridCell NextMove(Grid grid, Agent agent)
        {
            var visibleCells = agent.GetVisibleCells(grid);

            // Filter possible moves to visible and navigable cells
            var possibleMoves = Directions
                .Select(dir =>
                {
                    int newRow = agent.Position.Row + dir.RowOffset;
                    int newCol = agent.Position.Column + dir.ColOffset;
                    if (newRow >= 0 && newRow < grid.Rows && newCol >= 0 && newCol < grid.Columns)
                    {
                        var nextCell = grid.GetCell(newRow, newCol);
                        if (nextCell.Type != CellType.Obstacle && visibleCells.Contains(nextCell))
                            return nextCell;
                    }
                    return null;
                })
                .Where(cell => cell != null)
                .ToList();

            if (possibleMoves.Any())
            {
                // Choose a random move from possible moves
                return possibleMoves[random.Next(possibleMoves.Count)];
            }

            // No valid moves, stay in place
            return agent.Position;
        }
    }
}