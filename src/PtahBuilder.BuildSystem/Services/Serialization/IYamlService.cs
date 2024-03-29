﻿namespace PtahBuilder.BuildSystem.Services.Serialization;

public interface IYamlService
{
    T Deserialize<T>(string yaml);
    (T entity, Dictionary<string, object>? metadata) DeserializeAndGetMetadata<T>(string yaml);
    string Serialize<T>(T entity);
}