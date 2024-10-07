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

            // If no valid neighbors exist, fallback to the random strategy
            if (!validNeighbors.Any())
                return randomWalkStrategy.NextMove(grid, agent);

            // Calculate the score for each neighbor based on how many unexplored cells it can reveal
            var scoredMoves = new Dictionary<GridCell, int>();

            foreach (var neighbor in validNeighbors)
            {
                // Determine the number of newly visible cells from this neighbor position
                int newlyVisibleCells = CountNewlyVisibleCells(grid, neighbor, agent.ExploredCells);
                scoredMoves[neighbor] = newlyVisibleCells;
            }

            // Select the neighbor with the maximum number of newly revealed cells
            var bestMove = scoredMoves
                .OrderByDescending(pair => pair.Value)
                .ThenBy(_ => System.Guid.NewGuid()) // To randomize ties
                .FirstOrDefault()
                .Key;

            return bestMove ?? randomWalkStrategy.NextMove(grid, agent);
        }

        /// <summary>
        /// Counts the number of newly visible cells from a given position that are not in the explored set.
        /// </summary>
        private int CountNewlyVisibleCells(
            Grid grid,
            GridCell position,
            HashSet<GridCell> exploredCells
        )
        {
            int visibilityRadius = 1; // Adjust radius if needed for the agent's field of view
            int newCells = 0;

            for (int rowOffset = -visibilityRadius; rowOffset <= visibilityRadius; rowOffset++)
            {
                for (int colOffset = -visibilityRadius; colOffset <= visibilityRadius; colOffset++)
                {
                    int wrappedRow = grid.GetWrappedRow(position.Row + rowOffset);
                    int wrappedCol = grid.GetWrappedColumn(position.Column + colOffset);

                    var neighborCell = grid.GetWrappedCell(wrappedRow, wrappedCol);

                    if (
                        !exploredCells.Contains(neighborCell)
                        && neighborCell.Type != CellType.Obstacle
                    )
                    {
                        newCells++;
                    }
                }
            }

            return newCells;
        }
    }
}
