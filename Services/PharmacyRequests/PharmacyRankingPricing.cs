namespace darooyab_rewrite.Services.PharmacyRequests;

public static class PharmacyRankingPricing
{
    public static int GetDailyPriceToman(int rankNumber) => rankNumber switch
    {
        1 => 5000,
        2 => 4000,
        3 => 3500,
        4 => 3000,
        5 => 2500,
        6 => 2000,
        7 or 8 => 1500,
        9 or 10 => 1000,
        _ => throw new ArgumentOutOfRangeException(nameof(rankNumber))
    };

    public static int GetPriceRial(int rankNumber, int durationInDays)
    {
        if (durationInDays is < 1 or > 60)
        {
            throw new ArgumentOutOfRangeException(nameof(durationInDays));
        }

        return checked(GetDailyPriceToman(rankNumber) * 10 * durationInDays);
    }
}
