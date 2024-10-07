using System.Windows.Input;
using GridMind.Agents;
using GridMind.Environment;
using GridMind.Utilities;

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
            // Calculate new row and column using wrapping logic for toroidal movement
            int newRow = (agent.Position.Row + direction.RowOffset + grid.Rows) % grid.Rows;
            int newCol =
                (agent.Position.Column + direction.ColOffset + grid.Columns) % grid.Columns;

            // Get the cell at the wrapped position
            var nextCell = grid.GetCell(newRow, newCol);

            // Return the next cell only if it is not an obstacle
            if (nextCell.Type != CellType.Obstacle)
            {
                return nextCell;
            }

            // If the cell is an obstacle, stay in place
            return agent.Position;
        }
    }
}
