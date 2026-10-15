using System;
using System.Text;
using System.Threading;
using TerminalGrid.NET;

namespace TerminalGridDemo
{
    /// <summary>
    /// Classic DOOM Fire effect implemented using TerminalGrid.NET rendering engine.
    /// Adaptation of Fabien Sanglard's algorithm for terminal output.
    /// </summary>
    public static class DoomFireDemo
    {
        // Fire matrix resolution dimensions
        private const int FireWidth = 60;
        private const int FireHeight = 22;

        // Palette of 37 colors ranging from black, through red, orange, yellow to white intensity
        private static readonly (byte R, byte G, byte B)[] FirePalette = new[]
        {
            (7,7,7),     (7,7,7),     (23,7,7),    (39,15,7),   (55,23,7),
            (71,31,7),   (87,39,7),   (103,47,7),  (119,55,7),  (135,63,7),
            (150,71,7),  (166,79,7),  (182,95,7),  (198,111,7), (203,127,15),
            (203,143,23), (203,159,31), (203,175,47), (203,191,55), (255,207,63),
            (255,223,71), (255,239,79), (255,255,111), (255,255,159), (255,255,207),
            (255,255,255)
        };

        // Density characters mapped to fire intensity ranges
        private static readonly char[] FireChars = { ' ', '.', ':', '-', '=', '+', '*', '#', '%', '@', '█' };

        public static void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            int[] firePixels = new int[FireWidth * FireHeight];
            Random rand = new Random();

            // Initialize fire bottom row with maximum heat (white source)
            int bottomRowStart = (FireHeight - 1) * FireWidth;
            for (int i = 0; i < FireWidth; i++)
            {
                firePixels[bottomRowStart + i] = FirePalette.Length - 1;
            }

            bool running = true;
            // Handle graceful exit on Ctrl+C or key press
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.ReadKey(true);
                running = false;
            });

            // Clear screen once at start
            Console.Clear();

            while (running)
            {
                // Step 1: Spread fire upwards with decay and wind
                SpreadFire(firePixels, rand);

                // Step 2: Render grid via TerminalGrid.NET
                string renderedFrame = RenderFireToGrid(firePixels);

                // Print frame at top-left origin to prevent flickering
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(TerminalWidgets.RenderPanel("DOOM FIRE - TerminalGrid.NET", "Press any key to exit...", BorderPreset.Rounded, FireWidth + 4));
                Console.WriteLine(renderedFrame);

                Thread.Sleep(35); // Control frame rate (~30 FPS)
            }

            Console.CursorVisible = true;
            Console.Clear();
        }

        /// <summary>
        /// Core DOOM fire propagation algorithm.
        /// </summary>
        private static void SpreadFire(int[] firePixels, Random rand)
        {
            for (int x = 0; x < FireWidth; x++)
            {
                for (int y = 1; y < FireHeight; y++)
                {
                    int srcIndex = y * FireWidth + x;
                    int randOffset = rand.Next(3); // Random decay factor
                    int destIndex = srcIndex - FireWidth - randOffset + 1;

                    if (destIndex >= 0 && destIndex < firePixels.Length)
                    {
                        int sourcePixel = firePixels[srcIndex];
                        int decay = randOffset & 1;
                        firePixels[destIndex] = Math.Max(0, sourcePixel - decay);
                    }
                }
            }
        }

        /// <summary>
        /// Converts pixel intensity array into a styled ANSI truecolor string block.
        /// </summary>
        private static string RenderFireToGrid(int[] firePixels)
        {
            var sb = new StringBuilder();

            for (int y = 0; y < FireHeight - 1; y++) // Skip bottom base row from direct view if wanted, or render all
            {
                sb.Append("  "); // Left margin
                for (int x = 0; x < FireWidth; x++)
                {
                    int intensity = firePixels[y * FireWidth + x];
                    int colorIdx = (intensity * (FirePalette.Length - 1)) / (FirePalette.Length - 1);
                    var color = FirePalette[colorIdx];

                    // Map intensity to density character
                    int charIdx = (intensity * (FireChars.Length - 1)) / (FirePalette.Length - 1);
                    char symbol = FireChars[charIdx];

                    // Apply TrueColor ANSI escape code using our library architecture
                    string ansiPixel = AnsiStyle.ColorizeRgb(new string(symbol, 2), color.R, color.G, color.B);
                    sb.Append(ansiPixel);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
