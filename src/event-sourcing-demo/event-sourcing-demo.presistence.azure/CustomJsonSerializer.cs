using event_sourcing_demo.cart.core.Utility;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.IO;

namespace event_sourcing_demo.presistence.azure
{
    public class CustomJsonSerializer : CosmosSerializer
    {
        private static readonly Newtonsoft.Json.JsonSerializer Serializer = JsonSerializer.Create(new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateSetterContractResolver()
        });

        public override T FromStream<T>(Stream stream)
        {
            using var sr = new StreamReader(stream);
            using var reader = new JsonTextReader(sr);

            return Serializer.Deserialize<T>(reader);
        }

        public override Stream ToStream<T>(T input)
        {
            var stream = new MemoryStream();
            using var sw = new StreamWriter(stream, leaveOpen: true);
            using var writer = new JsonTextWriter(sw);
            Serializer.Serialize(writer, input);
            writer.Flush();
            sw.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}
