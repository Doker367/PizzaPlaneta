using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ManyBox.Utils
{
    // Converts JSON values 1/0, "1"/"0", true/false, "true"/"false" into bool?
    public sealed class BoolFlexibleJsonConverter : JsonConverter<bool?>
    {
        public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.True:
                        return true;
                    case JsonTokenType.False:
                        return false;
                    case JsonTokenType.Number:
                        if (reader.TryGetInt32(out var n)) return n != 0;
                        if (reader.TryGetInt64(out var l)) return l != 0;
                        if (reader.TryGetDouble(out var d)) return Math.Abs(d) > double.Epsilon;
                        break;
                    case JsonTokenType.String:
                        var s = reader.GetString();
                        if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase)) return true;
                        if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase)) return false;
                        if (int.TryParse(s, out var n2)) return n2 != 0;
                        if (double.TryParse(s, out var d2)) return Math.Abs(d2) > double.Epsilon;
                        break;
                }
            }
            catch { }
            return null; // If cannot parse, return null rather than throw
        }

        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if (value.HasValue) writer.WriteBooleanValue(value.Value);
            else writer.WriteNullValue();
        }
    }
}
