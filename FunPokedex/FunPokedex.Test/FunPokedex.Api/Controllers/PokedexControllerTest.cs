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

            var mockPokedexService = new Mock<IPokedexService>();
            mockPokedexService.Setup(a => a.GetPokemon(pokemon.Name)).ReturnsAsync(PokedexResult<Pokemon>.Success(pokemon));

            PokedexController controller = new(mockPokedexService.Object, new Mock<IFunPokedexService>().Object, MockedLogger);

            var response = await controller.GetPokemon(pokemon.Name);
            Assert.NotNull(response.Value);
            Assert.IsType<PokemonDto>(response.Value);
            Assert.Equal(pokemon.Name, response.Value.Name);
            Assert.Equal(pokemon.Description, response.Value.Description);
            Assert.Equal(pokemon.Habitat, response.Value.Habitat);
            Assert.Equal(pokemon.IsLegendary, response.Value.IsLegendary);

            mockPokedexService.Verify(a => a.GetPokemon(pokemon.Name), Times.Once);
        }

        [Fact]
        public async Task GetPokemon_NotFound()
        {
            string pokemonName = "not-exists";

            var mockPokedexService = new Mock<IPokedexService>();
            mockPokedexService.Setup(a => a.GetPokemon(pokemonName)).ReturnsAsync(PokedexResult<Pokemon>.Fail("not found", FailureType.NotFound));

            PokedexController controller = new (mockPokedexService.Object, new Mock<IFunPokedexService>().Object, MockedLogger);
            var response = await controller.GetPokemon(pokemonName);
            Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.IsType<ErrorDto>(((NotFoundObjectResult)response.Result).Value);

            var dto = (ErrorDto) ((NotFoundObjectResult)response.Result).Value!;
            Assert.Equal("100", dto.Code);

            mockPokedexService.Verify(a => a.GetPokemon(pokemonName), Times.Once);

        }

        [Fact]
        public async Task GetPokemon_Exception()
        {
            string pokemonName = "exception";

            var mockPokedexService = new Mock<IPokedexService>();
            mockPokedexService.Setup(a => a.GetPokemon(pokemonName)).ThrowsAsync(new Exception("generic exception"));

            PokedexController controller = new(mockPokedexService.Object, new Mock<IFunPokedexService>().Object, MockedLogger);

            // unexpected errors are not manage by controller
            await Assert.ThrowsAnyAsync<Exception>(() => controller.GetPokemon(pokemonName));
            mockPokedexService.Verify(a => a.GetPokemon(pokemonName), Times.Once);
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

            var mockPokedexService = new Mock<IPokedexService>();
            mockPokedexService.Setup(a => a.GetPokemon(pokemon.Name)).ReturnsAsync(PokedexResult<Pokemon>.Success(pokemon));

            var mockFunPokedexService = new Mock<IFunPokedexService>();
            mockFunPokedexService.Setup(a => a.GetFunDescription(It.IsAny<Pokemon>())).ReturnsAsync(PokedexResult<string>.Success(funDescription));

            PokedexController controller = new(mockPokedexService.Object, mockFunPokedexService.Object, MockedLogger);

            var response = await controller.GetPokemonTranslated(pokemon.Name);
            Assert.NotNull(response.Value);
            Assert.IsType<PokemonDto>(response.Value);

            Assert.Equal(pokemon.Name, response.Value.Name);
            Assert.Equal(funDescription, response.Value.Description);
            Assert.Equal(pokemon.Habitat, response.Value.Habitat);
            Assert.Equal(pokemon.IsLegendary, response.Value.IsLegendary);

            mockPokedexService.Verify(a => a.GetPokemon(pokemon.Name), Times.Once);
            mockFunPokedexService.Verify(a => a.GetFunDescription(It.IsAny<Pokemon>()), Times.Once);
        }

        [Fact]
        public async Task GetPokemonTranslated_PockemonNotFound()
        {
            string pokemonName = "not-exists";
            var mockPokedexService = new Mock<IPokedexService>();
            mockPokedexService.Setup(a => a.GetPokemon(pokemonName)).ReturnsAsync(PokedexResult<Pokemon>.Fail("not found", FailureType.NotFound));

            var mockFunPokedexService = new Mock<IFunPokedexService>();

            PokedexController controller = new(mockPokedexService.Object, mockFunPokedexService.Object, MockedLogger);
            var response = await controller.GetPokemonTranslated(pokemonName);

            Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.IsType<ErrorDto>(((NotFoundObjectResult)response.Result).Value);

            var dto = (ErrorDto)((NotFoundObjectResult)response.Result).Value!;
            Assert.Equal("100", dto.Code);

            mockPokedexService.Verify(a => a.GetPokemon(pokemonName), Times.Once);
            mockFunPokedexService.Verify(a => a.GetFunDescription(It.IsAny<Pokemon>()), Times.Never);
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

            var mockPokedexService = new Mock<IPokedexService>();
            mockPokedexService.Setup(a => a.GetPokemon(pokemon.Name)).ReturnsAsync(PokedexResult<Pokemon>.Success(pokemon));

            var mockFunPokedexService = new Mock<IFunPokedexService>();
            mockFunPokedexService.Setup(a => a.GetFunDescription(It.IsAny<Pokemon>())).ReturnsAsync(PokedexResult<string>.Fail("not found", FailureType.NotFound));

            PokedexController controller = new(mockPokedexService.Object, mockFunPokedexService.Object, MockedLogger);

            var response = await controller.GetPokemonTranslated(pokemon.Name);
            Assert.NotNull(response.Value);
            Assert.IsType<PokemonDto>(response.Value);

            Assert.Equal(pokemon.Name, response.Value.Name);
            Assert.Equal(pokemon.Description, response.Value.Description);
            Assert.Equal(pokemon.Habitat, response.Value.Habitat);
            Assert.Equal(pokemon.IsLegendary, response.Value.IsLegendary);

            mockPokedexService.Verify(a => a.GetPokemon(pokemon.Name), Times.Once);
            mockFunPokedexService.Verify(a => a.GetFunDescription(It.IsAny<Pokemon>()), Times.Once);
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

            var mockPokedexService = new Mock<IPokedexService>();

            mockPokedexService.Setup(a => a.GetPokemon(pokemon.Name)).ReturnsAsync(PokedexResult<Pokemon>.Success(pokemon));

            var mockFunPokedexService = new Mock<IFunPokedexService>();
            mockFunPokedexService.Setup(a => a.GetFunDescription(It.IsAny<Pokemon>())).ThrowsAsync(new Exception("internal service error"));

            PokedexController controller = new(mockPokedexService.Object, mockFunPokedexService.Object, MockedLogger);

            // unexpected errors are not manage by controller
            await Assert.ThrowsAnyAsync<Exception>(() => controller.GetPokemonTranslated(pokemon.Name));

            mockPokedexService.Verify(a => a.GetPokemon(pokemon.Name), Times.Once);
            mockFunPokedexService.Verify(a => a.GetFunDescription(It.IsAny<Pokemon>()), Times.Once);
        }


        private static ILogger<PokedexController> MockedLogger => new Mock<ILogger<PokedexController>>().Object;

    }
}
