using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion.RestJson
{
    public class ObjectModule : NancyModule
    {
        private IDataAccess data;

        public ObjectModule(IDataAccess d)
            : base("/objects")
        {
            this.data = d;

            Get["/{channel}"] = parameters =>
            {
                return getObjects(parameters.channel);
            };

            Get["/{channel}/id/{id}"] = parameters =>
            {
                return getObjectById(parameters.channel, parameters.id);
            };

            Put["/{channel}/id/{id}"] = parameters =>
            {
                return setObjectById(parameters.channel, parameters.id);
            };
        }

        private Response getObjects(string channel)
        {
            if (this.data.Channels.ContainsKey(channel))
            {
                List<IdTypeWrapper> elements = new List<IdTypeWrapper>();

                foreach(IObject obj in this.data.Channels[channel])
                {
                    elements.Add(new IdTypeWrapper(obj));
                }

                return JsonConvert.SerializeObject(elements, Formatting.Indented);
            }

            return HttpStatusCode.NotFound;
        }

        private Response getObjectById(string channel, string id)
        {
            if(this.data.Channels.ContainsKey(channel))
            {
                IObject obj = this.data.Channels[channel].Find(id);

                if(obj != null)
                {
                    return JsonConvert.SerializeObject(obj, Formatting.Indented);
                }
            }

            return HttpStatusCode.NotFound;
        }

        private Response setObjectById(string channel, string id)
        {
            if(this.data.Channels.ContainsKey(channel))
            {
                Channel c = this.data.Channels[channel];

                byte[] payload = new byte[this.Request.Body.Length];
                this.Request.Body.Read(payload, 0, payload.Length);

                string encoded = Encoding.UTF8.GetString(payload);

                if (!string.IsNullOrEmpty(encoded))
                {
                    IdTypeWrapper b = JsonConvert.DeserializeObject<IdTypeWrapper>(encoded, new TypeConverter(c));

                    if(b != null)
                    {
                        IObject obj = c.Find(b.Id);

                        if (obj != null)
                        {
                            ElementWrapper<IId> wrapper = new ElementWrapper<IId>(obj);

                            JsonConvert.PopulateObject(encoded, wrapper);

                            return HttpStatusCode.OK;
                        }
                        else
                        {
                            Type prototype = c.FindType(b.Type);

                            if(prototype != null)
                            {
                                Type type = typeof(ElementWrapper<>).MakeGenericType(prototype);

                                dynamic wrapper = JsonConvert.DeserializeObject(encoded, type);

                                if (wrapper.Element != null)
                                {
                                    c.Publish((dynamic)wrapper.Element);
                                }
                            }
                        }
                    }
                }

                return HttpStatusCode.BadRequest;
            }

            return HttpStatusCode.NotFound;
        }
    }
}
