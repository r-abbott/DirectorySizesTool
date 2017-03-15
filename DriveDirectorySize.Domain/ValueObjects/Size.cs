using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DriveDirectorySize.Domain.ValueObjects
{
    public class Size
    {
        public long ByteValue { get; }
        public long Value { get; }
        public SizeType SizeType { get; }

        public Size(long value, SizeType sizeType)
        {
            if (value < 0) throw new ArgumentException("Invalid value entered. Must be non-negative.");

            Value = value;
            SizeType = sizeType;
            ByteValue = CalculateByteValue();
        }

        public override string ToString()
        {
            return $"{Value} {SizeType}";
        }

        private long CalculateByteValue()
        {
            return Value * SizeType.Value;
        }

        public static Size Bytes(long value)
        {
            return new Size(value, SizeType.Bytes);
        }

        public static Size Kilobytes(long value)
        {
            return new Size(value, SizeType.Kilobytes);
        }

        public static Size MegaBytes(long value)
        {
            return new Size(value, SizeType.Megabytes);
        }

        public static Size GigaBytes(long value)
        {
            return new Size(value, SizeType.Gigabytes);
        }

        public static Size BestFit(long bytes)
        {
            var sizeType = SizeType.GetSizeType(bytes);
            var sizeTypeValue = bytes / sizeType.Value;
            return new Size(sizeTypeValue, sizeType);
        }

        public static bool TryParse(string input, out Size size)
        {
            var parts = input.Split(' ');
            size = null;
            if(parts.Length != 2)
            {
                return false;
            }

            long inputSize;
            if(!long.TryParse(parts[0], out inputSize)){
                return false;
            }

            var sizeString = parts[1].ToLower();
            var sizeType = SizeType.GetAll().FirstOrDefault(s => s.DisplayName.Equals(sizeString));
            if(sizeType == null)
            {
                return false;
            }
            size = new Size(inputSize * sizeType.Value, sizeType);
            return true;
        }
    }
}
