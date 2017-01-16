using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion.RestJson
{
    public class ChannelModule : NancyModule
    {
        private IDataAccess data;

        public ChannelModule(IDataAccess d)
            : base("/channels")
        {
            this.data = d;

            Get["/"] = parameters => 
            {
                return getChannels();
            };
        }

        private string getChannels()
        {
            List<string> channels = new List<string>();

            foreach(KeyValuePair<string, Channel> pair in this.data.Channels)
            {
                channels.Add(pair.Value.Name);
            }

            return JsonConvert.SerializeObject(channels, Formatting.Indented);
        }
    }
}
