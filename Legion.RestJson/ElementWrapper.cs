using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion.RestJson
{
    public class ElementWrapper<T>
    {
        public T Element { get; set; }

        public ElementWrapper()
        {

        }

        public ElementWrapper(T element)
        {
            this.Element = element;
        }
    }
}
