using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CspUtils
{
    public class Csp
    {
        public Csp(params CspDirective[] directives) : this((IEnumerable<CspDirective>)directives)
        {
        }

        public Csp(IEnumerable<CspDirective> directives)
        {
            Directives = directives.ToImmutableArray();
        }

        public ImmutableArray<CspDirective> Directives { get; }
        
        public Csp Union(Csp policy2)
        {
            // This method tries to keep the ordering of directives
            // consistent with the inputs as much as possible, which 
            // is why it's not using a dictionary for lookup.
            var directives = Directives
                .ToList();
            
            foreach (var directive in policy2.Directives)
            {
                var matchingIndex = directives.FindIndex(d => d.Name == directive.Name);
                if (matchingIndex >= 0)
                {
                    directives[matchingIndex] = directives[matchingIndex].Union(directive);
                }
                else
                {
                    directives.Add(directive);
                }
            }

            return new Csp(directives);
        }

        public override string ToString()
        {
            return $"{string.Join("; ", Directives)}";
        }

        protected bool Equals(Csp other)
        {
            return Directives.SequenceEqual(other.Directives);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Csp) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Directives);
        }
    }
}