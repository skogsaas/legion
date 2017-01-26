using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skogsaas.Legion;
using System.ComponentModel;

namespace LegionTest
{
    [TestClass]
    public class FactoryTest
    {
        public static Channel channel;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            FactoryTest.channel = Manager.Create("TEST");
        }

        [TestMethod]
        public void TestCreate()
        {
            int oEvents = 0;
            int sEvents = 0;

            TestObject i = FactoryTest.channel.CreateType<TestObject>();
            i.PropertyChanged += (object caller, PropertyChangedEventArgs args) => { oEvents++; };

            Assert.AreEqual(0, oEvents);
            i.Dummy1 = 10;
            Assert.AreEqual(1, oEvents);

            Assert.IsNull(i.Dummy2);
            i.Dummy2 = FactoryTest.channel.CreateType<TestStruct>();
            Assert.IsNotNull(i.Dummy2);

            Assert.AreEqual(2, oEvents);

            i.Dummy2.PropertyChanged += (object caller, PropertyChangedEventArgs args) => { sEvents++; };
            Assert.AreEqual(0, sEvents);
            i.Dummy2.Dummy1 = "Hello World!";
            Assert.AreEqual(1, sEvents);
            Assert.AreEqual(3, oEvents);
        }

        [TestMethod]
        public void TestParent()
        {
            ParentObject i = FactoryTest.channel.CreateType<ParentObject>();

            i.Alias = "Hello World!";
            Assert.AreEqual("Hello World!", i.Alias);
        }

        [TestMethod]
        public void TestId()
        {
            TestObject i = FactoryTest.channel.CreateType<TestObject>("123ABC");
            
            Assert.AreEqual("123ABC", i.Id);
        }

        [TestMethod]
        public void TestCollection()
        {
            CollectionObject i = FactoryTest.channel.CreateType<CollectionObject>();

        }
    }
}