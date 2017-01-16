using Nancy.Hosting.Self;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion.RestJson
{
    public class Server
    {
        private NancyHost host;
        private DataAccess data;

        private Bootstrapper bootstrapper;

        public Server(int port, params Channel[] channels)
        {
            this.data = new DataAccess(channels);
            this.bootstrapper = new Bootstrapper(this.data);

            this.host = new NancyHost(this.bootstrapper, new Uri($"http://localhost:{port}"));
        }

        public void Start()
        {
            this.host.Start();
        }

        public void Stop()
        {
            this.host.Stop();
        }
    }
}
