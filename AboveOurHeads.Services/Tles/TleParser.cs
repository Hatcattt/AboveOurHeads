using AboveOurHeads.Core.Models;

namespace AboveOurHeads.Services.Tles
{
    public static class TleParser
    {
        public static TleData Parse(string content)
        {
            var lines = content
                .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length != 3)
                throw new FormatException($"TLE data must contain exactly 3 lines, found {lines.Length}.");

            return new TleData
            {
                Name = lines[0].Trim(),
                Line1 = lines[1].Trim(),
                Line2 = lines[2].Trim(),
            };
        }
    }
}
