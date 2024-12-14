using FunPokedex.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FunPokedex.Api.Controllers
{
    [ApiController]
    [Route("pokemon")]
    public class PokedexController(ILogger<PokedexController> logger) : ControllerBase
    {

        private readonly ILogger<PokedexController> _logger = logger;


        [HttpGet("{pokemonName}")]
        public ActionResult<PokemonDto> GetPokemon([BindRequired, FromRoute] string pokemonName)
        {
            _logger.LogDebug($"Executed GET GetPokemon API");
            return new PokemonDto()
            {
                Name = pokemonName,
                Description = $"fake description for {pokemonName}",
                Habitat = $"fake habitat for {pokemonName}",
                IsLegendary = false,
                HasFunDescription = false,
            };
        }

        [HttpGet("/translated/{pokemonName}")]
        public ActionResult<PokemonDto> GetPokemonTranslated([BindRequired, FromRoute] string pokemonName)
        {
            _logger.LogDebug($"Executed GET GetPokemonTranslated API");
            return new PokemonDto()
            {
                Name = pokemonName,
                Description = $"fake fun description for {pokemonName}",
                Habitat = $"fake habitat for {pokemonName}",
                IsLegendary = false,
                HasFunDescription = true,
            };
        }
    }
}
