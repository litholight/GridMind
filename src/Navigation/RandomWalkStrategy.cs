// src/Navigation/RandomWalkStrategy.cs
using System;
using GridMind.Environment;
using GridMind.Agents;
using GridMind.Utilities;

namespace GridMind.Navigation
{
    public class RandomWalkStrategy : IMovementStrategy
    {
        private static readonly MovementDirection[] Directions = 
        { 
            MovementDirection.Up, 
            MovementDirection.Down, 
            MovementDirection.Left, 
            MovementDirection.Right 
        };

        private readonly Random random = new Random();

        public GridCell NextMove(Grid grid, Agent agent)
        {
            MovementDirection randomDirection = Directions[random.Next(Directions.Length)];
            int newRow = agent.Position.Row + randomDirection.RowOffset;
            int newCol = agent.Position.Column + randomDirection.ColOffset;

            if (newRow < 0 || newRow >= grid.Rows || newCol < 0 || newCol >= grid.Columns)
                return agent.Position;  // Out of bounds, stay in place

            GridCell nextCell = grid.GetCell(newRow, newCol);
            return nextCell.Type == CellType.Obstacle ? agent.Position : nextCell;
        }
    }
}