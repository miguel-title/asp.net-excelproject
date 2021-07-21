using Neodynamic.SDK.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AtomicSeller.Controllers
{
    public class PrintPDFController : Controller
    {
		// GET: PrintPDF
		public ActionResult Index()
		{
			ViewBag.WCPScript = WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "PrintPDF", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);

			return View();
		}

		[AllowAnonymous]
		public void PrintFile(string useDefaultPrinter, string printerName)
		{
			//full path of the PDF file to be printed
			string pdfFilePath = @"c:\myDocument.pdf";

			//create a temp file name for our PDF file...
			string fileName = "MyFile-" + Guid.NewGuid().ToString("N");

			//Create a PrintFilePDF object with the PDF file
			PrintFilePDF file = new PrintFilePDF(pdfFilePath, fileName);
			//Create a ClientPrintJob and send it back to the client!
			ClientPrintJob cpj = new ClientPrintJob();
			//set file to print...
			cpj.PrintFile = file;

			//set client printer...
			if (useDefaultPrinter == "checked" || printerName == "null")
				cpj.ClientPrinter = new DefaultPrinter();
			else
				cpj.ClientPrinter = new InstalledPrinter(printerName);

			//send it...
			System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
			System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent());
			System.Web.HttpContext.Current.Response.End();

		}
	}
}