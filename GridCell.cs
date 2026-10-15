namespace TerminalGrid.NET
{
    /// <summary>
    /// Individual cell content representation with formatting capabilities.
    /// </summary>
    public sealed class GridCell
    {
        public string Text { get; set; }
        public TextAlignment Alignment { get; set; }
        public string AnsiStyleCode { get; set; }

        public GridCell(string text, TextAlignment alignment = TextAlignment.Left, string ansiStyleCode = null)
        {
            Text = text ?? "";
            Alignment = alignment;
            AnsiStyleCode = ansiStyleCode;
        }

        public override string ToString() => Text;
    }
}