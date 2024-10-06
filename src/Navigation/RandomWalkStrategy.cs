// src/Navigation/RandomWalkStrategy.cs
using System;
using System.Collections.Generic;
using System.Linq;
using GridMind.Agents;
using GridMind.Environment;
using GridMind.Utilities;

namespace GridMind.Navigation
{
    public class RandomWalkStrategy : IMovementStrategy
    {
        private readonly Random random = new Random();

        public GridCell NextMove(Grid grid, Agent agent)
        {
            // Get the list of navigable cells around the agent's current position using MovementValidator
            var validNeighbors = MovementValidator.GetValidNeighbors(grid, agent.Position);

            // If there are no valid neighbors, stay in place
            if (!validNeighbors.Any())
                return agent.Position;

            // Randomly select one of the valid neighboring cells as the next move
            return validNeighbors[random.Next(validNeighbors.Count)];
        }
    }
}
