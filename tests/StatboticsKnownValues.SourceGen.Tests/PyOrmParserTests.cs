namespace StatboticsKnownValues.SourceGen.Tests;

using System.IO;
using System.Linq;

using StatboticsKnownValues.SourceGen;

using Xunit;

public sealed class PyOrmParserTests
{
    private static string LoadFixture(string name)
        => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", name));

    [Fact]
    public void Parse_SimpleHappyPath_ExtractsAllColumns()
    {
        string content = LoadFixture("team_simple.py");

        var result = PyOrmParser.Parse(content);

        Assert.Single(result);
        Assert.True(result.ContainsKey("TeamORM"));
        Assert.Equal(
            new[] { "team", "name", "country", "rookie_year", "epa" },
            result["TeamORM"]);
    }

    [Fact]
    public void Parse_MultilineMappedColumnWithMappedAnnotation_ExtractsColumnName()
    {
        string content = LoadFixture("team_event_multiline.py");

        var result = PyOrmParser.Parse(content);

        Assert.Single(result);
        Assert.True(result.ContainsKey("TeamEventORM"));
        Assert.Contains("team", result["TeamEventORM"]);
        Assert.Contains("type", result["TeamEventORM"]);
        Assert.Contains("week", result["TeamEventORM"]);
        Assert.Contains("first_event", result["TeamEventORM"]);
    }

    [Fact]
    public void Parse_WithCommentsAndInlineComments_IgnoresCommentedDeclarations()
    {
        string content = LoadFixture("match_with_comments.py");

        var result = PyOrmParser.Parse(content);

        Assert.Single(result);
        Assert.True(result.ContainsKey("MatchORM"));
        Assert.Equal(
            new[] { "match_key", "red_score", "blue_score", "time" },
            result["MatchORM"]);
        Assert.DoesNotContain("commented_out", result["MatchORM"]);
    }

    [Fact]
    public void Parse_EmptyOrmClass_ReturnsEmptyArray()
    {
        string content = LoadFixture("empty.py");

        var result = PyOrmParser.Parse(content);

        Assert.Single(result);
        Assert.True(result.ContainsKey("EmptyORM"));
        Assert.Empty(result["EmptyORM"]);
    }

    [Fact]
    public void Parse_NoOrmClass_ReturnsEmptyDictionary()
    {
        string content = "from sqlalchemy.orm import mapped_column\n\nclass NotAnOrm:\n    foo: int = mapped_column(Integer)\n";

        var result = PyOrmParser.Parse(content);

        Assert.Empty(result);
    }
}
