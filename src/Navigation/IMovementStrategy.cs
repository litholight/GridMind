// src/Navigation/IMovementStrategy.cs

using GridMind.Agents;
using GridMind.Environment;

namespace GridMind.Navigation
{
    public interface IMovementStrategy
    {
        GridCell NextMove(Grid grid, Agent agent);
    }
}