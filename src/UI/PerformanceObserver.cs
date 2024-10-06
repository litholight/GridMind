// src/UI/PerformanceObserver.cs
using System;
using Avalonia.Controls;
using GridMind.Utilities;

namespace GridMind.UI
{
    public class PerformanceObserver : IObserver<PerformanceMetrics>
    {
        private readonly TextBlock performanceLabel;

        public PerformanceObserver(TextBlock performanceLabel)
        {
            this.performanceLabel = performanceLabel;
        }

        public void OnNext(PerformanceMetrics metrics)
        {
            performanceLabel.Text =
                $"Performance: Steps = {metrics.StepsTaken}, "
                + $"Explored = {metrics.CellsExplored}, "
                + $"Repeated = {metrics.RepeatedVisits}, "
                + $"Goal Reached = {metrics.GoalReached}";
        }

        public void OnCompleted() { }

        public void OnError(Exception error) { }
    }
}
