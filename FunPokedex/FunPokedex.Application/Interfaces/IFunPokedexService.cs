using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunPokedex.Application.Models;

namespace FunPokedex.Application.Interfaces
{
    public interface IFunPokedexService
    {
        public Task<PokedexResult<string>> GetFunDescription(Pokemon pokemon, CancellationToken token);
    }
}
