using System.Text.Json;
using System.Text.Json.Serialization;
using PtahBuilder.Tests.TestBuilder.Entities.Conditions;

namespace PtahBuilder.Tests.TestBuilder.Converters;

public class ConditionJsonConverter : JsonConverter<ICondition>
{
    public override ICondition? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => ConditionParser.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, ICondition value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}