// src/Program.cs
using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GridMind.Agents;
using GridMind.Configuration;
using GridMind.Environment;
using GridMind.Navigation;
using GridMind.UI;

namespace GridMind
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace().UseReactiveUI();
    }

    public class App : Application
    {
        private GridInitializer gridInitializer;
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
                // Initialize the GridInitializer with default values
                gridInitializer = new GridInitializer();

                // Use GridInitializer to set up the environment
                (grid, agent) = gridInitializer.InitializeGrid(
                    rows: 10,
                    columns: 10,
                    numberOfObstacles: 10,
                    strategy: new ExplorerSearchStrategy()
                );

                // Pass the initialized Grid and Agent to the VisualizerWindow
                desktop.MainWindow = new VisualizerWindow(
                    grid,
                    agent,
                    () =>
                    {
                        if (agent.Position == agent.Goal)
                        {
                            System.Console.WriteLine(
                                $"{agent.Name} has reached the goal at ({agent.Position.Row}, {agent.Position.Column})!"
                            );
                        }
                        else
                        {
                            agent.Move(grid);
                        }
                    }
                );
            }

            base.OnFrameworkInitializationCompleted();
        }

        public void ReinitializeGrid(int rows, int columns, int numberOfObstacles)
        {
            // Reinitialize the grid and agent with the given parameters
            (grid, agent) = gridInitializer.InitializeGrid(
                rows,
                columns,
                numberOfObstacles,
                strategy: new ExplorerSearchStrategy()
            );
        }
    }
}
