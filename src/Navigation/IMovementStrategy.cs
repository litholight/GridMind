// src/Navigation/IMovementStrategy.cs
using GridMind.Agents;
using GridMind.Environment;

namespace GridMind.Navigation
{
    public interface IMovementStrategy
    {
        /// <summary>
        /// Determines the next move for the agent based on its current position and strategy.
        /// </summary>
        GridCell NextMove(Grid grid, Agent agent);
    }
}
