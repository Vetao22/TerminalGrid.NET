using System;
using System.Threading;
using TerminalGrid.NET;

namespace TerminalGridDemo
{
    class Program
    {
        static void Main()
        {
            Console.Clear();

            // 1. Render a styled CLI panel
            Console.WriteLine(TerminalWidgets.RenderPanel("TerminalGrid.NET Dashboard", "System Status: SECURE & ACTIVE", BorderPreset.Rounded, 65));
            Console.WriteLine();

            // 2. Create and render an advanced table with colors and progress bars
            var table = new GridTable(BorderPreset.Rounded)
                .AddHeaders("ID", "Service", "Status", "Latency", "Health")
                .SetColumnAlignment(0, TextAlignment.Center)
                .SetColumnAlignment(3, TextAlignment.Right)
                .SetColumnAlignment(4, TextAlignment.Center);

            table.AddRow(1, "API Gateway", AnsiStyle.Colorize("ONLINE", ConsoleColor16.BrightGreen), "14ms", TerminalWidgets.RenderProgressBar(1.0, 12));
            table.AddRow(2, "Auth Service", AnsiStyle.Colorize("WARNING", ConsoleColor16.BrightYellow), "120ms", TerminalWidgets.RenderProgressBar(0.75, 12));
            table.AddRow(3, "Billing Worker", AnsiStyle.Colorize("OFFLINE", ConsoleColor16.BrightRed), "0ms", TerminalWidgets.RenderProgressBar(0.0, 12));

            Console.WriteLine(table.Render());

            // 3. Run a simulated terminal spinner task
            TerminalWidgets.RunSpinner("Syncing cluster nodes...", () =>
            {
                Thread.Sleep(1500); // Simulate background workload
            });

            Console.WriteLine();
        }
    }
}