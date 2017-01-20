using Newtonsoft.Json;
using System;

namespace Skogsaas.Legion.Json
{
    public class TypeConverter : JsonConverter
    {
        private Channel channel;

        public TypeConverter(Channel c)
        {
            this.channel = c;
        }

        public override bool CanConvert(Type objectType)
        {
            return this.channel.FindType(objectType) != null;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer s)
        {
            s.NullValueHandling = NullValueHandling.Include;

            if(existingValue == null)
            {
                Type generated = this.channel.FindType(objectType);

                return s.Deserialize(reader, generated);
            }
            else
            {
                if(reader.TokenType == JsonToken.Null)
                {
                    reader.Skip();
                    return null;
                }
                else
                {
                    s.Populate(reader, existingValue);

                    return existingValue;
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
