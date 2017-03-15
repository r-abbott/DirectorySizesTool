using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DriveDirectorySize.Domain.ValueObjects
{
    public sealed class SizeType : IComparable
    {
        public static readonly SizeType Bytes = new SizeType("b", 1);
        public static readonly SizeType Kilobytes = new SizeType("kb", 1000);
        public static readonly SizeType Megabytes = new SizeType("mb", 1000000);
        public static readonly SizeType Gigabytes = new SizeType("gb", 1000000000);

        public string DisplayName { get; }
        public long Value { get; }

        public static IEnumerable<SizeType> GetAll()
        {
            var type = typeof(SizeType);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = new SizeType();
                var locatedValue = info.GetValue(instance) as SizeType;

                if (locatedValue != null)
                {
                    yield return locatedValue;
                }
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as SizeType;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(object other)
        {
            return Value.CompareTo(((SizeType)other).Value);
        }

        public static SizeType GetSizeType(long bytes)
        {
            if (bytes == 0)
            {
                return SizeType.Bytes;
            }
            var ordered = GetAll().OrderByDescending(s => s.Value);
            return ordered.First(s => s.Value <= bytes);
        }

        private SizeType() { }
        public SizeType(string displayName, long value)
        {
            DisplayName = displayName;
            Value = value;
        }
    }
}
