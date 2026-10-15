namespace TerminalGrid.NET
{
    public enum BorderPreset
    {
        Square,
        Rounded,
        Double,
        Heavy,
        Ascii,
        Minimal,
        None
    }

    /// <summary>
    /// Character mapping definition for table drawing borders.
    /// </summary>
    public sealed class BorderStyle
    {
        public char TopLeft { get; }
        public char TopRight { get; }
        public char BottomLeft { get; }
        public char BottomRight { get; }
        public char Horizontal { get; }
        public char Vertical { get; }
        public char Cross { get; }
        public char TeeDown { get; }
        public char TeeUp { get; }
        public char TeeRight { get; }
        public char TeeLeft { get; }

        public BorderStyle(
            char topLeft, char topRight, char bottomLeft, char bottomRight,
            char horizontal, char vertical, char cross,
            char teeDown, char teeUp, char teeRight, char teeLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Horizontal = horizontal;
            Vertical = vertical;
            Cross = cross;
            TeeDown = teeDown;
            TeeUp = teeUp;
            TeeRight = teeRight;
            TeeLeft = teeLeft;
        }

        public static BorderStyle GetPreset(BorderPreset preset)
        {
            return preset switch
            {
                BorderPreset.Square => new BorderStyle('┌', '┐', '└', '┘', '─', '│', '┼', '┬', '┴', '├', '┤'),
                BorderPreset.Rounded => new BorderStyle('╭', '╮', '╰', '╯', '─', '│', '┼', '┬', '┴', '├', '┤'),
                BorderPreset.Double => new BorderStyle('╔', '╗', '╚', '╝', '═', '║', '╬', '╦', '╩', '╠', '╣'),
                BorderPreset.Heavy => new BorderStyle('┏', '┓', '┗', '┛', '━', '┃', '╋', '┳', '┻', '┣', '┫'),
                BorderPreset.Ascii => new BorderStyle('+', '+', '+', '+', '-', '|', '+', '+', '+', '+', '+'),
                BorderPreset.Minimal => new BorderStyle(' ', ' ', ' ', ' ', '─', ' ', '─', '─', '─', ' ', ' '),
                BorderPreset.None => new BorderStyle(' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '),
                _ => GetPreset(BorderPreset.Square)
            };
        }
    }
}