using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skogsaas.Legion;
using Newtonsoft.Json;
using Skogsaas.Legion.Json;

namespace Legion.Json.Test
{
    [TestClass]
    public class TestCollection
    {
        [TestMethod]
        public void TestSerialize()
        {
            Channel channel = Manager.Create("TEST");

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new TypeConverter(channel));

            IStructCollectionObject obj = channel.CreateType<IStructCollectionObject>();
            obj.Collection = new System.Collections.Generic.List<IDummyStruct>();
            obj.Collection.Add(channel.CreateType<IDummyStruct>());

            string data = JsonConvert.SerializeObject(obj, Formatting.Indented);

            obj.Collection = null;

            JsonConvert.PopulateObject(data, obj, settings);
        }
    }
}
