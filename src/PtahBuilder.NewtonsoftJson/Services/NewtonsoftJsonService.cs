﻿using Newtonsoft.Json;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.NewtonsoftJson.Config.Internal;

namespace PtahBuilder.NewtonsoftJson.Services
{
    public class NewtonsoftJsonService : IJsonService
    {
        public JsonSerializerSettings Settings { get; }

        public NewtonsoftJsonService(NewtonsoftJsonConverterConfig config)
        {
            Settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            foreach (var converter in config.Converters)
            {
                Settings.Converters.Add(converter);
            }
        }

        public T Deserialize<T>(string text) => JsonConvert.DeserializeObject<T>(text, Settings) ?? throw new InvalidOperationException("Unable to deserialize text");

        public string Serialize<T>(T entity) => JsonConvert.SerializeObject(entity, Settings);
    }
}
