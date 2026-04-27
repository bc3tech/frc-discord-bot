namespace StatboticsKnownValues.SourceGen.Tests;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using StatboticsKnownValues.SourceGen;

using Xunit;

public sealed class StatboticsKnownValuesGeneratorTests
{
    private static string LoadFixture(string name)
        => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", name));

    private static GeneratorDriverRunResult RunGenerator(IEnumerable<(string Path, string Content)> files)
    {
        var compilation = CSharpCompilation.Create("TestAssembly");
        var generator = new StatboticsKnownValuesGenerator();

        AdditionalText[] additionalTexts = [.. files.Select(f => (AdditionalText)new InMemoryAdditionalText(f.Path, f.Content))];

        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(generator)
            .AddAdditionalTexts([.. additionalTexts])
            .RunGenerators(compilation);

        return driver.GetRunResult();
    }

    [Fact]
    public void Generator_HappyPath_EmitsCompilableSource()
    {
        var result = RunGenerator(
        [
            ("vendor/statbotics/backend/src/db/models/team.py", LoadFixture("team_simple.py")),
            ("vendor/statbotics-extras/country-state.json", LoadFixture("country-state.json")),
        ]);

        Assert.Empty(result.Diagnostics);
        var generatedSource = Assert.Single(result.GeneratedTrees);
        string text = generatedSource.ToString();
        Assert.Contains("internal static class StatboticsKnownValues", text);
        Assert.Contains("[\"/v3/teams\"] = ImmutableHashSet.Create(StringComparer.Ordinal", text);
        Assert.Contains("\"Canada\"", text);
        Assert.Contains("\"NC\"", text);
    }

    [Fact]
    public void Generator_ZeroColumnsInOrmFile_EmitsStatbot001Diagnostic()
    {
        var result = RunGenerator(
        [
            // empty.py declares EmptyORM with no mapped_column declarations,
            // but EmptyORM isn't in the EndpointMapping, so STATBOT003 fires first.
            // For STATBOT001 we need a mapped class that produces zero columns.
            ("vendor/statbotics/backend/src/db/models/team.py",
             "class TeamORM(Base):\n    pass\n"),
            ("vendor/statbotics-extras/country-state.json", LoadFixture("country-state.json")),
        ]);

        Assert.Contains(result.Diagnostics, d => d.Id == "STATBOT001");
    }

    [Fact]
    public void Generator_MissingCountryStateJson_EmitsStatbot002Diagnostic()
    {
        var result = RunGenerator(
        [
            ("vendor/statbotics/backend/src/db/models/team.py", LoadFixture("team_simple.py")),
            // no country-state.json
        ]);

        Assert.Contains(result.Diagnostics, d => d.Id == "STATBOT002");
    }

    [Fact]
    public void Generator_OrmClassNotInMapping_EmitsStatbot003Warning()
    {
        var result = RunGenerator(
        [
            // empty.py has class EmptyORM which isn't in the mapping table
            ("vendor/statbotics/backend/src/db/models/empty.py", LoadFixture("empty.py")),
            ("vendor/statbotics-extras/country-state.json", LoadFixture("country-state.json")),
        ]);

        Assert.Contains(result.Diagnostics, d => d.Id == "STATBOT003");
    }

    [Fact]
    public void Generator_SortsCollections_ProducesByteForByteIdenticalOutput()
    {
        // Run twice with identical inputs and confirm the generated source is identical.
        var inputs = new[]
        {
            ("vendor/statbotics/backend/src/db/models/team.py", LoadFixture("team_simple.py")),
            ("vendor/statbotics/backend/src/db/models/match.py", LoadFixture("match_with_comments.py")),
            ("vendor/statbotics-extras/country-state.json", LoadFixture("country-state.json")),
        };

        var result1 = RunGenerator(inputs);
        var result2 = RunGenerator(inputs);

        string source1 = result1.GeneratedTrees.Single().ToString();
        string source2 = result2.GeneratedTrees.Single().ToString();

        Assert.Equal(source1, source2);
    }

    [Fact]
    public void Generator_MetricColumnsAreSortedOrdinal()
    {
        var result = RunGenerator(
        [
            ("vendor/statbotics/backend/src/db/models/team.py", LoadFixture("team_simple.py")),
            ("vendor/statbotics-extras/country-state.json", LoadFixture("country-state.json")),
        ]);

        string text = result.GeneratedTrees.Single().ToString();
        // team_simple.py declares: team, name, country, rookie_year, epa
        // After ordinal sort: country, epa, name, rookie_year, team
        int countryIdx = text.IndexOf("\"country\"", System.StringComparison.Ordinal);
        int epaIdx = text.IndexOf("\"epa\"", System.StringComparison.Ordinal);
        int nameIdx = text.IndexOf("\"name\"", System.StringComparison.Ordinal);
        int teamIdx = text.IndexOf("\"team\"", System.StringComparison.Ordinal);

        Assert.True(countryIdx > 0, "country should appear in generated source");
        Assert.True(countryIdx < epaIdx, $"Expected 'country' ({countryIdx}) before 'epa' ({epaIdx})");
        Assert.True(epaIdx < nameIdx, $"Expected 'epa' ({epaIdx}) before 'name' ({nameIdx})");
        Assert.True(nameIdx < teamIdx, $"Expected 'name' ({nameIdx}) before 'team' ({teamIdx})");
    }

    [Fact]
    public void Generator_JsonParserHandlesEscapedQuotesInValues()
    {
        // Defensive regression: if a country name ever contains a literal quote
        // (escaped as \" in JSON), the hand-rolled JSON parser must not terminate
        // the string early. Today CountryStateRefresh writes raw UTF-8 so this
        // shape doesn't appear in production snapshots, but the parser must
        // handle it correctly to avoid silent data corruption if invariants drift.
        const string escapedJson = """
            {
              "countries": ["Côte d\"Ivoire", "USA"],
              "states": ["NC"]
            }
            """;

        var result = RunGenerator(
        [
            ("vendor/statbotics/backend/src/db/models/team.py", LoadFixture("team_simple.py")),
            ("vendor/statbotics-extras/country-state.json", escapedJson),
        ]);

        Assert.Empty(result.Diagnostics);
        string source = result.GeneratedTrees.Single().ToString();
        // The parser must decode \" as a literal " in the value, then the
        // generator's own Escape() function re-escapes it for the C# source literal.
        Assert.Contains("\"Côte d\\\"Ivoire\"", source);
        Assert.Contains("\"USA\"", source);
    }
}

internal sealed class InMemoryAdditionalText(string path, string content) : AdditionalText
{
    public override string Path { get; } = path;

    public override Microsoft.CodeAnalysis.Text.SourceText GetText(System.Threading.CancellationToken cancellationToken = default)
        => Microsoft.CodeAnalysis.Text.SourceText.From(content);
}
