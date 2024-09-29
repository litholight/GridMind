﻿// src/Program.cs
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
        // Entry point of the program
        static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Configure the Avalonia App
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace()
                         .UseReactiveUI();
    }

    // Avalonia Application Class
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
                // Set up the Grid and Agent
                grid = new Grid(5, 5);
                var startCell = grid.GetCell(0, 0);
                var goalCell = grid.GetCell(4, 4);
                startCell.Type = CellType.Start;
                goalCell.Type = CellType.Goal;

                grid.GetCell(2, 2).Type = CellType.Obstacle;
                grid.GetCell(3, 2).Type = CellType.Obstacle;

                // Create the agent and assign a strategy
                agent = new Agent("Explorer", startCell, goalCell);
                agent.SetMovementStrategy(new BreadthFirstSearchStrategy());

                // Pass the initialized Grid and Agent to the VisualizerWindow
                desktop.MainWindow = new VisualizerWindow(grid, agent, () => 
                {
                    if (agent.Position == goalCell)
                    {
                        System.Console.WriteLine($"{agent.Name} has reached the goal!");
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
