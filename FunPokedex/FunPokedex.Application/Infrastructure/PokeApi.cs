
using System;
using System.IO;
using FunPokedex.Application.Exceptions;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;
using FunPokedex.Application.Models.Converters;
using Newtonsoft.Json;

namespace FunPokedex.Application.Infrastructure
{
    public class PokeApi(HttpClient client) : IPokeApi
    {
        private readonly HttpClient _httpClient = client;

        async Task<PokemonSpecieDto?> IPokeApi.GetPokemonSpecies(string name, CancellationToken token)
        {
            string path = String.Format("api/v2/pokemon-species/{0}", name);
            
            HttpResponseMessage response = await _httpClient.GetAsync(path, token);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync(token);
                return ReadResponse(content);
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            
            throw PokedexApplicationException.UnexpectedResponse(path, response.StatusCode.ToString());


        }

        private static  PokemonSpecieDto ReadResponse(string content)
        {

            if (string.IsNullOrEmpty(content)) throw PokedexApplicationException.InvalidResponseDeserialization("response content is null");

            try
            {
                JsonSerializerSettings settings = new()
                {
                    Converters = { new PokemonSpecieDtoConverter() }
                };
                return JsonConvert.DeserializeObject<PokemonSpecieDto>(content, settings) ?? throw PokedexApplicationException.InvalidResponseDeserialization("response content is null");
            }
            catch (Exception e) when (e is JsonReaderException || e is JsonSerializationException)
            {
                throw PokedexApplicationException.InvalidResponseDeserialization(e.Message, e);
            }
        }

    }

}
