using System.Collections.Generic;

namespace Legion
{
	public class Manager
	{
        private Dictionary<string, Channel> channels;

		public Manager()
		{
            this.channels = new Dictionary<string, Channel>();
		}

        public Channel Create(string name)
        {
            if(!this.channels.ContainsKey(name))
            {
                this.channels[name] = new Channel(name);
            }

            return this.channels[name];
        }
	}
}
