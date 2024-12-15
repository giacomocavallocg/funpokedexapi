using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FunPokedex.Application.Exceptions;
using FunPokedex.Application.Infrastructure;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;
using RichardSzalay.MockHttp;
using static System.Net.Mime.MediaTypeNames;

namespace FunPokedex.Tests.FunPokedex.Application.Infrastructure
{
    public class FunTranslationApiTest
    {
        [Fact]
        public async Task Test_OK()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When("http://localhost/translate/shakespeare.json?text=text").Respond("application/json", shakespeareResponse);
            mockHttp.When("http://localhost/translate/yoda.json?text=text").Respond("application/json", yodaResponse);

            IFunTranslationsApi pokeApi = new FunTranslationApi(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") });

            FunTranslationDto? sResponse = await pokeApi.GetShakespeareTranslation("text", new CancellationToken());
            Assert.NotNull(sResponse);
            Assert.Equal("shakespere text", sResponse.Translation);
            Assert.Equal("text", sResponse.OriginalText);
            Assert.Equal("shakespeare", sResponse.Origin);


            FunTranslationDto? yResponse = await pokeApi.GetYodaTranslation("text", new CancellationToken());
            Assert.NotNull(yResponse);
            Assert.Equal("yoda text", yResponse.Translation);
            Assert.Equal("text", yResponse.OriginalText);
            Assert.Equal("yoda", yResponse.Origin);
        }

        [Fact]
        public async Task Test_JsonError()
        {

            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When("http://localhost/translate/shakespeare.json?text=text").Respond("application/json", "{\"invalid_json\":true}");
            mockHttp.When("http://localhost/translate/yoda.json?text=text").Respond("application/json", "{\"invalid_json\":true}");

            IFunTranslationsApi pokeApi = new FunTranslationApi(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") });


            await Assert.ThrowsAnyAsync<PokedexApplicationException>(() => pokeApi.GetShakespeareTranslation("text", new CancellationToken()));
            await Assert.ThrowsAnyAsync<PokedexApplicationException>(() => pokeApi.GetYodaTranslation("text", new CancellationToken()));

        }

        [Fact]
        public async Task Test_Not200Status()
        { 
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When("http://localhost/translate/shakespeare.json?text=text").Respond(HttpStatusCode.NotFound);
            mockHttp.When("http://localhost/translate/yoda.json?text=text").Respond(HttpStatusCode.NotFound);

            IFunTranslationsApi pokeApi = new FunTranslationApi(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") });


            await Assert.ThrowsAnyAsync<PokedexApplicationException>(() => pokeApi.GetShakespeareTranslation("text", new CancellationToken()));
            await Assert.ThrowsAnyAsync<PokedexApplicationException>(() => pokeApi.GetYodaTranslation("text", new CancellationToken()));
        }

        private const string shakespeareResponse = "{\"success\":{\"total\":1},\"contents\":{\"translated\":\"shakespere text\",\"text\":\"text\",\"translation\":\"shakespeare\"}}";

        private const string yodaResponse = "{\"success\":{\"total\":1},\"contents\":{\"translated\":\"yoda text\",\"text\":\"text\",\"translation\":\"yoda\"}}";
    }

}
