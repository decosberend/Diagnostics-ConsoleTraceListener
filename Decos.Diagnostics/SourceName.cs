using System;
using System.Collections.Generic;
using System.Linq;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents the name of a logging source.
    /// </summary>
    public class SourceName : IEquatable<SourceName>, IComparable<SourceName>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceName"/> class with
        /// the specified name.
        /// </summary>
        /// <param name="name">The name of the logging source.</param>
        public SourceName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Parts = name.Split('.');
        }

        /// <summary>
        /// Gets the name of the logging source.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the parent source, or <c>null</c> if this name
        /// specifies a top-level source.
        /// </summary>
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

        /// <summary>
        /// Creates a new <see cref="SourceName"/> that represents the specified
        /// type.
        /// </summary>
        /// <param name="type">The type of logging source.</param>
        /// <returns>
        /// A new instance of the <see cref="SourceName"/> class for the
        /// specified type.
        /// </returns>
        public static SourceName FromType(Type type)
            => new SourceName(type.Namespace);

        /// <summary>
        /// Creates a new <see cref="SourceName"/> that represents the specified
        /// type.
        /// </summary>
        /// <typeparam name="T">The type of logging source.</typeparam>
        /// <returns>
        /// A new instance of the <see cref="SourceName"/> class for the
        /// specified type.
        /// </returns>
        public static SourceName FromType<T>()
            => FromType(typeof(T));

        /// <summary>
        /// Returns a new <see cref="SourceName"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the logging source.</param>
        public static implicit operator SourceName(string name)
        {
            if (name == null)
                return null;

            return new SourceName(name);
        }

        /// <summary>
        /// Returns the name of the logging source.
        /// </summary>
        /// <param name="sourceName">The name of the logging source.</param>
        public static implicit operator string(SourceName sourceName)
            => sourceName?.Name ?? null;

        /// <summary>
        /// Determines whether two source names are the same.
        /// </summary>
        /// <param name="a">The first source name to compare.</param>
        /// <param name="b">The second source name to compare with.</param>
        /// <returns>
        /// <c>true</c> if both names represent the same source; otherwise,
        /// <c>false</c>.
        /// </returns>
        public static bool operator ==(SourceName a, SourceName b)
        {
            if (a is null)
                return b is null;

            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether two source names are different.
        /// </summary>
        /// <param name="a">The first source name to compare.</param>
        /// <param name="b">The second source name to compare with.</param>
        /// <returns>
        /// <c>true</c> if both names represent a different source; otherwise,
        /// <c>false</c>.
        /// </returns>
        public static bool operator !=(SourceName a, SourceName b)
            => !(a == b);

        /// <summary>
        /// Determines whether one source name is less than another source name.
        /// </summary>
        /// <param name="left">The first source name to compare.</param>
        /// <param name="right">The second source name to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> is less than <paramref
        /// name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(SourceName left, SourceName right)
        {
            if (left == null)
                return true;

            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Determines whether one source name is less than or equal to source
        /// name.
        /// </summary>
        /// <param name="left">The first source name to compare.</param>
        /// <param name="right">The second source name to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> is less than or equal to
        /// <paramref name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=(SourceName left, SourceName right)
        {
            if (left == null)
                return true;

            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Determines whether one source name is greater than another source
        /// name.
        /// </summary>
        /// <param name="left">The first source name to compare.</param>
        /// <param name="right">The second source name to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> is greater than or equal to
        /// <paramref name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(SourceName left, SourceName right)
        {
            if (left == null)
                return false;

            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Determines whether one source name is greater than or equal to
        /// another source name.
        /// </summary>
        /// <param name="left">The first source name to compare.</param>
        /// <param name="right">The second source name to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> is greater than or equal to
        /// <paramref name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=(SourceName left, SourceName right)
        {
            if (left == null)
                return right == null;

            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Compares the source name with another source name and returns an
        /// integer that indicates whether the source name precedes, follows, or
        /// occurs in the same position in the sort order as the other source
        /// name.
        /// </summary>
        /// <param name="other">
        /// A source name to compare with this source name.
        /// </param>
        /// <returns>
        /// A value that indicates the relative order of the source names being
        /// compared. If the return value is less than zero, this name precedes
        /// <paramref name="other"/> in the sort order. If the return value is
        /// zero, this name occurs in the same position in the sort order as
        /// <paramref name="other"/> and if the return value is greater than
        /// zero, this name follows <paramref name="other"/> in the sort order.
        /// </returns>
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

        /// <summary>
        /// Determines whether the source name is equal to another name.
        /// </summary>
        /// <param name="other">A name to compare with this source name.</param>
        /// <returns>
        /// <c>true</c> if this source name is equal to <paramref name="other"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(SourceName other)
            => !(other is null) && other.Name == Name;

        /// <summary>
        /// Determines whether the source name is equal to another object.
        /// </summary>
        /// <param name="obj">An object to compare with this source name.</param>
        /// <returns>
        /// <c>true</c> if this source name is equal to <paramref name="obj"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
            => obj is SourceName other && Equals(other);

        /// <summary>
        /// Returns the hash code for this name.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
            => Name.GetHashCode();

        /// <summary>
        /// Returns a string representation of the source name.
        /// </summary>
        /// <returns>A string that represents this source name.</returns>
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