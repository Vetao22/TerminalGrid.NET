namespace TerminalGrid.NET
{
    public enum ConsoleColor16
    {
        Reset = 0,
        Black = 30, Red = 31, Green = 32, Yellow = 33,
        Blue = 34, Magenta = 35, Cyan = 36, White = 37,
        BrightBlack = 90, BrightRed = 91, BrightGreen = 92, BrightYellow = 93,
        BrightBlue = 94, BrightMagenta = 95, BrightCyan = 96, BrightWhite = 97
    }

    /// <summary>
    /// Utility class for creating ANSI escape codes for terminal colors and text formatting.
    /// </summary>
    public static class AnsiStyle
    {
        public const string Reset = "\u001b[0m";
        public const string Bold = "\u001b[1m";
        public const string Dim = "\u001b[2m";
        public const string Italic = "\u001b[3m";
        public const string Underline = "\u001b[4m";

        public static string Foreground(ConsoleColor16 color) => $"\u001b[{(int)color}m";
        public static string Background(ConsoleColor16 color) => $"\u001b[{(int)color + 10}m";

        public static string ForegroundRgb(byte r, byte g, byte b) => $"\u001b[38;2;{r};{g};{b}m";
        public static string BackgroundRgb(byte r, byte g, byte b) => $"\u001b[48;2;{r};{g};{b}m";

        public static string Colorize(string text, ConsoleColor16 color, bool isBold = false)
        {
            if (string.IsNullOrEmpty(text)) return "";
            string style = isBold ? $"{Bold}{Foreground(color)}" : Foreground(color);
            return $"{style}{text}{Reset}";
        }
    }
}