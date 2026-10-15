using System;
using System.Text;
using System.Threading;

namespace TerminalGrid.NET
{
    /// <summary>
    /// Rich CLI widgets: Panels, Progress bars, and Spinners.
    /// </summary>
    public static class TerminalWidgets
    {
        public static string RenderPanel(string title, string content, BorderPreset preset = BorderPreset.Rounded, int width = 50)
        {
            var border = BorderStyle.GetPreset(preset);
            var sb = new StringBuilder();

            string titleStr = string.IsNullOrEmpty(title) ? "" : $" {title} ";
            int borderWidth = width - 2 - titleStr.Length;
            int leftPad = borderWidth / 2;
            int rightPad = borderWidth - leftPad;

            sb.Append(border.TopLeft);
            sb.Append(new string(border.Horizontal, leftPad));
            sb.Append(AnsiStyle.Colorize(titleStr, ConsoleColor16.BrightYellow, isBold: true));
            sb.Append(new string(border.Horizontal, rightPad));
            sb.Append(border.TopRight);
            sb.AppendLine();

            sb.Append(border.Vertical);
            sb.Append(' ');
            sb.Append(content.PadRight(width - 4));
            sb.Append(' ');
            sb.Append(border.Vertical);
            sb.AppendLine();

            sb.Append(border.BottomLeft);
            sb.Append(new string(border.Horizontal, width - 2));
            sb.Append(border.BottomRight);

            return sb.ToString();
        }

        public static string RenderProgressBar(double percentage, int width = 20)
        {
            percentage = Math.Clamp(percentage, 0.0, 1.0);
            int filled = (int)Math.Round(percentage * width);
            int empty = width - filled;

            string fillBar = new string('█', filled);
            string emptyBar = new string('░', empty);

            string color = percentage >= 1.0 
                ? AnsiStyle.Foreground(ConsoleColor16.BrightGreen) 
                : AnsiStyle.Foreground(ConsoleColor16.BrightCyan);

            return $"{color}[{fillBar}{emptyBar}]{AnsiStyle.Reset} {percentage * 100:F0}%";
        }

        public static void RunSpinner(string label, Action action)
        {
            char[] frames = new[] { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
            bool done = false;

            Thread thread = new Thread(() =>
            {
                int i = 0;
                while (!done)
                {
                    Console.Write($"\r{AnsiStyle.Foreground(ConsoleColor16.BrightMagenta)}{frames[i]}{AnsiStyle.Reset} {label}");
                    i = (i + 1) % frames.Length;
                    Thread.Sleep(80);
                }
            });

            thread.Start();
            action();
            done = true;
            thread.Join();

            Console.WriteLine($"\r{AnsiStyle.Foreground(ConsoleColor16.BrightGreen)}✔{AnsiStyle.Reset} {label} Finished!");
        }
    }
}