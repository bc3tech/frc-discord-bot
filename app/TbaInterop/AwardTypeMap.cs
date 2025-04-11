namespace FunctionApp.TbaInterop;

using Microsoft.Extensions.EnumStrings;

using System.Text.Json.Serialization;

[EnumStrings]
[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum EventType : int
{
    Regional = 0,
    District = 1,
    DistrictCmp = 2,
    CmpDivision = 3,
    CmpFinals = 4,
    DistrictCmpDivision = 5,
    Foc = 6,
    Remote = 7,
    Offseason = 99,
    Preseason = 100,
    Unlabeled = -1
}

[EnumStrings]
[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum AwardType
{
    Chairmans = 0,
    Winner = 1,
    Finalist = 2,
    WoodieFlowers = 3,
    DeansList = 4,
    Volunteer = 5,
    Founders = 6,
    BartKamenMemorial = 7,
    MakeItLoud = 8,
    EngineeringInspiration = 9,
    RookieAllStar = 10,
    GraciousProfessionalism = 11,
    Coopertition = 12,
    Judges = 13,
    HighestRookieSeed = 14,
    RookieInspiration = 15,
    IndustrialDesign = 16,
    Quality = 17,
    Safety = 18,
    Sportsmanship = 19,
    Creativity = 20,
    EngineeringExcellence = 21,
    Entrepreneurship = 22,
    ExcellenceInDesign = 23,
    ExcellenceInDesignCad = 24,
    ExcellenceInDesignAnimation = 25,
    DrivingTomorrowsTechnology = 26,
    Imagery = 27,
    MediaAndTechnology = 28,
    InnovationInControl = 29,
    Spirit = 30,
    Website = 31,
    Visualization = 32,
    AutodeskInventor = 33,
    FutureInnovator = 34,
    RecognitionOfExtraordinaryService = 35,
    OutstandingCart = 36,
    WsuAimHigher = 37,
    LeadershipInControl = 38,
    Num1Seed = 39,
    IncrediblePlay = 40,
    PeoplesChoiceAnimation = 41,
    VisualizationRisingStar = 42,
    BestOffensiveRound = 43,
    BestPlayOfTheDay = 44,
    FeatherweightInTheFinals = 45,
    MostPhotogenic = 46,
    OutstandingDefense = 47,
    PowerToSimplify = 48,
    AgainstAllOdds = 49,
    RisingStar = 50,
    ChairmansHonorableMention = 51,
    ContentCommunicationHonorableMention = 52,
    TechnicalExecutionHonorableMention = 53,
    Realization = 54,
    RealizationHonorableMention = 55,
    DesignYourFuture = 56,
    DesignYourFutureHonorableMention = 57,
    SpecialRecognitionCharacterAnimation = 58,
    HighScore = 59,
    TeacherPioneer = 60,
    BestCraftsmanship = 61,
    BestDefensiveMatch = 62,
    PlayOfTheDay = 63,
    Programming = 64,
    Professionalism = 65,
    GoldenCorndog = 66,
    MostImprovedTeam = 67,
    Wildcard = 68,
    ChairmansFinalist = 69,
    Other = 70,
    Autonomous = 71,
    InnovationChallengeSemiFinalist = 72,
    RookieGameChanger = 73,
    SkillsCompetitionWinner = 74,
    SkillsCompetitionFinalist = 75,
    RookieDesign = 76,
    EngineeringDesign = 77,
    Designers = 78,
    Concept = 79,
    GameDesignChallengeWinner = 80,
    GameDesignChallengeFinalist = 81
}

internal static class AwardTypeEnumExtensions
{
    private static readonly HashSet<AwardType> BlueBannerAwards =
    [
        AwardType.Chairmans,
        AwardType.ChairmansFinalist,
        AwardType.Winner,
        AwardType.WoodieFlowers,
        AwardType.SkillsCompetitionWinner,
        AwardType.GameDesignChallengeWinner
    ];
    public static bool IsBlueBanner(this AwardType award) => BlueBannerAwards.Contains(award);

    public static readonly HashSet<AwardType> IndividualAwards =
    [
        AwardType.WoodieFlowers,
        AwardType.DeansList,
        AwardType.Volunteer,
        AwardType.Founders,
        AwardType.BartKamenMemorial,
        AwardType.MakeItLoud
    ];

    public static readonly HashSet<AwardType> NonJudgedNonTeamAwards =
    [
        AwardType.HighestRookieSeed,
        AwardType.WoodieFlowers,
        AwardType.DeansList,
        AwardType.Volunteer,
        AwardType.Winner,
        AwardType.Finalist,
        AwardType.Wildcard
    ];

    public static readonly string?[][] NormalizedName =
    [
        ["Chairman's Award", null],
        ["Chairman's Award Finalist", null],
        ["Winner", null],
        ["Woodie Flowers Finalist Award", "Woodie Flowers Award"]
    ];

    public static readonly string?[] Searchable =
    [
        "Chairman's",
        "Chairman's Finalist",
        "Engineering Inspiration",
        "Coopertition",
        "Creativity",
        "Engineering Excellence",
        "Entrepreneurship",
        "Dean's List",
        "Bart Kamen Memorial",
        "Gracious Professionalism",
        "Highest Rookie Seed",
        "Imagery",
        "Industrial Design",
        "Safety",
        "Innovation in Control",
        "Quality",
        "Rookie All Star",
        "Rookie Inspiration",
        "Spirit",
        "Volunteer",
        "Woodie Flowers",
        "Judges'"
    ];
}
