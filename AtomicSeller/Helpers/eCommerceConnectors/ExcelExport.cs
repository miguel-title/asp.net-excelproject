using AtomicSeller.Models;
using AtomicSeller.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ShopifyOrderAPI.Models;
using iTextSharp.text.pdf;
using ClosedXML.Excel;
using System.Data;
using System.ComponentModel;

namespace AtomicSeller
{
    class ExcelExport
    {

        public string ExportToExcelFile(List<DeliveryDM> _Deliveries, List<DeliveryProductDM> _Products)
        {
            List<ExcelDM> _data = MakeExcelData(_Deliveries, _Products);
            DataTable _DataTable = ConvertListToDataTable(_data);
            
            XLWorkbook wb = new XLWorkbook();
            string ExportFilePath = string.Empty;

            try
            {      

                wb.Worksheets.Add(_DataTable, "Custom data");

                string FileName = DateTime.UtcNow.ToString("s").Replace(":", "") + "Export.xlsx";


                string ExportFilesDir = @"D:\testexceldata\";

                if (!System.IO.Directory.Exists(ExportFilesDir))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(ExportFilesDir);
                    }
                    catch (Exception ex)
                    {
                        return "ERROR" + ex.Message;
                    }
                }

                ExportFilePath = Path.Combine(ExportFilesDir, FileName);
                wb.SaveAs(ExportFilePath);
            }
            catch (Exception ex)
            {
            }
            return ExportFilePath;
        }


        public List<ExcelDM> MakeExcelData(List<DeliveryDM> _Deliveries, List<DeliveryProductDM> _Products)
        {
            List<ExcelDM> _result = new List<ExcelDM>();

            foreach (DeliveryDM _delivery in _Deliveries)
            {
                bool isProductExist = false;
                int deliveryID = _delivery.DeliveryID;
                foreach (DeliveryProductDM _deliveryproduct in _Products)
                {
                    ExcelDM _exceldata = new ExcelDM();
                    _exceldata.Date = _delivery.ShippingDate.ToString();
                    _exceldata.CustomerName = _delivery.RecipLastName;
                    _exceldata.OrderKey = _delivery.OrderKey;
                    _exceldata.DeliveryCity = _delivery.RecipCity;

                    if (_deliveryproduct.DeliveryID == deliveryID)
                    {
                        _exceldata.ProductsName = _deliveryproduct.ProductName;
                        _exceldata.Size = _deliveryproduct.Size;
                        _exceldata.Quantity = _deliveryproduct.Quantity.ToString();
                        _exceldata.Location = _deliveryproduct.Location;

                        isProductExist = true;
                        _result.Add(_exceldata);
                    }
                }

                if (!isProductExist)
                {
                    ExcelDM _exceldata = new ExcelDM();
                    _exceldata.Date = _delivery.ShippingDate.ToString();
                    _exceldata.CustomerName = _delivery.RecipLastName;
                    _exceldata.OrderKey = _delivery.OrderKey;
                    _exceldata.DeliveryCity = _delivery.RecipCity;
                    _result.Add(_exceldata);
                }

            }

            return _result;
        }

        public DataTable ConvertListToDataTable<T>(IList<T> data)

        {
            PropertyDescriptorCollection properties = null;
            try
            {
                properties = TypeDescriptor.GetProperties(typeof(T));
            }
            catch (Exception ex)
            {
            }
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                try
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                catch (Exception ex)
                { }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;

        }

    }
}