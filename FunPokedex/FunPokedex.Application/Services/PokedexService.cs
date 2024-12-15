
using System.Transactions;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;


namespace FunPokedex.Application.Services
{
    public class PokedexService(IPokeApi pokeApi, IApplicationCache cache) : IPokedexService
    {
        private readonly IPokeApi _pokeApi = pokeApi;
        private readonly IApplicationCache _cache = cache;

        public async Task<PokedexResult<Pokemon>> GetPokemon(string name)
        {

            if (_cache.TryGetValue(GetCacheKey(name), out Pokemon? cachePokemon))
                return PokedexResult<Pokemon>.Success(cachePokemon!);

            PokemonSpecieDto? specie = await _pokeApi.GetPokemonSpecies(name);

            if (specie == null) return PokedexResult<Pokemon>.Fail($"species {name} not found", FailureType.NotFound);

            Pokemon pokemon = new()
            {
                Name = specie.Name,
                Description = specie.FlavorTexts.FirstOrDefault(d => d.Language == "en")?.FlavorText ?? String.Empty,
                Habitat = specie.Habitat,
                IsLegendary = specie.IsLegendary,
            };

            _cache.SetValue(GetCacheKey(pokemon.Name), pokemon);
            return PokedexResult<Pokemon>.Success(pokemon);
        }

        private static string GetCacheKey(string pokemonName) => string.Format(CACHE_FOMRAT, pokemonName);

        private const string CACHE_FOMRAT = "species.{0}";
    }
}
