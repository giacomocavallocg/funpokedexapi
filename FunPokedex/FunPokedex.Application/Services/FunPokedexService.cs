using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;

namespace FunPokedex.Application.Services
{
    public class FunPokedexService(IFunTranslationsApi translationApi, IApplicationCache cache) : IFunPokedexService
    {

        private readonly IFunTranslationsApi _translationsApi = translationApi;
        private readonly IApplicationCache _cache = cache;

        public async Task<PokedexResult<string>> GetFunDescription(Pokemon pokemon)
        {
            if (_cache.TryGetValue(GetCacheKey(pokemon), out string? cacheTranslation))
                return PokedexResult<string>.Success(cacheTranslation!);

            FunTranslationDto? translation;

            if ((!String.IsNullOrEmpty(pokemon.Habitat) && pokemon.Habitat == "cave") || pokemon.IsLegendary)
            {
                translation = await _translationsApi.GetYodaTranslation(pokemon.Description);
            }
            else
            {
                translation = await _translationsApi.GetShakespeareTranslation(pokemon.Description);
            }

            if (translation != null)
            {
                _cache.SetValue(GetCacheKey(pokemon), translation.Translation);
                return PokedexResult<string>.Success(translation.Translation);
            }

            return PokedexResult<string>.Success(pokemon.Description);
        }


        private static string GetCacheKey(Pokemon pokemon) => string.Format(CACHE_FOMRAT, pokemon.Name);

        private const string CACHE_FOMRAT = "translations.{0}";
    }
}
