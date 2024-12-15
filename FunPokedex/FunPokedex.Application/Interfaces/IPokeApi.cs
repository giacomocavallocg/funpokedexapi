using FunPokedex.Application.Models;

namespace FunPokedex.Application.Interfaces
{
    public interface IPokeApi
    {
        internal Task<PokemonSpecieDto?> GetPokemonSpecies(string name, CancellationToken token);
    }
}
