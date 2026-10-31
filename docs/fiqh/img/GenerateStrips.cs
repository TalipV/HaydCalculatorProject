// Generates the day-strip images and icons of docs/fiqh/zero-to-hero.md.
// Run from the repository root: dotnet run docs/fiqh/img/GenerateStrips.cs
//
// In the guide each strip is an HTML comment followed by its image line:
//   <!-- strip: Blood: 3R 12C 3R | Ruling: 3H 12T 3F -->
//   ![Blood: 3 red, 12 clean, 3 red. Ruling: ...](img/strips/strip-1a2b3c4d.svg)
// Rows are separated by "|". Each row is "Label: <count><code> ..."; "..." at the end means the
// course goes on. Codes: K black, R red, A ashqar, Y yellow, W brownish, C clean (blood rows);
// H hayd, T tuhr, F blood that is not hayd (ruling rows).
// The tool rewrites the image lines, writes the SVG files and deletes strip files no longer used.
// Edit only the comments; never edit an image line or a generated SVG by hand.

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

string guidePath = args.Length > 0 ? args[0] : Path.Combine("docs", "fiqh", "zero-to-hero.md");
string imageDirectory = Path.Combine(Path.GetDirectoryName(guidePath)!, "img");
string stripDirectory = Path.Combine(imageDirectory, "strips");
string iconDirectory = Path.Combine(imageDirectory, "icons");
Directory.CreateDirectory(stripDirectory);
Directory.CreateDirectory(iconDirectory);

foreach ((string name, char code) in StripRenderer.Icons)
{
    StripRenderer.WriteSvg(Path.Combine(iconDirectory, name + ".svg"), StripRenderer.RenderIcon(code));
}

string guide = File.ReadAllText(guidePath);
string newLine = guide.Contains("\r\n") ? "\r\n" : "\n";
string[] lines = guide.Split(newLine);
List<string> output = [];
HashSet<string> usedStrips = [];

for (int i = 0; i < lines.Length; i++)
{
    output.Add(lines[i]);
    Match match = StripRenderer.StripComment().Match(lines[i]);
    if (!match.Success)
    {
        continue;
    }

    List<StripRow> rows = StripRenderer.Parse(match.Groups[1].Value);
    string fileName = "strip-" + Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(match.Groups[1].Value)))[..8] + ".svg";
    if (usedStrips.Add(fileName))
    {
        StripRenderer.WriteSvg(Path.Combine(stripDirectory, fileName), StripRenderer.RenderStrip(rows));
    }

    if (i + 1 < lines.Length && lines[i + 1].StartsWith("![", StringComparison.Ordinal))
    {
        i++;
    }

    output.Add($"![{StripRenderer.Describe(rows)}](img/strips/{fileName})");
}

foreach (string file in Directory.GetFiles(stripDirectory, "strip-*.svg"))
{
    if (!usedStrips.Contains(Path.GetFileName(file)))
    {
        File.Delete(file);
    }
}

File.WriteAllText(guidePath, string.Join(newLine, output));
Console.WriteLine($"{usedStrips.Count} strips written.");

internal sealed record StripRow(string Label, List<char> Days, bool GoesOn);

internal static partial class StripRenderer
{
    private const int CellSize = 22;
    private const int GroupGap = 8;
    private const int RowHeight = 28;
    private const int RulerHeight = 18;
    private const string Grey = "#8b949e";

    // Colours chosen to read on light and on dark backgrounds (GitHub, Visual Studio).
    private static readonly Dictionary<char, (string Name, string Svg)> s_symbols = new()
    {
        ['K'] = ("black", Drop("#000000", "#9aa4ae")),
        ['R'] = ("red", Drop("#f0508a", null)),
        ['A'] = ("ashqar", Drop("#f08a24", null)),
        ['Y'] = ("yellow", Drop("#ffd21f", "#c9a200")),
        ['W'] = ("brownish", Drop("#8d5a3b", null)),
        ['C'] = ("clean", Clean()),
        ['T'] = ("tuhr", Clean()),
        ['H'] = ("hayd", $"<circle cx=\"11\" cy=\"11\" r=\"8\" fill=\"#c62828\"/>"),
        ['F'] = ("not hayd", $"<g stroke=\"{Grey}\" stroke-width=\"3.6\" stroke-linecap=\"round\"><line x1=\"5\" y1=\"5\" x2=\"17\" y2=\"17\"/><line x1=\"17\" y1=\"5\" x2=\"5\" y2=\"17\"/></g>"),
    };

    public static IEnumerable<(string Name, char Code)> Icons =>
    [
        ("black", 'K'), ("red", 'R'), ("ashqar", 'A'), ("yellow", 'Y'), ("brownish", 'W'),
        ("clean", 'C'), ("hayd", 'H'), ("not-hayd", 'F'),
    ];

    [GeneratedRegex(@"^<!-- strip: (.+) -->$")]
    public static partial Regex StripComment();

    public static List<StripRow> Parse(string spec)
    {
        List<StripRow> rows = [];
        foreach (string part in spec.Split('|'))
        {
            int colon = part.IndexOf(':');
            List<char> days = [];
            bool goesOn = false;
            foreach (string token in part[(colon + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (token == "...")
                {
                    goesOn = true;
                    continue;
                }

                char code = token[^1];
                if (!s_symbols.ContainsKey(code) || !int.TryParse(token[..^1], out int count))
                {
                    throw new FormatException($"Unknown token '{token}' in strip '{spec}'.");
                }

                days.AddRange(Enumerable.Repeat(code, count));
            }

            rows.Add(new StripRow(part[..colon].Trim(), days, goesOn));
        }

        if (rows.Select(row => row.Days.Count).Distinct().Count() != 1)
        {
            throw new FormatException($"The rows of strip '{spec}' have different lengths.");
        }

        return rows;
    }

    public static string Describe(List<StripRow> rows)
    {
        return string.Join(" ", rows.Select(row =>
        {
            List<string> blocks = [];
            for (int i = 0; i < row.Days.Count;)
            {
                int start = i;
                while (i < row.Days.Count && row.Days[i] == row.Days[start])
                {
                    i++;
                }

                blocks.Add($"{i - start} {s_symbols[row.Days[start]].Name}");
            }

            return $"{row.Label}: {string.Join(", ", blocks)}{(row.GoesOn ? ", and on" : "")}.";
        }));
    }

    public static string RenderIcon(char code)
    {
        return $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{CellSize}\" height=\"{CellSize}\" viewBox=\"0 0 {CellSize} {CellSize}\">\n{s_symbols[code].Svg}\n</svg>\n";
    }

    public static string RenderStrip(List<StripRow> rows)
    {
        int dayCount = rows[0].Days.Count;
        int labelWidth = (rows.Max(row => row.Label.Length) * 7) + 16;
        int width = DayX(labelWidth, dayCount) + (rows.Any(row => row.GoesOn) ? 24 : 4);
        int height = RulerHeight + (rows.Count * RowHeight);

        StringBuilder svg = new();
        svg.Append($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {width} {height}\" font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" fill=\"{Grey}\">\n");
        for (int day = 0; day < dayCount; day++)
        {
            if (day == 0 || (day + 1) % 5 == 0)
            {
                svg.Append($"<text x=\"{DayX(labelWidth, day) + 11}\" y=\"11\" font-size=\"10\" text-anchor=\"middle\">{day + 1}</text>\n");
            }
        }

        for (int r = 0; r < rows.Count; r++)
        {
            int y = RulerHeight + (r * RowHeight);
            svg.Append($"<text x=\"0\" y=\"{y + 15}\" font-size=\"13\">{rows[r].Label}</text>\n");
            for (int day = 0; day < dayCount; day++)
            {
                svg.Append($"<g transform=\"translate({DayX(labelWidth, day)},{y})\">{s_symbols[rows[r].Days[day]].Svg}</g>\n");
            }

            if (rows[r].GoesOn)
            {
                svg.Append($"<text x=\"{DayX(labelWidth, dayCount) + 4}\" y=\"{y + 15}\" font-size=\"14\">…</text>\n");
            }
        }

        svg.Append("</svg>\n");
        return svg.ToString();
    }

    public static void WriteSvg(string path, string svg)
    {
        // The working tree uses CRLF (.gitattributes).
        File.WriteAllText(path, svg.ReplaceLineEndings("\r\n"));
    }

    private static int DayX(int labelWidth, int day)
    {
        return labelWidth + (day * CellSize) + (day / 5 * GroupGap);
    }

    private static string Drop(string fill, string? stroke)
    {
        string outline = stroke is null ? "" : $" stroke=\"{stroke}\" stroke-width=\"1.2\"";
        return $"<path d=\"M11 1.5 C11 1.5 3.5 10 3.5 14.2 a7.5 7.5 0 0 0 15 0 C18.5 10 11 1.5 11 1.5 Z\" fill=\"{fill}\"{outline}/>";
    }

    private static string Clean()
    {
        return $"<rect x=\"4\" y=\"4\" width=\"14\" height=\"14\" rx=\"2\" fill=\"none\" stroke=\"{Grey}\" stroke-width=\"1.4\"/>";
    }
}
