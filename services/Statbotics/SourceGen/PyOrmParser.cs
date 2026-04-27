namespace StatboticsKnownValues.SourceGen;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Parses Statbotics SQLAlchemy ORM Python files to extract column names per ORM class.
/// Uses a line-based regex state machine — the format is stable enough that a full
/// Python AST parser is overkill. If upstream conventions change in a way the regex
/// can't handle, the source generator emits STATBOT001 and the maintainer is forced
/// to either revisit the parser or pin the submodule to an earlier SHA.
/// </summary>
public static class PyOrmParser
{
    private static readonly Regex s_classDeclaration = new(
        @"^class\s+(?<name>\w+ORM)\s*\(",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex s_columnDeclaration = new(
        @"^\s+(?<name>\w+)\s*:\s*[\w\[\]\s\.]+\s*=\s*mapped_column\s*\(",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Parses a Python file's text. Returns ORM class name → ordered set of column names.
    /// Lines whose first non-whitespace character is '#' are skipped (whole-line comments).
    /// Inline comments after a declaration are tolerated by the regex (matching only the
    /// part before the inline '#').
    /// </summary>
    public static IReadOnlyDictionary<string, ImmutableArray<string>> Parse(string fileContent)
    {
        var result = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        string? currentClass = null;

        using var reader = new StringReader(fileContent);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            string trimmed = line.TrimStart();
            if (trimmed.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            Match classMatch = s_classDeclaration.Match(line);
            if (classMatch.Success)
            {
                currentClass = classMatch.Groups["name"].Value;
                if (!result.ContainsKey(currentClass))
                {
                    result[currentClass] = new List<string>();
                }

                continue;
            }

            if (currentClass is null)
            {
                continue;
            }

            Match colMatch = s_columnDeclaration.Match(line);
            if (colMatch.Success)
            {
                result[currentClass].Add(colMatch.Groups["name"].Value);
            }
        }

        return result.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToImmutableArray(),
            StringComparer.Ordinal);
    }
}
