using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalGrid.NET
{
    /// <summary>
    /// Core table rendering engine supporting headers, word wrapping, alignment, and custom borders.
    /// </summary>
    public sealed class GridTable
    {
        private readonly List<string> _headers;
        private readonly List<List<GridCell>> _rows;
        private readonly Dictionary<int, TextAlignment> _columnAlignments;
        private readonly Dictionary<int, int> _columnWidthLimits;

        public BorderStyle Border { get; set; }
        public int Padding { get; set; }
        public string HeaderStyleCode { get; set; }
        public string BorderStyleCode { get; set; }

        public GridTable(BorderPreset borderPreset = BorderPreset.Square)
        {
            _headers = new List<string>();
            _rows = new List<List<GridCell>>();
            _columnAlignments = new Dictionary<int, TextAlignment>();
            _columnWidthLimits = new Dictionary<int, int>();

            Border = BorderStyle.GetPreset(borderPreset);
            Padding = 1;
            HeaderStyleCode = AnsiStyle.Bold + AnsiStyle.Foreground(ConsoleColor16.BrightCyan);
            BorderStyleCode = AnsiStyle.Foreground(ConsoleColor16.BrightBlack);
        }

        public GridTable AddHeaders(params string[] headers)
        {
            _headers.AddRange(headers);
            return this;
        }

        public GridTable AddRow(params object[] values)
        {
            var row = new List<GridCell>();
            for (int i = 0; i < values.Length; i++)
            {
                string text = values[i]?.ToString() ?? "";
                TextAlignment align = _columnAlignments.TryGetValue(i, out var a) ? a : TextAlignment.Left;
                row.Add(new GridCell(text, align));
            }
            _rows.Add(row);
            return this;
        }

        public GridTable SetColumnAlignment(int columnIndex, TextAlignment alignment)
        {
            _columnAlignments[columnIndex] = alignment;
            return this;
        }

        public GridTable SetColumnMaxWidth(int columnIndex, int maxWidth)
        {
            _columnWidthLimits[columnIndex] = maxWidth;
            return this;
        }

        public string Render()
        {
            int colCount = Math.Max(_headers.Count, GetMaxRowColumnCount());
            if (colCount == 0) return string.Empty;

            int[] colWidths = CalculateColumnWidths(colCount);
            var sb = new StringBuilder();

            RenderHorizontalBorder(sb, colWidths, Border.TopLeft, Border.TeeDown, Border.TopRight);

            if (_headers.Count > 0)
            {
                RenderRow(sb, _headers.ConvertAll(h => new GridCell(h, TextAlignment.Left, HeaderStyleCode)), colWidths);
                RenderHorizontalBorder(sb, colWidths, Border.TeeRight, Border.Cross, Border.TeeLeft);
            }

            for (int r = 0; r < _rows.Count; r++)
            {
                RenderRow(sb, _rows[r], colWidths);
            }

            RenderHorizontalBorder(sb, colWidths, Border.BottomLeft, Border.TeeUp, Border.BottomRight);

            return sb.ToString();
        }

        private void RenderRow(StringBuilder sb, List<GridCell> cells, int[] colWidths)
        {
            List<string[]> wrappedCells = new List<string[]>();
            int maxLines = 1;

            for (int i = 0; i < colWidths.Length; i++)
            {
                string text = i < cells.Count ? cells[i].Text : "";
                int width = colWidths[i];
                var lines = WrapText(text, width);
                wrappedCells.Add(lines);
                if (lines.Length > maxLines) maxLines = lines.Length;
            }

            for (int line = 0; line < maxLines; line++)
            {
                sb.Append(ApplyStyle(Border.Vertical.ToString(), BorderStyleCode));

                for (int i = 0; i < colWidths.Length; i++)
                {
                    string textLine = line < wrappedCells[i].Length ? wrappedCells[i][line] : "";
                    TextAlignment align = i < cells.Count ? cells[i].Alignment : TextAlignment.Left;
                    string style = i < cells.Count ? cells[i].AnsiStyleCode : null;

                    string paddedText = AlignText(textLine, colWidths[i], align);
                    string paddingSpace = new string(' ', Padding);

                    sb.Append(paddingSpace);
                    sb.Append(ApplyStyle(paddedText, style));
                    sb.Append(paddingSpace);

                    sb.Append(ApplyStyle(Border.Vertical.ToString(), BorderStyleCode));
                }
                sb.AppendLine();
            }
        }

        private void RenderHorizontalBorder(StringBuilder sb, int[] colWidths, char left, char mid, char right)
        {
            sb.Append(ApplyStyle(left.ToString(), BorderStyleCode));
            for (int i = 0; i < colWidths.Length; i++)
            {
                int totalWidth = colWidths[i] + (Padding * 2);
                sb.Append(ApplyStyle(new string(Border.Horizontal, totalWidth), BorderStyleCode));
                if (i < colWidths.Length - 1)
                    sb.Append(ApplyStyle(mid.ToString(), BorderStyleCode));
            }
            sb.Append(ApplyStyle(right.ToString(), BorderStyleCode));
            sb.AppendLine();
        }

        private int[] CalculateColumnWidths(int colCount)
        {
            int[] widths = new int[colCount];

            for (int i = 0; i < _headers.Count; i++)
                widths[i] = _headers[i].Length;

            foreach (var row in _rows)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    int len = row[i].Text.Length;
                    if (len > widths[i]) widths[i] = len;
                }
            }

            for (int i = 0; i < colCount; i++)
            {
                if (_columnWidthLimits.TryGetValue(i, out int max) && widths[i] > max)
                    widths[i] = max;
            }

            return widths;
        }

        private static string AlignText(string text, int width, TextAlignment alignment)
        {
            if (text.Length >= width) return text.Substring(0, width);

            int spaces = width - text.Length;
            return alignment switch
            {
                TextAlignment.Right => new string(' ', spaces) + text,
                TextAlignment.Center => new string(' ', spaces / 2) + text + new string(' ', spaces - (spaces / 2)),
                _ => text + new string(' ', spaces)
            };
        }

        private static string[] WrapText(string text, int maxWidth)
        {
            if (string.IsNullOrEmpty(text)) return new[] { "" };
            if (text.Length <= maxWidth) return new[] { text };

            List<string> lines = new List<string>();
            while (text.Length > 0)
            {
                int length = Math.Min(maxWidth, text.Length);
                lines.Add(text.Substring(0, length));
                text = text.Substring(length);
            }
            return lines.ToArray();
        }

        private int GetMaxRowColumnCount()
        {
            int max = 0;
            foreach (var row in _rows)
                if (row.Count > max) max = row.Count;
            return max;
        }

        private static string ApplyStyle(string text, string ansiStyle)
        {
            if (string.IsNullOrEmpty(ansiStyle) || string.IsNullOrEmpty(text)) return text;
            return $"{ansiStyle}{text}{AnsiStyle.Reset}";
        }
    }
}