using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunPokedex.Application.Interfaces
{
    public interface IApplicationCache
    {
        public bool TryGetValue<T>(string key, out T? value);

        public void SetValue<T>(string key, T value);

    }
}
