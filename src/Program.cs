// src/Program.cs
using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GridMind.Environment;
using GridMind.Agents;
using GridMind.Navigation;
using GridMind.UI;

namespace GridMind
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace()
                         .UseReactiveUI();
    }

    public class App : Application
    {
        private Grid? grid;
        private Agent? agent;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Create a larger grid size
                int gridRows = 15;
                int gridColumns = 15;
                grid = new Grid(gridRows, gridColumns);

                // Create a random number generator
                var random = new Random();

                // Set up random start and goal cells
                var startRow = random.Next(0, gridRows);
                var startCol = random.Next(0, gridColumns);
                var goalRow = random.Next(0, gridRows);
                var goalCol = random.Next(0, gridColumns);

                // Ensure start and goal are not the same
                while (startRow == goalRow && startCol == goalCol)
                {
                    goalRow = random.Next(0, gridRows);
                    goalCol = random.Next(0, gridColumns);
                }

                // Set the start and goal cells
                var startCell = grid.GetCell(startRow, startCol);
                var goalCell = grid.GetCell(goalRow, goalCol);
                startCell.Type = CellType.Start;
                goalCell.Type = CellType.Goal;

                // Randomly place obstacles
                int numberOfObstacles = 30;  // Adjust the number of obstacles as needed
                for (int i = 0; i < numberOfObstacles; i++)
                {
                    int obstacleRow, obstacleCol;
                    do
                    {
                        obstacleRow = random.Next(0, gridRows);
                        obstacleCol = random.Next(0, gridColumns);
                    }
                    // Ensure obstacles don't overlap with start or goal cells
                    while ((obstacleRow == startRow && obstacleCol == startCol) ||
                           (obstacleRow == goalRow && obstacleCol == goalCol) ||
                           grid.GetCell(obstacleRow, obstacleCol).Type == CellType.Obstacle);

                    // Place obstacle
                    grid.GetCell(obstacleRow, obstacleCol).Type = CellType.Obstacle;
                }

                // Create the agent and assign a strategy
                agent = new Agent("Explorer", startCell, goalCell);
                agent.SetMovementStrategy(new RandomWalkStrategy());

                // Pass the initialized Grid and Agent to the VisualizerWindow
                desktop.MainWindow = new VisualizerWindow(grid, agent, () =>
                {
                    if (agent.Position == goalCell)
                    {
                        System.Console.WriteLine($"{agent.Name} has reached the goal at ({goalCell.Row}, {goalCell.Column})!");
                    }
                    else
                    {
                        agent.Move(grid);
                    }
                });
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
