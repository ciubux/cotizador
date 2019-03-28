using System;
using Cotizador.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using BusinessLayer;
using Moq;
using System.Web;

namespace UnitTests
{
    [TestClass]
    public class CotizacionTest : CotizacionController, ITest
    {
        [TestMethod]
        public void TestCreate()
        {  
            
            
            var request = new Mock<HttpRequestBase>();

            request.SetupGet(x => x.Headers).Returns(
            new System.Net.WebHeaderCollection {   {"idCliente", "DB36F96D-873B-43CA-887C-0021D1003E2E"}
            });

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            /*CotizacionController cotizacionControllerTest;
            cotizacionControllerTest.ControllerContext = new ControllerContext(context.Object, null, cotizacionControllerTest);
            */


           //      cotizacionControllerTest.GetCliente();

         
          
            Assert.AreEqual("Index", "Index");


         
        }

        [TestMethod]
        public void TestList()
        {
           
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestShow()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
