using FunPokedex.Api.Models;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FunPokedex.Api.Controllers
{
    [ApiController]
    [Route("pokemon")]
    public class PokedexController(IPokedexService pokedexSerivice, IFunPokedexService funService, ILogger<PokedexController> logger) : ControllerBase
    {
        private readonly IPokedexService _pokedexService = pokedexSerivice;
        private readonly IFunPokedexService _funService = funService;
        private readonly ILogger<PokedexController> _logger = logger;


        [HttpGet("{pokemonName}")]
        public async Task<ActionResult<PokemonDto>> GetPokemon([BindRequired, FromRoute] string pokemonName)
        {
            PokedexResult<Pokemon> result = await _pokedexService.GetPokemon(pokemonName);

            if (!result.IsSuccess)
                return HandleFailResult(result);

            Pokemon pokemon = result.GetValueOrThrow();

            return new PokemonDto()
            {
                Name = pokemon.Name,
                Description = pokemon.Description,
                Habitat = pokemon.Habitat,
                IsLegendary = pokemon.IsLegendary,
                HasFunDescription = false,
            };
        }

        [HttpGet("/translated/{pokemonName}")]
        public async Task<ActionResult<PokemonDto>> GetPokemonTranslated([BindRequired, FromRoute] string pokemonName)
        {
            PokedexResult<Pokemon> result = await _pokedexService.GetPokemon(pokemonName);

            if (!result.IsSuccess)
                return HandleFailResult(result);

            Pokemon pokemon = result.GetValueOrThrow();

            PokedexResult<string> funTraslation = await _funService.GetFunDescription(pokemon);

            return new PokemonDto()
            {
                Name = pokemon.Name,
                Description = funTraslation.IsSuccess ? funTraslation.GetValueOrThrow() : pokemon.Description,
                Habitat = pokemon.Habitat,
                IsLegendary = pokemon.IsLegendary,
                HasFunDescription = funTraslation.IsSuccess,
            };
        }

        private static ActionResult HandleFailResult<T>(PokedexResult<T> result)
        {
            return result.FailureType switch
            {
                FailureType.NotFound => new NotFoundObjectResult(ErrorDto.NotFoundError(result.Error!)),
                _ => throw new Exception("Unknown error occurred."),
            };
        }
    }
}
