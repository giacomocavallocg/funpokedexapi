using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;
using FunPokedex.Application.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace FunPokedex.Tests.FunPokedex.Application.Services
{
    public class FunPokedexServiceTest
    {

        private readonly Mock<IFunTranslationsApi> _translationsApiMock;
        private readonly Mock<IApplicationCache> _cacheMock;
        private readonly CancellationToken _token;

        public FunPokedexServiceTest() 
        { 
            _translationsApiMock = new Mock<IFunTranslationsApi>();
            _cacheMock = new Mock<IApplicationCache>();
            _token = new CancellationToken();
        }

        [Fact]
        public async Task GetFunDescription_OK()
        {

            _translationsApiMock.Setup(s => s.GetShakespeareTranslation(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                new FunTranslationDto() { Translation = "shakspeare", OriginalText = "original", Origin = "shakspeare" });

            _translationsApiMock.Setup(s => s.GetYodaTranslation(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                new FunTranslationDto() { Translation = "yoda", OriginalText = "original", Origin = "yoda" });


            FunPokedexService service = new(_translationsApiMock.Object, _cacheMock.Object);

            // cave
            Pokemon pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "cave",
                IsLegendary = false,
            };

            var result = await service.GetFunDescription(pokemon, _token);

            Assert.True(result.IsSuccess);
            Assert.Equal("yoda", result.Value);

            // legendary
            pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "ocean",
                IsLegendary = true,
            };

            result = await service.GetFunDescription(pokemon, _token);

            Assert.True(result.IsSuccess);
            Assert.Equal("yoda", result.Value);

            // other
            pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "ocean",
                IsLegendary = false,
            };

            result = await service.GetFunDescription(pokemon, _token);

            Assert.True(result.IsSuccess);
            Assert.Equal("shakspeare", result.Value);
        }

        [Fact]
        public async Task GetFunDescription_OKFromCache()
        {

            string? desc = "from cache";
            _cacheMock.Setup(a => a.TryGetValue(It.IsAny<string>(), out desc)).Returns(true);


            FunPokedexService service = new(_translationsApiMock.Object, _cacheMock.Object);

            Pokemon pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "cave",
                IsLegendary = false,
            };

            var result = await service.GetFunDescription(pokemon, _token);

            Assert.True(result.IsSuccess);
            Assert.Equal(desc, result.Value);

            _translationsApiMock.Verify(t => t.GetYodaTranslation(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            _translationsApiMock.Verify(t => t.GetShakespeareTranslation(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task GetFunDescription_OKNullTranslation()
        {

            _translationsApiMock.Setup(s => s.GetShakespeareTranslation(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);
            _translationsApiMock.Setup(s => s.GetYodaTranslation(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);


            FunPokedexService service = new(_translationsApiMock.Object, _cacheMock.Object);

            // yoda
            Pokemon pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "cave",
                IsLegendary = false,
            };

            var result = await service.GetFunDescription(pokemon, _token);

            Assert.True(result.IsSuccess);
            Assert.Equal(pokemon.Description, result.Value);

            // shakspeare
            pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "ocean",
                IsLegendary = false,
            };

            result = await service.GetFunDescription(pokemon, _token);

            Assert.True(result.IsSuccess);
            Assert.Equal(pokemon.Description, result.Value);
        }

        [Fact]
        public async Task GetFunDescription_TranslationException()
        {

            _translationsApiMock.Setup(s => s.GetShakespeareTranslation(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            _translationsApiMock.Setup(s => s.GetYodaTranslation(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());


            FunPokedexService service = new(_translationsApiMock.Object, _cacheMock.Object);

            // yoda
            Pokemon pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "cave",
                IsLegendary = false,
            };

            var result = await service.GetFunDescription(pokemon, _token);
            Assert.True(result.IsSuccess);
            Assert.Equal(pokemon.Description, result.GetValueOrThrow());

            // shakspeare
            pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "ocean",
                IsLegendary = false,
            };

            result = await service.GetFunDescription(pokemon, _token);
            Assert.True(result.IsSuccess);
            Assert.Equal(pokemon.Description, result.GetValueOrThrow());
        }

        [Fact]
        public async Task GetFunDescription_CacheException()
        {
            string? desc = "from cache";
            _cacheMock.Setup(a => a.TryGetValue(It.IsAny<string>(), out desc)).Throws(new Exception());

            FunPokedexService service = new(_translationsApiMock.Object, _cacheMock.Object);

            Pokemon pokemon = new()
            {
                Name = "wormadam",
                Description = "When BURMY evolved, its cloak\nbecame a part of this Pokémon’s\nbody. The cloak is never shed.",
                Habitat = "cave",
                IsLegendary = false,
            };

            await Assert.ThrowsAnyAsync<Exception>(() => service.GetFunDescription(pokemon, _token));
        }

    }
}
