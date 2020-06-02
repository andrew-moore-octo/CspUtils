using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CspUtils
{
    public class CspDirective
    {
        public CspDirective(string name, params string[] expressions) : this(name, (IEnumerable<string>) expressions)
        {
        }

        public CspDirective(string name, IEnumerable<string> expressions)
        {
            Name = name;
            Expressions = expressions.ToImmutableArray();
        }

        public string Name { get; }

        public ImmutableArray<string> Expressions { get; }
        
        public CspDirective Union(CspDirective other)
        {
            // Remove duplicates.
            var expressions = Expressions
                .Union(other.Expressions)
                .ToArray();

            var hasNone = expressions.Any(e => e == "'none'");
            if (hasNone)
            {
                return new CspDirective(
                    Name,
                    expressions
                        .Where(e => e != "'none'")
                        .ToArray()
                );
            }
            
            return new CspDirective(
                Name,
                expressions
            );
        }

        public override string ToString()
        {
            return $"{Name} {string.Join(" ", Expressions)}";
        }

        protected bool Equals(CspDirective other)
        {
            return Name == other.Name && Expressions.SequenceEqual(other.Expressions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CspDirective) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Expressions);
        }
    }
}