using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;
using FunPokedex.Application.Services;
using Moq;

namespace FunPokedex.Tests.FunPokedex.Application.Services
{
    public class PokedexServiceTest
    {
        private readonly Mock<IPokeApi> _pokeApiMock;
        private readonly Mock<IApplicationCache> _cacheMock;

        public PokedexServiceTest()
        {
            _pokeApiMock = new Mock<IPokeApi>();
            _cacheMock = new Mock<IApplicationCache>();
        }


        [Fact]
        public async Task GetPokemon_Ok()
        {

            PokemonSpecieDto specie = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "test",
                Habitat = "cave",
                FlavorTexts = [new() { FlavorText = "text1", Language = "en" }, new() { FlavorText = "text2", Language = "it" }],
                IsLegendary = true,
                IsMythical = true,
            };

            _pokeApiMock.Setup(s => s.GetPokemonSpecies("test")).ReturnsAsync(specie);

            var service = new PokedexService(_pokeApiMock.Object, _cacheMock.Object);

            var result = await service.GetPokemon("test");
            Assert.True(result.IsSuccess);
            Assert.Equal("text1", result.GetValueOrThrow().Description);

            // no en description
            specie = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "test2",
                Habitat = "cave",
                FlavorTexts = [new() { FlavorText = "text1", Language = "fr" }, new() { FlavorText = "text2", Language = "it" }],
                IsLegendary = true,
                IsMythical = true,
            };

            _pokeApiMock.Setup(s => s.GetPokemonSpecies("test2")).ReturnsAsync(specie);


            result = await service.GetPokemon("test2");
            Assert.True(result.IsSuccess);
            Assert.Equal(String.Empty, result.GetValueOrThrow().Description);
        }

        [Fact]
        public async Task GetPokemon_Ok_FromCache()
        {

            Pokemon pokemon = new()
            {
                Name = "test",
                Habitat = "cave",
                Description = "text1",
                IsLegendary = true,
            };

            _cacheMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out pokemon!)).Returns(true);

            var service = new PokedexService(_pokeApiMock.Object, _cacheMock.Object);

            var result = await service.GetPokemon("test");
            Assert.True(result.IsSuccess);
            Assert.Equal("text1", result.GetValueOrThrow().Description);

            _pokeApiMock.Verify(s => s.GetPokemonSpecies("test"), Times.Never());
        }

        [Fact]
        public async Task GetPokemon_NotExists()
        {

            _pokeApiMock.Setup(s => s.GetPokemonSpecies("test")).ReturnsAsync(() => null);

            var service = new PokedexService(_pokeApiMock.Object, _cacheMock.Object);

            var result = await service.GetPokemon("test");
            Assert.False(result.IsSuccess);
            Assert.Equal(FailureType.NotFound, result.FailureType);

        }

        [Fact]
        public async Task GetPokemon_Exception()
        {
            _pokeApiMock.Setup(s => s.GetPokemonSpecies("test")).ThrowsAsync(new Exception());
            var service = new PokedexService(_pokeApiMock.Object, _cacheMock.Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.GetPokemon("test"));
        }

        [Fact]
        public async Task GetPokemon_Exception_Cache()
        {
            Pokemon? p = null;
            _cacheMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out p)).Throws(new Exception());
            var service = new PokedexService(_pokeApiMock.Object, _cacheMock.Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.GetPokemon("test"));
        }
    }
}
