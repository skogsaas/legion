using System.Collections.Generic;

namespace Skogsaas.Legion
{
	public class Manager
	{
        private Dictionary<string, Channel> channels;

        private static Manager instance = null;

        private static Manager Instance
        {
            get
            {
                if(Manager.instance == null)
                {
                    Manager.instance = new Manager();
                }

                return Manager.instance;
            }
        }

        public static Channel Create(string name)
        {
            return Manager.Instance.create(name);
        }

        #region Implementation

        private Manager()
		{
            this.channels = new Dictionary<string, Channel>();
		}

        private Channel create(string name)
        {
            if(!this.channels.ContainsKey(name))
            {
                this.channels[name] = new Channel(name);
            }

            return this.channels[name];
        }

        #endregion
    }
}
