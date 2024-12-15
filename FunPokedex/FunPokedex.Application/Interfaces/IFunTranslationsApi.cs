using FunPokedex.Application.Models;

namespace FunPokedex.Application.Interfaces
{
    public interface IFunTranslationsApi
    {
        internal Task<FunTranslationDto?> GetShakespeareTranslation(string text, CancellationToken token);

        internal Task<FunTranslationDto?> GetYodaTranslation(string text, CancellationToken token);
    }
}
