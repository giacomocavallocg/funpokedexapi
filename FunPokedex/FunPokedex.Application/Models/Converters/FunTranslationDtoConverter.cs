using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FunPokedex.Application.Models.Converters
{

    internal class FunTranslationDtoConverter : JsonConverter<FunTranslationDto>
    {

        public override FunTranslationDto? ReadJson(JsonReader reader, Type objectType, FunTranslationDto? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            JToken? content = jsonObject["contents"];

            if(content == null || content is not JObject contentObj) throw new JsonReaderException("required field content is not found");

            string translated = contentObj["translated"]?.Value<string>() ?? throw new JsonReaderException("required field content.translated is not found");
            string text = contentObj["text"]?.Value<string>() ?? throw new JsonReaderException("required field content.text is not found");
            string translatrion = contentObj["translation"]?.Value<string>() ?? throw new JsonReaderException("required field content.translation is not found");

            return new FunTranslationDto()
            {
                OriginalText = text,
                Translation = translated,
                Origin = translatrion,

            };
        }

        public override void WriteJson(JsonWriter writer, FunTranslationDto? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
