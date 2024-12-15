using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace FunPokedex.Application.Models.Converters
{
    internal class PokemonSpecieDtoConverter : JsonConverter<PokemonSpecieDto>
    {

        public override PokemonSpecieDto? ReadJson(JsonReader reader, Type objectType, PokemonSpecieDto? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            int id = jsonObject["id"]?.Value<int>() ?? throw new JsonReaderException("required field id is not found");
            string name = jsonObject["name"]?.Value<string>() ?? throw new JsonReaderException("required field name is not found");
            bool isLegendary = jsonObject["is_legendary"]?.Value<bool>() ?? throw new JsonReaderException("required field is_legendary is not found");
            bool isMithical = jsonObject["is_mythical"]?.Value<bool>() ?? throw new JsonReaderException("required field is_mythical is not found");

            List<PokemonSpecieTextDto> texts = GetTexts(jsonObject["flavor_text_entries"]);
            string? habitat = GetHabitat(jsonObject["habitat"]);

            return new PokemonSpecieDto()
            {
                Id = id,
                Name = name,
                Habitat = habitat,
                IsLegendary = isLegendary,
                IsMythical = isMithical,
                FlavorTexts = texts,

            };
        }

        public override void WriteJson(JsonWriter writer, PokemonSpecieDto? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static string? GetHabitat(JToken? habitatToken)
        {
            if (habitatToken == null) return null;

            if (habitatToken is JObject habitatObject)
            {
                return habitatObject["name"]?.Value<string>() ?? throw new JsonReaderException("required field habitat.name is not found");
            }

            throw new JsonReaderException("invalid format for filed habitat");
        }

        private static List<PokemonSpecieTextDto> GetTexts(JToken? textsToken)
        {
            if (textsToken == null) return [];

            if (textsToken is JArray textsArray)
            {
                List<PokemonSpecieTextDto> texts = [];

                return textsArray.Select(eTok =>
                {
                    if (eTok is JObject eObj)
                    {
                        string text = eObj["flavor_text"]?.Value<string>() ?? throw new JsonReaderException("required field flavor_text_entries.flavor_text is not found");
                        text = Regex.Replace(text, @"\t|\n|\r|\f", " ");
                        if (eObj["language"] is JObject langObj)
                        {
                            string lang = langObj["name"]?.Value<string>() ?? throw new JsonReaderException("required field flavor_text_entries.language.name is not found");

                            return new PokemonSpecieTextDto()
                            {
                                FlavorText = text,
                                Language = lang,
                            };
                        }

                        throw new JsonReaderException("invalid format for filed language");
                    }

                    throw new JsonReaderException("invalid format for filed flavor_text");

                }).ToList();
            }

            throw new JsonReaderException("invalid format for filed flavor_text_entries");
        }
    }
}
