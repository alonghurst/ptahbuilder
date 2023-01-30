using System.Text.Json;
using System.Text.Json.Serialization;
using PtahBuilder.Tests.TestBuilder.Entities.Dice;

namespace PtahBuilder.Tests.TestBuilder.Converters;

public class DiceJsonConverter : JsonConverter<Dice>
{
    public override Dice Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Dice.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, Dice value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}