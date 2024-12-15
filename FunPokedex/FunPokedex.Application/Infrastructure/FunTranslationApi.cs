
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using FunPokedex.Application.Exceptions;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Models;
using FunPokedex.Application.Models.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FunPokedex.Application.Infrastructure
{
    public class FunTranslationApi(HttpClient client) : IFunTranslationsApi
    {
        private readonly HttpClient _httpClient = client;


        Task<FunTranslationDto?> IFunTranslationsApi.GetShakespeareTranslation(string text, CancellationToken token)
        {
            string path = String.Format("translate/{0}?text={1}", "shakespeare.json", text.Replace(Environment.NewLine, ""));
            return ExecuteRequest(path, token);
        }

        Task<FunTranslationDto?> IFunTranslationsApi.GetYodaTranslation(string text, CancellationToken token)
        {
            string path = String.Format("translate/{0}?text={1}", "yoda.json", text.Replace(Environment.NewLine, ""));
            return ExecuteRequest(path, token);
        }

        private async Task<FunTranslationDto?> ExecuteRequest(string path, CancellationToken token)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(path, token);

            if (!response.IsSuccessStatusCode) throw PokedexApplicationException.UnexpectedResponse(path, response.StatusCode.ToString());

            string content = await response.Content.ReadAsStringAsync(token);
            return ReadResponse(content);
        }


        private static FunTranslationDto? ReadResponse(string content)
        {
            if (string.IsNullOrEmpty(content)) throw PokedexApplicationException.InvalidResponseDeserialization("response content is null");

            try
            {
                JsonSerializerSettings settings = new()
                {
                    Converters = { new FunTranslationDtoConverter() }
                };
                return JsonConvert.DeserializeObject<FunTranslationDto>(content, settings) ?? throw PokedexApplicationException.InvalidResponseDeserialization("response content is null");
            }
            catch (Exception e) when (e is JsonReaderException || e is JsonSerializationException)
            {
                throw PokedexApplicationException.InvalidResponseDeserialization(e.Message, e);
            }
        }
    }

}
