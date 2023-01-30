using System.Text.Json;
using System.Text.Json.Serialization;
using PtahBuilder.Tests.TestBuilder.Entities.Dice;

namespace PtahBuilder.Tests.TestBuilder.Converters;

public class DiceEquationJsonConverter : JsonConverter<IDiceValue>
{
    public override IDiceValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => DiceParser.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, IDiceValue value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}