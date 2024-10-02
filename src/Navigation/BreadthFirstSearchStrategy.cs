// src/Navigation/BreadthFirstSearchStrategy.cs
using System.Collections.Generic;
using System.Linq;
using GridMind.Environment;
using GridMind.Agents;
using GridMind.Utilities;

namespace GridMind.Navigation
{
    public class BreadthFirstSearchStrategy : IMovementStrategy
    {
        public GridCell NextMove(Grid grid, Agent agent)
        {
            var exploredCells = agent.ExploredCells;

            var visited = new HashSet<GridCell>();
            var queue = new Queue<(GridCell Cell, GridCell? Parent)>();
            var parentMap = new Dictionary<GridCell, GridCell>();

            queue.Enqueue((agent.Position, null));
            visited.Add(agent.Position);

            while (queue.Count > 0)
            {
                var (current, parent) = queue.Dequeue();

                if (current == agent.Goal)
                {
                    // Trace the path back to the agent’s current position
                    while (parentMap.ContainsKey(current) && parentMap[current] != agent.Position)
                    {
                        current = parentMap[current];
                    }
                    return current;
                }

                foreach (var neighbor in GetNeighbors(grid, current, exploredCells))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue((neighbor, current));
                        parentMap[neighbor] = current;
                    }
                }
            }

            // If no path is found, return the agent’s current position
            return agent.Position;
        }

        private IEnumerable<GridCell> GetNeighbors(Grid grid, GridCell cell, HashSet<GridCell> exploredCells)
        {
            var neighbors = new List<GridCell>();
            var directions = new MovementDirection[]
            {
                MovementDirection.Up,
                MovementDirection.Down,
                MovementDirection.Left,
                MovementDirection.Right
            };

            foreach (var dir in directions)
            {
                int newRow = cell.Row + dir.RowOffset;
                int newCol = cell.Column + dir.ColOffset;

                if (newRow >= 0 && newRow < grid.Rows && newCol >= 0 && newCol < grid.Columns)
                {
                    var neighbor = grid.GetCell(newRow, newCol);
                    if (neighbor.Type != CellType.Obstacle && exploredCells.Contains(neighbor))
                        neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }
    }
}
