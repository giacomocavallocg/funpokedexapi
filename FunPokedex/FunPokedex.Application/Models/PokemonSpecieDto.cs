

namespace FunPokedex.Application.Models
{
    internal record PokemonSpecieDto
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public required List<PokemonSpecieTextDto> FlavorTexts { get; set; }

        public string? Habitat { get; set; }

        public bool IsLegendary { get; set; } = false;

        public bool IsMythical { get; set; } = false;

    }

    internal record PokemonSpecieTextDto
    {
        public required string FlavorText { get; set; }

        public required string Language { get; set; }
    }
}
