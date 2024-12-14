using FunPokedex.Application.Models;

namespace FunPokedex.Application.Interfaces
{
    public interface IFunTranslationsApi
    {
        internal Task<FunTranslationDto?> GetShakespeareTranslation(string text);

        internal Task<FunTranslationDto?> GetYodaTranslation(string text);
    }
}
