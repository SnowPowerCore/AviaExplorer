using Refit;
using System.Threading.Tasks;

namespace AviaExplorer.Services.Avia.Api
{
    public interface IAviaApi
    {
        [Get("/supported_directions.json")]
        Task<TResult> GetSupportedDirections<TResult>([AliasAs("origin_iata")] string originIATA,
                                                     [AliasAs("one_way")] bool oneWay,
                                                     [AliasAs("locale")] string language);

        [Get("/prices.json")]
        Task<TResult> GetPrices<TResult>([AliasAs("origin_iata")] string originIATA,
                                         [AliasAs("one_way")] bool oneWay,
                                         [AliasAs("locale")] string language,
                                         [AliasAs("period")] string periodDate,
                                         [AliasAs("direct")] bool direct,
                                         [AliasAs("price")] string maxPrice,
                                         [AliasAs("no_visa")] bool noVisa,
                                         [AliasAs("schengen")] bool schengen,
                                         [AliasAs("need_visa")] bool needVisa,
                                         [AliasAs("min_trip_duration_in_days")] string minTripDurationInDays,
                                         [AliasAs("max_trip_duration_in_days")] string maxTripDurationInDays);
    }
}