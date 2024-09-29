// src/Agents/Agent.cs
using GridMind.Environment;
using GridMind.Navigation;

namespace GridMind.Agents
{
    public class Agent
    {
        public string Name { get; }
        public GridCell Position { get; set; }
        public GridCell? Goal { get; set; }
        private IMovementStrategy movementStrategy;

        public Agent(string name, GridCell startPosition, GridCell? goal = null)
        {
            Name = name;
            Position = startPosition;
            Goal = goal;
            movementStrategy = new RandomWalkStrategy();  // Default strategy
        }

        public void SetMovementStrategy(IMovementStrategy strategy)
        {
            movementStrategy = strategy;
        }

        public void Move(Grid grid)
        {
            Position = movementStrategy.NextMove(grid, this);

            // Check if the agent has reached the goal
            if (Position == Goal)
            {
                System.Console.WriteLine($"{Name} has reached the goal at ({Position.Row}, {Position.Column})!");
            }
        }
    }
}