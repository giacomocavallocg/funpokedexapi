using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunPokedex.Application.Exceptions
{
    public class PokedexApplicationException: Exception
    {
        public string Code { get; }

        public PokedexApplicationException(string code, string message): base(message)
        {
            Code = code;
        }

        public PokedexApplicationException(string code, string message, Exception inner) : base(message, inner)
        {
            Code = code;
        }

        public static PokedexApplicationException UnexpectedResponse(string path, string statusCode)
        {
            return new PokedexApplicationException($"Unexpected response code {statusCode} from path {path}", "001");
        }

        public static PokedexApplicationException InvalidResponseDeserialization(string message)
        {
            return new PokedexApplicationException(message, "002");
        }

        public static PokedexApplicationException InvalidResponseDeserialization(string message, Exception inner)
        {
            return new PokedexApplicationException(message, "002", inner);
        }
    }
}
