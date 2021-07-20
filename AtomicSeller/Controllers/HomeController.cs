using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AtomicSeller.Helpers;
using AtomicSeller.ViewModels;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using AtomicSeller;
using System.Diagnostics;
using System.Drawing.Printing;

namespace AtomicSeller.Controllers
{
    public class HomeController : BaseController
    {
        string m_printerName = "";
        [HttpGet]
        public ActionResult Index()
        {
            //SessionBag.SetSessionBagInitData();
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);

            if (Request != null &&
                Request.UrlReferrer != null &&
                !string.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Excellisting()
        {
            List<DeliveryDM> _Deliveries = InitDeliveries();
            List <DeliveryProductDM> _Products = InitProducts();

            new ExcelExport().ExportToExcelFile(_Deliveries, _Products);
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult LabelPrinting()
        {
            return RedirectToAction("Index", "LabelPrinting");
        }


        public void SendLabelsToPrinter()
        {
            List<DeliveryDM> deliveries = InitDeliveries();
            foreach (DeliveryDM delivery in deliveries)
            {
                string PathToExecutable = @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
                PrinterSettings settings = new PrinterSettings();
                var PrintFileToPrinter = string.Format("/t \"{0}\" \"{1}\"", delivery.LabelPath, settings.PrinterName);
                var args = string.Format("{0}", PrintFileToPrinter);
                Process process = new Process();
                process.StartInfo.FileName = PathToExecutable;
                process.StartInfo.Arguments = args;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.ErrorDialog = false;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
            }
            
        }

        public void SelectPrinter()
        {
        }

        public List<DeliveryDM> InitDeliveries()
        {
            List<DeliveryDM> _Deliveries = new List<DeliveryDM>();


            DeliveryDM _Delivery1 = new DeliveryDM();
            _Delivery1.DeliveryID = 1;
            _Delivery1.RecipLastName = "Lolo";
            _Delivery1.RecipCity = "MARSEILLE";
            _Delivery1.ShippingDate = new DateTime(2021, 07, 06);
            _Delivery1.OrderKey = "2596";
            _Delivery1.LabelPath = @"D:\SampleLabels\SampleLabel1.pdf";

            _Deliveries.Add(_Delivery1);

            DeliveryDM _Delivery2 = new DeliveryDM();
            _Delivery2.DeliveryID = 2;
            _Delivery2.RecipLastName = "Lili";
            _Delivery2.RecipCity = "LYON";
            _Delivery2.ShippingDate = new DateTime(2021, 07, 06);
            _Delivery2.OrderKey = "2369";
            _Delivery2.LabelPath = @"D:\SampleLabels\SampleLabel2.pdf";

            _Deliveries.Add(_Delivery2);

            DeliveryDM _Delivery3 = new DeliveryDM();
            _Delivery3.DeliveryID = 3;
            _Delivery3.RecipLastName = "Lala";
            _Delivery3.RecipCity = "PARIS";
            _Delivery3.ShippingDate = new DateTime(2021, 07, 06);
            _Delivery3.OrderKey = "5566";
            _Delivery3.LabelPath = @"D:\SampleLabels\SampleLabel3.pdf";

            _Deliveries.Add(_Delivery3);

            DeliveryDM _Delivery4 = new DeliveryDM();
            _Delivery4.DeliveryID = 4;
            _Delivery4.RecipLastName = "Lele";
            _Delivery4.RecipCity = "MARSEILLE";
            _Delivery4.ShippingDate = new DateTime(2021, 07, 06);
            _Delivery4.OrderKey = "2582";
            _Delivery4.LabelPath = @"D:\SampleLabels\SampleLabel4.pdf";

            _Deliveries.Add(_Delivery4);

            return _Deliveries;
        }


        public List<DeliveryProductDM> InitProducts()
        {
            List<DeliveryProductDM> _OrderProducts = new List<DeliveryProductDM>();

            DeliveryProductDM _Product1 = new DeliveryProductDM();

            _Product1.DeliveryID = 1;
            _Product1.ProductName = "ACE RED LION";
            _Product1.Quantity = 2;
            _Product1.Size = "39";
            _Product1.Location = "1-8-A";

            _OrderProducts.Add(_Product1);

            DeliveryProductDM _Product2 = new DeliveryProductDM();

            _Product2.DeliveryID = 2;
            _Product2.ProductName = "LILITH BLACK";
            _Product2.Quantity = 1;
            _Product2.Size = "45";
            _Product2.Location = "1-12-B";

            _OrderProducts.Add(_Product2);

            DeliveryProductDM _Product3 = new DeliveryProductDM();

            _Product3.DeliveryID = 1;
            _Product3.ProductName = "ONIX";
            _Product3.Quantity = 2;
            _Product3.Size = "29";
            _Product3.Location = "2-12-C";

            _OrderProducts.Add(_Product3);

            DeliveryProductDM _Product4 = new DeliveryProductDM();

            _Product4.DeliveryID = 2;
            _Product4.ProductName = "LUX WHITE";
            _Product4.Quantity = 1;
            _Product4.Size = "36";
            _Product4.Location = "5-4-B";

            _OrderProducts.Add(_Product4);

            DeliveryProductDM _Product5 = new DeliveryProductDM();

            _Product5.DeliveryID = 2;
            _Product5.ProductName = "LUX BLUE";
            _Product5.Quantity = 1;
            _Product5.Size = "36";
            _Product5.Location = "5-4-A";

            _OrderProducts.Add(_Product5);

            // ...

            return _OrderProducts;
        }

    }
}