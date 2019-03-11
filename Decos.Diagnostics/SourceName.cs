using System;

namespace Decos.Diagnostics
{
    public class SourceName : IEquatable<SourceName>, IComparable<SourceName>
    {
        public SourceName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name { get; }

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

        public static bool operator !=(SourceName a, SourceName b) 
            => !(a == b);

        public static bool operator ==(SourceName a, SourceName b)
        {
            if (a is null)
                return b is null;

            return a.Equals(b);
        }

        public static bool operator >(SourceName a, SourceName b)
        {
            if (a == null)
                return false;

            return a.CompareTo(b) > 0;
        }

        public static bool operator <(SourceName a, SourceName b)
        {
            if (a == null)
                return true;

            return a.CompareTo(b) < 0;
        }

        public static bool operator >=(SourceName a, SourceName b)
        {
            if (a == null)
                return b == null;

            return a.CompareTo(b) >= 0;
        }

        public static bool operator <=(SourceName a, SourceName b)
        {
            if (a == null)
                return true;

            return a.CompareTo(b) <= 0;
        }

        public int CompareTo(SourceName other)
        {
            // Everything is greater than null
            if (other == null)
                return 1;

            return StringComparer.Ordinal.Compare(Name, other.Name);
        }

        public bool Equals(SourceName other)
            => other != null && other.Name == Name;

        public override bool Equals(object obj)
            => obj is SourceName other && Equals(other);

        public override int GetHashCode()
            => Name.GetHashCode();
    }
}