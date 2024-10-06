// src/Navigation/ExplorationStrategy.cs
using System.Collections.Generic;
using System.Linq;
using GridMind.Agents;
using GridMind.Environment;
using GridMind.Utilities;

namespace GridMind.Navigation
{
    public class ExplorationStrategy : IMovementStrategy
    {
        private readonly RandomWalkStrategy randomWalkStrategy;

        public ExplorationStrategy()
        {
            // Initialize the fallback random strategy
            randomWalkStrategy = new RandomWalkStrategy();
        }

        public GridCell NextMove(Grid grid, Agent agent)
        {
            // Get the list of valid neighbors around the agent’s current position
            var validNeighbors = MovementValidator.GetValidNeighbors(grid, agent.Position);

            // Filter to only include neighbors that have not been visited yet
            var unexploredNeighbors = validNeighbors
                .Where(neighbor => !agent.VisitedCells.Contains(neighbor))
                .ToList();

            if (unexploredNeighbors.Any())
            {
                // If there are unexplored neighbors, move to one of them
                return unexploredNeighbors.First();
            }

            // If all neighbors have been explored, switch to random walk strategy
            return randomWalkStrategy.NextMove(grid, agent);
        }
    }
}
