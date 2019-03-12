using System;
using System.Collections.Generic;
using System.Linq;

namespace Decos.Diagnostics
{
    public class SourceName : IEquatable<SourceName>, IComparable<SourceName>
    {
        public SourceName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Parts = name.Split('.');
        }

        public string Name { get; }

        public SourceName Parent
        {
            get
            {
                if (Parts.Count < 2)
                    return null;

                return new SourceName(string.Join(".", Parts.Take(Parts.Count - 1)));
            }
        }

        private IReadOnlyList<string> Parts { get; }

        public static SourceName FromType(Type type)
            => new SourceName(type.Namespace);

        public static SourceName FromType<T>()
            => FromType(typeof(T));

        public static implicit operator SourceName(string name)
        {
            if (name == null)
                return null;

            return new SourceName(name);
        }

        public static implicit operator string(SourceName sourceName)
            => sourceName?.Name ?? null;

        public static bool operator ==(SourceName a, SourceName b)
        {
            if (a is null)
                return b is null;

            return a.Equals(b);
        }

        public static bool operator !=(SourceName a, SourceName b)
            => !(a == b);

        public static bool operator <(SourceName a, SourceName b)
        {
            if (a == null)
                return true;

            return a.CompareTo(b) < 0;
        }

        public static bool operator <=(SourceName a, SourceName b)
        {
            if (a == null)
                return true;

            return a.CompareTo(b) <= 0;
        }

        public static bool operator >(SourceName a, SourceName b)
        {
            if (a == null)
                return false;

            return a.CompareTo(b) > 0;
        }

        public static bool operator >=(SourceName a, SourceName b)
        {
            if (a == null)
                return b == null;

            return a.CompareTo(b) >= 0;
        }

        public int CompareTo(SourceName other)
        {
            // Everything is greater than null
            if (other == null)
                return 1;

            if (other == this)
                return 0;

            // parents are less specific and should be sorted lower
            if (IsDescendantOf(other))
                return -1;

            // children are more specific and should be sorted higher
            if (other.IsDescendantOf(this))
                return 1;

            return StringComparer.Ordinal.Compare(Name, other.Name);
        }

        public bool Equals(SourceName other)
            => !(other is null) && other.Name == Name;

        public override bool Equals(object obj)
            => obj is SourceName other && Equals(other);

        public override int GetHashCode()
            => Name.GetHashCode();

        public override string ToString()
            => Name;

        /// <summary>
        /// Determines whether a source with this name would be included in the
        /// specified filter.
        /// </summary>
        /// <param name="filter">The source name filter.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="filter"/> matches this source name;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool Matches(SourceName filter)
        {
            return this == filter || IsDescendantOf(filter);
        }

        private bool IsDescendantOf(SourceName ancestor)
        {
            if (ancestor == null)
                return false;

            var parent = Parent;
            while (parent != null)
            {
                if (parent == ancestor)
                    return true;

                parent = parent.Parent;
            }

            return false;
        }
    }
}