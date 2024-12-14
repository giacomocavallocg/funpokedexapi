using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunPokedex.Application.Models
{
    public class Pokemon
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Habitat { get; set; }
        public required bool IsLegendary { get; set; }
    }
}
