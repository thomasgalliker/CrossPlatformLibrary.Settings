using System;
using System.Diagnostics;

namespace CrossPlatformLibrary.Settings
{
    [DebuggerDisplay("SerializableDateTime: Ticks={Ticks}, Kind={Kind}")]
    [Obsolete]
    public struct SerializableDateTime
    {
        // ReSharper disable once MemberCanBePrivate.Global because it's used for serialization
        public long Ticks { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global because it's used for serialization
        public DateTimeKind Kind { get; set; }

        public static SerializableDateTime Undefined = new SerializableDateTime { Ticks = -1, Kind = DateTimeKind.Unspecified };

        public static SerializableDateTime FromDateTime(DateTime dateTime)
        {
            return new SerializableDateTime { Ticks = dateTime.Ticks, Kind = dateTime.Kind };
        }

        public override bool Equals(object obj)
        {
            return obj is SerializableDateTime && this == (SerializableDateTime)obj;
        }

        public override int GetHashCode()
        {
            return this.Ticks.GetHashCode() ^ this.Kind.GetHashCode();
        }

        public static bool operator ==(SerializableDateTime x, SerializableDateTime y)
        {
            return x.Ticks == y.Ticks && x.Kind == y.Kind;
        }

        public static bool operator !=(SerializableDateTime x, SerializableDateTime y)
        {
            return !(x == y);
        }
    }
}