namespace Common.Tba.Api;

using Microsoft.Extensions.EnumStrings;

using System.Collections.Generic;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Used to form the normalized names programmatically")]
public enum EventType
{
    CMP_FINALS,

    NONE = 999
}

[EnumStrings(ExtensionClassModifiers = "public static partial")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Used to form the normalized names programmatically")]
public enum AwardType
{
    Chairmans = 0,
    Winner = 1,
    Finalist = 2,
    Woodie_Flowers = 3,
    Deans_List = 4,
    Volunteer = 5,
    Founders = 6,
    Bart_Kamen_Memorial = 7,
    Make_It_Loud = 8,
    Engineering_Inspiration = 9,
    Rookie_All_Star = 10,
    Gracious_Professionalism = 11,
    Coopertition = 12,
    Judges = 13,
    Highest_Rookie_Seed = 14,
    Rookie_Inspiration = 15,
    Industrial_Design = 16,
    Quality = 17,
    Safety = 18,
    Sportsmanship = 19,
    Creativity = 20,
    Engineering_Excellence = 21,
    Entrepreneurship = 22,
    Excellence_In_Design = 23,
    Excellence_In_Design_CAD = 24,
    Excellence_In_Design_Animation = 25,
    Driving_Tomorrows_Technology = 26,
    Imagery = 27,
    Media_And_Technology = 28,
    Innovation_In_Control = 29,
    Spirit = 30,
    Website = 31,
    Visualization = 32,
    Autodesk_Inventor = 33,
    Future_Innovator = 34,
    Recognition_Of_Extraordinary_Service = 35,
    Outstanding_Cart = 36,
    WSU_Aim_Higher = 37,
    Leadership_In_Control = 38,
    Num_1_Seed = 39,
    Incredible_Play = 40,
    Peoples_Choice_Animation = 41,
    Visualization_Rising_Star = 42,
    Best_Offensive_Round = 43,
    Best_Play_Of_The_Day = 44,
    Featherweight_In_The_Finals = 45,
    Most_Photogenic = 46,
    Outstanding_Defense = 47,
    Power_To_Simplify = 48,
    Against_All_Odds = 49,
    Rising_Star = 50,
    Chairmans_Honorable_Mention = 51,
    Content_Communication_Honorable_Mention = 52,
    Technical_Execution_Honorable_Mention = 53,
    Realization = 54,
    Realization_Honorable_Mention = 55,
    Design_Your_Future = 56,
    Design_Your_Future_Honorable_Mention = 57,
    Special_Recognition_Character_Animation = 58,
    High_Score = 59,
    Teacher_Pioneer = 60,
    Best_Craftsmanship = 61,
    Best_Defensive_Match = 62,
    Play_Of_The_Day = 63,
    Programming = 64,
    Professionalism = 65,
    Golden_Corndog = 66,
    Most_Improved_Team = 67,
    Wildcard = 68,
    Chairmans_Finalist = 69,
    Other = 70,
    Autonomous = 71,
    Innovation_Challenge_Semi_Finalist = 72,
    Rookie_Game_Changer = 73,
    Skills_Competition_Winner = 74,
    Skills_Competition_Finalist = 75,
    Rookie_Design = 76,
    Engineering_Design = 77,
    Designers = 78,
    Concept = 79,
    Game_Design_Challenge_Winner = 80,
    Game_Design_Challenge_Finalist = 81
}

public static partial class AwardTypeExtensions
{
    private static readonly HashSet<AwardType> BlueBannerAwards =
    [
        AwardType.Chairmans, AwardType.Chairmans_Finalist, AwardType.Winner,
        AwardType.Woodie_Flowers, AwardType.Skills_Competition_Winner, AwardType.Game_Design_Challenge_Winner
    ];

    private static readonly HashSet<AwardType> IndividualAwards =
    [
        AwardType.Woodie_Flowers, AwardType.Deans_List, AwardType.Volunteer,
        AwardType.Founders, AwardType.Bart_Kamen_Memorial, AwardType.Make_It_Loud
    ];

    private static readonly HashSet<AwardType> NonJudgedNonTeamAwards =
    [
        AwardType.Highest_Rookie_Seed, AwardType.Woodie_Flowers, AwardType.Deans_List,
        AwardType.Volunteer, AwardType.Winner, AwardType.Finalist, AwardType.Wildcard
    ];

    public static bool IsBlueBannerAward(this AwardType awardType) => BlueBannerAwards.Contains(awardType);
    public static bool IsIndividualAward(this AwardType awardType) => IndividualAwards.Contains(awardType);
    public static bool IsNonJudgedNonTeamAward(this AwardType awardType) => NonJudgedNonTeamAwards.Contains(awardType);

    private static readonly Dictionary<AwardType, Dictionary<EventType, string>> NormalizedNames = new()
    {
        { AwardType.Chairmans, new Dictionary<EventType, string> { { EventType.NONE, "Chairman's Award" } } },
        { AwardType.Chairmans_Finalist, new Dictionary<EventType, string> { { EventType.NONE, "Chairman's Award Finalist" } } },
        { AwardType.Winner, new Dictionary<EventType, string> { { EventType.NONE, "Winner" } } },
        { AwardType.Woodie_Flowers, new Dictionary<EventType, string>
            {
                {  EventType.NONE, "Woodie Flowers Finalist Award" },
                { EventType.CMP_FINALS, "Woodie Flowers Award" }
            }
        }
    };

    public static string GetNormalizedName(this AwardType awardType, EventType eventType = EventType.NONE) => 
        NormalizedNames.TryGetValue(awardType, out var names) && names.TryGetValue(eventType, out var name)
            ? name
            : awardType.ToInvariantString().Replace('_', ' ');

    public static readonly Dictionary<AwardType, string> SEARCHABLE = new()
    {
        { AwardType.Chairmans, "Chairman's" },
        { AwardType.Chairmans_Finalist, "Chairman's Finalist" },
        { AwardType.Engineering_Inspiration, "Engineering Inspiration" },
        { AwardType.Coopertition, "Coopertition" },
        { AwardType.Creativity, "Creativity" },
        { AwardType.Engineering_Excellence, "Engineering Excellence" },
        { AwardType.Entrepreneurship, "Entrepreneurship" },
        { AwardType.Deans_List, "Dean's List" },
        { AwardType.Bart_Kamen_Memorial, "Bart Kamen Memorial" },
        { AwardType.Gracious_Professionalism, "Gracious Professionalism" },
        { AwardType.Highest_Rookie_Seed, "Highest Rookie Seed" },
        { AwardType.Imagery, "Imagery" },
        { AwardType.Industrial_Design, "Industrial Design" },
        { AwardType.Safety, "Safety" },
        { AwardType.Innovation_In_Control, "Innovation in Control" },
        { AwardType.Quality, "Quality" },
        { AwardType.Rookie_All_Star, "Rookie All Star" },
        { AwardType.Rookie_Inspiration, "Rookie Inspiration" },
        { AwardType.Spirit, "Spirit" },
        { AwardType.Volunteer, "Volunteer" },
        { AwardType.Woodie_Flowers, "Woodie Flowers" },
        { AwardType.Judges, "Judges'" }
    };
}
