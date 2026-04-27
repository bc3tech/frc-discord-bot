namespace StatboticsKnownValues.SourceGen;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Roslyn incremental source generator that emits a single static C# class
/// (<c>StatboticsKnownValues</c>) containing the legal sets of values for the
/// Statbotics API's vague query parameters: per-endpoint <c>metric</c> column
/// names (sourced from upstream ORM declarations), and global <c>country</c> /
/// <c>state</c> values (sourced from a snapshot of <c>/v3/teams</c>).
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class StatboticsKnownValuesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<(string Path, string Content)>> pyFiles = context
            .AdditionalTextsProvider
            .Where(static f =>
                f.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                && f.Path.Replace('\\', '/').Contains("backend/src/db/models"))
            .Select(static (f, ct) => (f.Path, f.GetText(ct)?.ToString() ?? string.Empty))
            .Collect();

        IncrementalValueProvider<(string Path, string Content)?> jsonFile = context
            .AdditionalTextsProvider
            .Where(static f => Path.GetFileName(f.Path).Equals("country-state.json", StringComparison.OrdinalIgnoreCase))
            .Select(static (f, ct) => ((string Path, string Content)?)(f.Path, f.GetText(ct)?.ToString() ?? string.Empty))
            .Collect()
            .Select(static (arr, _) => arr.IsDefaultOrEmpty ? null : arr[0]);

        IncrementalValueProvider<(ImmutableArray<(string Path, string Content)> Py, (string Path, string Content)? Json)> combined =
            pyFiles.Combine(jsonFile);

        context.RegisterSourceOutput(combined, Emit);
    }

    private static void Emit(
        SourceProductionContext context,
        (ImmutableArray<(string Path, string Content)> Py, (string Path, string Content)? Json) input)
    {
        var metricColumns = new SortedDictionary<string, ImmutableArray<string>>(StringComparer.Ordinal);

        foreach ((string path, string content) in input.Py)
        {
            IReadOnlyDictionary<string, ImmutableArray<string>> classes = PyOrmParser.Parse(content);
            string fileName = Path.GetFileName(path);

            foreach (KeyValuePair<string, ImmutableArray<string>> kvp in classes)
            {
                string ormClassName = kvp.Key;
                ImmutableArray<string> columns = kvp.Value;

                if (!EndpointMapping.OrmClassToEndpoint.TryGetValue(ormClassName, out string? endpoint))
                {
                    if (!EndpointMapping.IntentionallyIgnored.Contains(ormClassName))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            Diagnostics.OrmClassNotInMapping,
                            Location.None,
                            ormClassName));
                    }

                    continue;
                }

                if (columns.IsDefaultOrEmpty)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Diagnostics.OrmFileProducedZeroColumns,
                        Location.None,
                        fileName));
                    continue;
                }

                ImmutableArray<string> sorted = [.. columns.OrderBy(static c => c, StringComparer.Ordinal)];
                metricColumns[endpoint] = sorted;
            }
        }

        ImmutableArray<string> sortedCountries = ImmutableArray<string>.Empty;
        ImmutableArray<string> sortedStates = ImmutableArray<string>.Empty;

        if (input.Json is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.CountryStateSnapshotMissing,
                Location.None));
        }
        else
        {
            (string[] countries, string[] states) = ParseCountryStateJson(input.Json.Value.Content);
            sortedCountries = [.. countries.OrderBy(static v => v, StringComparer.Ordinal)];
            sortedStates = [.. states.OrderBy(static v => v, StringComparer.Ordinal)];
        }

        context.AddSource(
            "StatboticsKnownValues.g.cs",
            SourceText.From(BuildSource(metricColumns, sortedCountries, sortedStates), Encoding.UTF8));
    }

    /// <summary>
    /// Parses the country-state.json snapshot. Hand-rolled string parsing instead of
    /// System.Text.Json so the analyzer only depends on Microsoft.CodeAnalysis.CSharp.
    /// The file shape is fixed by Tools/CountryStateRefresh and not user-editable.
    /// </summary>
    private static (string[] Countries, string[] States) ParseCountryStateJson(string json)
    {
        return (ExtractStringArray(json, "countries"), ExtractStringArray(json, "states"));
    }

    private static string[] ExtractStringArray(string json, string fieldName)
    {
        string marker = "\"" + fieldName + "\"";
        int idx = json.IndexOf(marker, StringComparison.Ordinal);
        if (idx < 0)
        {
            return Array.Empty<string>();
        }

        int openBracket = json.IndexOf('[', idx);
        if (openBracket < 0)
        {
            return Array.Empty<string>();
        }

        var values = new List<string>();
        var current = new StringBuilder();
        bool inString = false;
        bool escapeNext = false;

        for (int i = openBracket + 1; i < json.Length; i++)
        {
            char c = json[i];

            if (inString)
            {
                if (escapeNext)
                {
                    // Handle the standard JSON escapes that could appear in user data.
                    // Don't bother with \uXXXX here — CountryStateRefresh writes raw UTF-8
                    // (UnsafeRelaxedJsonEscaping), so unicode escapes never appear in our
                    // snapshot files. If that invariant ever changes, decode \uXXXX here.
                    char unescaped = c switch
                    {
                        '"' => '"',
                        '\\' => '\\',
                        '/' => '/',
                        'n' => '\n',
                        't' => '\t',
                        'r' => '\r',
                        'b' => '\b',
                        'f' => '\f',
                        _ => c,
                    };
                    current.Append(unescaped);
                    escapeNext = false;
                }
                else if (c == '\\')
                {
                    escapeNext = true;
                }
                else if (c == '"')
                {
                    values.Add(current.ToString());
                    current.Clear();
                    inString = false;
                }
                else
                {
                    current.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inString = true;
                }
                else if (c == ']')
                {
                    break;
                }
            }
        }

        return values.ToArray();
    }

    private static string BuildSource(
        SortedDictionary<string, ImmutableArray<string>> metricColumns,
        ImmutableArray<string> countries,
        ImmutableArray<string> states)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated by StatboticsKnownValues.SourceGen />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Collections.Immutable;");
        sb.AppendLine();
        sb.AppendLine("namespace ChatBot.Tools;");
        sb.AppendLine();
        sb.AppendLine("internal static class StatboticsKnownValues");
        sb.AppendLine("{");
        sb.AppendLine("    public static readonly IReadOnlyDictionary<string, ImmutableHashSet<string>> MetricColumns =");
        sb.AppendLine("        new Dictionary<string, ImmutableHashSet<string>>(StringComparer.Ordinal)");
        sb.AppendLine("        {");
        foreach (KeyValuePair<string, ImmutableArray<string>> kvp in metricColumns)
        {
            sb.Append("            [\"").Append(Escape(kvp.Key)).Append("\"] = ImmutableHashSet.Create(StringComparer.Ordinal");
            foreach (string col in kvp.Value)
            {
                sb.Append(", \"").Append(Escape(col)).Append('"');
            }

            sb.AppendLine("),");
        }

        sb.AppendLine("        };");
        sb.AppendLine();
        AppendImmutableHashSetField(sb, "KnownCountries", countries);
        sb.AppendLine();
        AppendImmutableHashSetField(sb, "KnownStates", states);
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void AppendImmutableHashSetField(StringBuilder sb, string fieldName, ImmutableArray<string> values)
    {
        sb.Append("    public static readonly ImmutableHashSet<string> ").Append(fieldName).AppendLine(" =");
        sb.Append("        ImmutableHashSet.Create(StringComparer.Ordinal");
        foreach (string v in values)
        {
            sb.Append(", \"").Append(Escape(v)).Append('"');
        }

        sb.AppendLine(");");
    }

    private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
