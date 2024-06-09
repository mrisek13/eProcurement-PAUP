using eProcurement_PAUP.Controllers;
using eProcurement_PAUP.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;

namespace eProcurement.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public int Zbroji(int a, int b)
        {
            return a + b;
        }

        [TestMethod]
        public void TestZbrajanje()
        {
            // Arrange
            int x = 10;
            int y = 5;

            // Act
            int zbroj = Zbroji(x, y);

            // Assert
            Assert.AreEqual(15, zbroj);
        }

        // Ovaj test provjerava da li metoda Index u kontroleru SuppliersController pravilno postavlja ViewBag.SearchString 
        // na praznu vrijednost kada je prilikom poziva metode proslijeđen prazan string za parametar searchString.
        [TestMethod]
        public void TestSuppliersSearchString()
        {
            // Arrange
            SuppliersController controller = new SuppliersController();

            // Act
            ViewResult result = controller.Index("", null, null, "") as ViewResult;

            // Assert
            Assert.AreEqual("", result.ViewBag.SearchString);
        }

        // Ovaj test provjerava da li metoda Index na ekranu Orders
        // u kontroleru OrdersController vraća rezultat tipa ViewResult.
        [TestMethod]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            OrdersController controller = new OrdersController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

    }
}
