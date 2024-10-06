// src/Navigation/BreadthFirstSearchStrategy.cs
using System.Collections.Generic;
using GridMind.Agents;
using GridMind.Environment;
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

                foreach (var neighbor in MovementValidator.GetValidNeighbors(grid, current))
                {
                    if (!visited.Contains(neighbor) && exploredCells.Contains(neighbor))
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
    }
}
