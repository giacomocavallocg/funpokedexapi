using System.Transactions;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;
using Newtonsoft.Json.Linq;

namespace FunPokedex.Application.Services
{
    public class FunPokedexService(IFunTranslationsApi translationApi, IApplicationCache cache) : IFunPokedexService
    {

        private readonly IFunTranslationsApi _translationsApi = translationApi;
        private readonly IApplicationCache _cache = cache;

        public async Task<PokedexResult<string>> GetFunDescription(Pokemon pokemon, CancellationToken token)
        {
            if (_cache.TryGetValue(GetCacheKey(pokemon), out string? cacheTranslation))
                return PokedexResult<string>.Success(cacheTranslation!);

            FunTranslationDto? translation = await GetFunTranslation(pokemon, token);

            if (translation != null)
            {
                _cache.SetValue(GetCacheKey(pokemon), translation.Translation);
                return PokedexResult<string>.Success(translation.Translation);
            }

            return PokedexResult<string>.Success(pokemon.Description);
        }

        private async Task<FunTranslationDto?> GetFunTranslation(Pokemon pokemon, CancellationToken token)
        {
            try
            {
                if ((!String.IsNullOrEmpty(pokemon.Habitat) && pokemon.Habitat == "cave") || pokemon.IsLegendary)
                {
                    return await _translationsApi.GetYodaTranslation(pokemon.Description, token);
                }
                else
                {
                    return await _translationsApi.GetShakespeareTranslation(pokemon.Description, token);
                }
            }
            catch(Exception)
            {
                return null;
            }
        } 


        private static string GetCacheKey(Pokemon pokemon) => string.Format(CACHE_FOMRAT, pokemon.Name);

        private const string CACHE_FOMRAT = "translations.{0}";
    }
}
