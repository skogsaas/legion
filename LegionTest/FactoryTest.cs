using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace LegionTest
{
    [TestClass]
    public class FactoryTest
    {
        [TestMethod]
        public void TestCreate()
        {
            int events = 0;

            Legion.Factory f = new Legion.Factory();
            IPropertyInterface i = f.create<IPropertyInterface>();
            i.PropertyChanged += (object caller, PropertyChangedEventArgs args) => { events++; };

            Assert.AreEqual(0, events);
            i.Dummy = 10;
            Assert.AreEqual(1, events);

        }
    }
}
