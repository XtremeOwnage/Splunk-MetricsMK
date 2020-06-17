using System;
using System.Diagnostics;

namespace InputsBuilder.Models
{
    /// <summary>
    /// Represents a class which is used like an enum.
    /// To use, inherit from this class, and add records like so:
    /// public static InheritsFromStringEnum Percent { get { return new InheritsFromStringEnum("pct"); } }
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class StringEnum : IEquatable<string>
    {
        protected static StringEnum New(string Value) => new StringEnum(Value);

        protected StringEnum(string value) { Value = value; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(string other)
        {
            return Value.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Implicit operator to allow StringEnum to be utitlized as a string directly, without the need to call .ToString();
        /// </summary>
        /// <param name="Value"></param>
        public static implicit operator string(StringEnum Value) => Value.ToString();
    }
}
