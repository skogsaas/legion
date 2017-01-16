using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion.RestJson
{
    public class IdTypeWrapper : ElementWrapper<IId>
    {
        public string Id { get; set; }
        public string Type { get; set; }

        public IdTypeWrapper()
        {

        }

        public IdTypeWrapper(IId element)
            : base(element)
        {
            this.Id = element.Id;
            this.Type = element.GetType().FullName;
        }
    }
}
