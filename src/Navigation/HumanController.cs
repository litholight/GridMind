// src/Navigation/HumanController.cs
using GridMind.Agents;
using GridMind.Commands;
using GridMind.Environment;
using GridMind.Utilities;
using System.Collections.Generic;

namespace GridMind.Navigation
{
    public class HumanController : IMovementStrategy
    {
        private MovementDirection currentDirection;
        private readonly Agent agent;
        private readonly Stack<ICommand> commandHistory;
        private readonly Grid grid;

        public HumanController(Agent agent, Grid grid)
        {
            this.agent = agent;
            currentDirection = MovementDirection.None;
            commandHistory = new Stack<ICommand>();
            this.grid = grid;
        }

        public void SetDirection(MovementDirection direction)
        {
            currentDirection = direction;
        }

        public GridCell NextMove(Grid grid, Agent agent)
        {
            if (currentDirection == MovementDirection.None)
            {
                // If no direction is set, stay in place
                return agent.Position;
            }

            var moveCommand = new MoveCommand(agent, grid, currentDirection);
            moveCommand.Execute();
            commandHistory.Push(moveCommand);

            // Reset currentDirection to prevent continuous movement
            currentDirection = MovementDirection.None;

            return agent.Position;
        }

        public void UndoLastMove()
        {
            if (commandHistory.Count > 0)
            {
                var lastCommand = commandHistory.Pop();
                lastCommand.Undo();
            }
        }
    }
}
