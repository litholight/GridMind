// src/Agents/Agent.cs
using System.Collections.Generic;
using System.ComponentModel;
using GridMind.Environment;
using GridMind.Navigation;
using GridMind.Utilities;

namespace GridMind.Agents
{
    public class Agent : INotifyPropertyChanged
    {
        public string Name { get; }
        public GridCell? Goal { get; set; }
        public FogOfWar FogOfWar { get; }
        private IMovementStrategy movementStrategy;

        public HashSet<GridCell> VisitedCells { get; }
        public HashSet<GridCell> ExploredCells { get; }
        public PerformanceTracker PerformanceTracker { get; }

        private GridCell position;
        public GridCell Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public Agent(string name, GridCell startPosition, GridCell? goal = null)
        {
            Name = name;
            Position = startPosition;
            Goal = goal;
            movementStrategy = new RandomWalkStrategy();

            VisitedCells = new HashSet<GridCell> { startPosition };
            ExploredCells = new HashSet<GridCell> { startPosition };

            FogOfWar = new FogOfWar();
            FogOfWar.CellExplored(startPosition);

            // Initialize PerformanceTracker
            PerformanceTracker = new PerformanceTracker(name);
        }

        public void SetMovementStrategy(IMovementStrategy strategy)
        {
            movementStrategy = strategy;
        }

        public void Move(Grid grid)
        {
            // Store the previous position to track if the agent has revisited a cell
            var previousPosition = Position;

            // Move the agent based on the movement strategy
            Position = movementStrategy.NextMove(grid, this);

            // Track whether this cell has been visited before
            bool isPreviouslyVisited = VisitedCells.Contains(Position);

            // Track steps and whether a cell is a repeated visit or a new one
            PerformanceTracker.UpdateMetrics(metrics =>
            {
                metrics.IncrementSteps();

                // Add the cell to visited cells and update metrics accordingly
                if (!isPreviouslyVisited) // If the cell is newly visited
                {
                    VisitedCells.Add(Position); // Mark the cell as visited
                }
                else if (Position != previousPosition) // If it's a repeated visit and not staying in place
                {
                    metrics.IncrementRepeatedVisits(); // Track repeated visits correctly
                }
            });

            // Get visible cells and mark them as explored
            var visibleCells = GetVisibleCells(grid);
            foreach (var cell in visibleCells)
            {
                // Mark the cell as explored only if it is newly discovered
                if (ExploredCells.Add(cell))
                {
                    FogOfWar.CellExplored(cell); // Notify observers
                    PerformanceTracker.UpdateMetrics(metrics => metrics.IncrementCellsExplored());
                }
            }

            if (Position == Goal)
            {
                System.Console.WriteLine(
                    $"{Name} has reached the goal at ({Position.Row}, {Position.Column})!"
                );
                PerformanceTracker.UpdateMetrics(metrics => metrics.GoalReached = true);
            }
        }

        public List<GridCell> GetVisibleCells(Grid grid, int visibilityRadius = 1)
        {
            var visibleCells = new List<GridCell>();

            for (int rowOffset = -visibilityRadius; rowOffset <= visibilityRadius; rowOffset++)
            {
                for (int colOffset = -visibilityRadius; colOffset <= visibilityRadius; colOffset++)
                {
                    // Calculate the wrapped row and column indices
                    int wrappedRow = grid.GetWrappedRow(Position.Row + rowOffset);
                    int wrappedCol = grid.GetWrappedColumn(Position.Column + colOffset);

                    // Retrieve the wrapped cell
                    var cell = grid.GetCell(wrappedRow, wrappedCol);
                    visibleCells.Add(cell);
                }
            }

            return visibleCells;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
