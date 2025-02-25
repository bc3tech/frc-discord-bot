namespace FIRST.Model;
using Microsoft.Extensions.EnumStrings;

using System.Runtime.Serialization;

[EnumStrings(ExtensionClassModifiers = "public static")]
public enum TournamentLevel
{
    [EnumMember(Value = "practice")]
    Practice = 2,
    [EnumMember(Value = "qual")]
    Qualification = 3,
    [EnumMember(Value = "playoff")]
    Playoff = 4,
}
