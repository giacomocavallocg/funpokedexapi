using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunPokedex.Api.Controllers;
using FunPokedex.Api.Models;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FunPokedex.Test.FunPokedex.Api.Controllers
{
    public class PokedexControllerTest
    {

        private readonly Mock<IPokedexService> _pokedexServiceMock;
        private readonly CancellationToken _token;

        public PokedexControllerTest()
        {
            _pokedexServiceMock = new Mock<IPokedexService>();
            _token = new CancellationToken();
        }

        [Fact]
        public async Task GetPokemon_OK()
        {
            Pokemon pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "",
                IsLegendary = true,
            };

            _pokedexServiceMock.Setup(a => a.GetPokemon(pokemon.Name, It.IsAny<CancellationToken>())).ReturnsAsync(PokedexResult<Pokemon>.Success(pokemon));

            PokedexController controller = new(_pokedexServiceMock.Object, new Mock<IFunPokedexService>().Object, MockedLogger);

            var response = await controller.GetPokemon(pokemon.Name, _token);
            Assert.NotNull(response.Value);
            Assert.IsType<PokemonDto>(response.Value);
            Assert.Equal(pokemon.Name, response.Value.Name);
            Assert.Equal(pokemon.Description, response.Value.Description);
            Assert.Equal(pokemon.Habitat, response.Value.Habitat);
            Assert.Equal(pokemon.IsLegendary, response.Value.IsLegendary);

            _pokedexServiceMock.Verify(a => a.GetPokemon(pokemon.Name, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPokemon_NotFound()
        {
            string pokemonName = "not-exists";

            _pokedexServiceMock.Setup(a => a.GetPokemon(pokemonName, It.IsAny<CancellationToken>())).ReturnsAsync(PokedexResult<Pokemon>.Fail("not found", FailureType.NotFound));

            PokedexController controller = new (_pokedexServiceMock.Object, new Mock<IFunPokedexService>().Object, MockedLogger);
            var response = await controller.GetPokemon(pokemonName, _token);
            Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.IsType<ErrorDto>(((NotFoundObjectResult)response.Result).Value);

            var dto = (ErrorDto) ((NotFoundObjectResult)response.Result).Value!;
            Assert.Equal("100", dto.Code);

            _pokedexServiceMock.Verify(a => a.GetPokemon(pokemonName, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task GetPokemon_Exception()
        {
            string pokemonName = "exception";

            _pokedexServiceMock.Setup(a => a.GetPokemon(pokemonName, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("generic exception"));

            PokedexController controller = new(_pokedexServiceMock.Object, new Mock<IFunPokedexService>().Object, MockedLogger);

            // unexpected errors are not manage by controller
            await Assert.ThrowsAnyAsync<Exception>(() => controller.GetPokemon(pokemonName, _token));
            _pokedexServiceMock.Verify(a => a.GetPokemon(pokemonName, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPokemonTranslated_OK()
        {
            Pokemon pokemon = new ()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "",
                IsLegendary = true,
            };

            string funDescription = "Funny description";

            _pokedexServiceMock.Setup(a => a.GetPokemon(pokemon.Name, It.IsAny<CancellationToken>())).ReturnsAsync(PokedexResult<Pokemon>.Success(pokemon));

            var mockFunPokedexService = new Mock<IFunPokedexService>();
            mockFunPokedexService.Setup(a => a.GetFunDescription(It.IsAny<Pokemon>(), It.IsAny<CancellationToken>())).ReturnsAsync(PokedexResult<string>.Success(funDescription));

            PokedexController controller = new(_pokedexServiceMock.Object, mockFunPokedexService.Object, MockedLogger);

            var response = await controller.GetPokemonTranslated(pokemon.Name, _token);
            Assert.NotNull(response.Value);
            Assert.IsType<PokemonDto>(response.Value);

            Assert.Equal(pokemon.Name, response.Value.Name);
            Assert.Equal(funDescription, response.Value.Description);
            Assert.Equal(pokemon.Habitat, response.Value.Habitat);
            Assert.Equal(pokemon.IsLegendary, response.Value.IsLegendary);

            _pokedexServiceMock.Verify(a => a.GetPokemon(pokemon.Name, It.IsAny<CancellationToken>()), Times.Once);
            mockFunPokedexService.Verify(a => a.GetFunDescription(It.IsAny<Pokemon>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPokemonTranslated_PockemonNotFound()
        {
            string pokemonName = "not-exists";
            _pokedexServiceMock.Setup(a => a.GetPokemon(pokemonName, It.IsAny<CancellationToken>())).ReturnsAsync(PokedexResult<Pokemon>.Fail("not found", FailureType.NotFound));

            var mockFunPokedexService = new Mock<IFunPokedexService>();

            PokedexController controller = new(_pokedexServiceMock.Object, mockFunPokedexService.Object, MockedLogger);
            var response = await controller.GetPokemonTranslated(pokemonName, _token);

            Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.IsType<ErrorDto>(((NotFoundObjectResult)response.Result).Value);

            var dto = (ErrorDto)((NotFoundObjectResult)response.Result).Value!;
            Assert.Equal("100", dto.Code);

            _pokedexServiceMock.Verify(a => a.GetPokemon(pokemonName, It.IsAny<CancellationToken>()), Times.Once);
            mockFunPokedexService.Verify(a => a.GetFunDescription(It.IsAny<Pokemon>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetPokemonTranslated_DescriptionNotFound()
        {
            Pokemon pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "",
                IsLegendary = true,
            };

            _pokedexServiceMock.Setup(a => a.GetPokemon(pokemon.Name, It.IsAny<CancellationToken>())).ReturnsAsync(PokedexResult<Pokemon>.Success(pokemon));

            var mockFunPokedexService = new Mock<IFunPokedexService>();
            mockFunPokedexService.Setup(a => a.GetFunDescription(It.IsAny<Pokemon>(), It.IsAny<CancellationToken>())).ReturnsAsync(PokedexResult<string>.Fail("not found", FailureType.NotFound));

            PokedexController controller = new(_pokedexServiceMock.Object, mockFunPokedexService.Object, MockedLogger);

            var response = await controller.GetPokemonTranslated(pokemon.Name, _token);
            Assert.NotNull(response.Value);
            Assert.IsType<PokemonDto>(response.Value);

            Assert.Equal(pokemon.Name, response.Value.Name);
            Assert.Equal(pokemon.Description, response.Value.Description);
            Assert.Equal(pokemon.Habitat, response.Value.Habitat);
            Assert.Equal(pokemon.IsLegendary, response.Value.IsLegendary);

            _pokedexServiceMock.Verify(a => a.GetPokemon(pokemon.Name, It.IsAny<CancellationToken>()), Times.Once);
            mockFunPokedexService.Verify(a => a.GetFunDescription(It.IsAny<Pokemon>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPokemonTranslated_Exception()
        {
            Pokemon pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "",
                IsLegendary = true,
            };

            _pokedexServiceMock.Setup(a => a.GetPokemon(pokemon.Name, It.IsAny<CancellationToken>())).ReturnsAsync(PokedexResult<Pokemon>.Success(pokemon));

            var mockFunPokedexService = new Mock<IFunPokedexService>();
            mockFunPokedexService.Setup(a => a.GetFunDescription(It.IsAny<Pokemon>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("internal service error"));

            PokedexController controller = new(_pokedexServiceMock.Object, mockFunPokedexService.Object, MockedLogger);

            // unexpected errors are not manage by controller
            await Assert.ThrowsAnyAsync<Exception>(() => controller.GetPokemonTranslated(pokemon.Name, _token));

            _pokedexServiceMock.Verify(a => a.GetPokemon(pokemon.Name, It.IsAny<CancellationToken>()), Times.Once);
            mockFunPokedexService.Verify(a => a.GetFunDescription(It.IsAny<Pokemon>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        private static ILogger<PokedexController> MockedLogger => new Mock<ILogger<PokedexController>>().Object;

    }
}
