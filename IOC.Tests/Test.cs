using NUnit.Framework;
using System;
//using InversionOfControlContainer;

namespace InversionOfControlContainer.Tests
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void Can_Resolve_Types()
        {
            var ioc = new Container();
            ioc.For<ILogger>().Use<SqlServerLogger>();

            var logger = ioc.Resolve<ILogger>();
            Assert.AreEqual(typeof(SqlServerLogger), logger.GetType());
        }
    }
}


