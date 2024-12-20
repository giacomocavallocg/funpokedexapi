﻿namespace FunPokedex.Api.Models
{
    public record PokemonDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string? Habitat { get; set; }
        public required bool IsLegendary { get; set; }
    }
}
