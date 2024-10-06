// src/Navigation/ExplorerSearchStrategy.cs
using System.Collections.Generic;
using GridMind.Agents;
using GridMind.Environment;

namespace GridMind.Navigation
{
    public class ExplorerSearchStrategy : IMovementStrategy
    {
        private BreadthFirstSearchStrategy bfsStrategy;
        private ExplorationStrategy explorationStrategy;

        public ExplorerSearchStrategy()
        {
            bfsStrategy = new BreadthFirstSearchStrategy();
            explorationStrategy = new ExplorationStrategy();
        }

        public GridCell NextMove(Grid grid, Agent agent)
        {
            // If the goal is within visible cells, use BFS strategy
            if (agent.GetVisibleCells(grid).Contains(agent.Goal))
            {
                return bfsStrategy.NextMove(grid, agent);
            }
            else
            {
                // Otherwise, explore
                return explorationStrategy.NextMove(grid, agent);
            }
        }
    }
}
