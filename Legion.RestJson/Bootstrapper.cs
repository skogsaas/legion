using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion.RestJson
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private DataAccess data;

        public Bootstrapper(DataAccess d)
        {
            this.data = d;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            container.Register<IDataAccess, DataAccess>(this.data);
        }
    }
}
