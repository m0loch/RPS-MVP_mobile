public partial class Utils
{
    public enum ClanList
    {
        ROCK = 0,
        PAPER = 1,
        SCISSORS = 2,
    }

    public static string GetClan(int clanId)
    {
        return ((ClanList)clanId).ToString();
    }
}