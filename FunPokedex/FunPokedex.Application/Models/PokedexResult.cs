using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunPokedex.Application.Models
{
    public class PokedexResult<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? Error { get; }
        public FailureType? FailureType { get; }

        private PokedexResult(bool isSuccess, T? value, string? error, FailureType? failureType = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            FailureType = failureType;
        }

        internal static PokedexResult<T> Success(T value) => new(true, value, null);
        internal static PokedexResult<T> Fail(string error, FailureType failureType) => new(false, default, error, failureType);
    }

}
