using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv.WindowNames
{
    public record ActiveWindowInfo
    {
        public bool IsInvalid => Program == string.Empty && Details == null;
        public string Program { get; private set; } = "";
        public string? Details { get; private set; } = null;
        public static ActiveWindowInfo Invalid => new();
        private ActiveWindowInfo() { }
        public ActiveWindowInfo(string program, string? details = null, bool alias = false)
        {
            Program = alias ? Alias.BestReplacement(program) : program;
            Details = details;
        }
        public ActiveWindowInfo((string program, string? details)? tuple, bool alias = true)
        {
            if (tuple is null) return;
            (string program, string? details) = tuple.Value;
            Program = alias ? Alias.BestReplacement(program) : program;
            Details = details;
        }
        public override string ToString()
        {
            if (Details is not null)
            {
                return $"{Program,-30}\t{Details}";
            }
            return Program;
        }
        public static implicit operator ActiveWindowInfo((string program, string? details)? tuple) => new(tuple, true);
    }
}
