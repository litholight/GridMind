// src/Commands/MoveCommand.cs
using GridMind.Agents;
using GridMind.Environment;
using GridMind.Utilities;
using System.Windows.Input;

namespace GridMind.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly Agent agent;
        private readonly Grid grid;
        private readonly MovementDirection direction;
        private GridCell previousPosition;

        public MoveCommand(Agent agent, Grid grid, MovementDirection direction)
        {
            this.agent = agent;
            this.grid = grid;
            this.direction = direction;
        }

        public void Execute()
        {
            previousPosition = agent.Position;
            agent.Position = GetNextPosition();
        }

        public void Undo()
        {
            agent.Position = previousPosition;
        }

        private GridCell GetNextPosition()
        {
            int newRow = agent.Position.Row + direction.RowOffset;
            int newCol = agent.Position.Column + direction.ColOffset;

            if (newRow >= 0 && newRow < grid.Rows && newCol >= 0 && newCol < grid.Columns)
            {
                var nextCell = grid.GetCell(newRow, newCol);
                if (nextCell.Type != CellType.Obstacle)
                {
                    return nextCell;
                }
            }

            // Invalid move, stay in place
            return agent.Position;
        }
    }
}
