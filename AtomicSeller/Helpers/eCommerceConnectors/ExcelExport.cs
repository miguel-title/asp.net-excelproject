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
        public List<DeliveryDM> SortDeliveries(List<DeliveryDM> _Deliveries, List<DeliveryProductDM> _Products)
        {
            List<string> _result = new List<string>();

            //Set NbOrderLines to Deliveries.
            foreach (DeliveryDM _delivery in _Deliveries)
            {
                int deliveryID = _delivery.DeliveryID;

                int nCount = 0;
                foreach (DeliveryProductDM _deliveryproduct in _Products)
                {
                    if (_deliveryproduct.DeliveryID == deliveryID)
                    {
                        nCount++;
                        // Added Patrice 20210724
                        if (string.IsNullOrEmpty(_delivery.LowestProductLocation))
                            _delivery.LowestProductLocation = _deliveryproduct.Location;
                        else
                        {
                            if (string.Compare (_delivery.LowestProductLocation, _deliveryproduct.Location)<0)
                                _delivery.LowestProductLocation = _deliveryproduct.Location;
                        }
                    }
                }

                _delivery.NbOrderLines = nCount;

            }

            _Deliveries = _Deliveries.OrderBy(m => m.NbOrderLines).ThenByDescending(m => m.LowestProductLocation).ToList();

            return (_Deliveries);
            /*
            foreach (DeliveryDM _delivery in _Deliveries)
            {
                _result.Add(_delivery.LabelPath);
            }

            return _result;
            */
        }
        public string ExportToExcelFile(List<DeliveryDM> _Deliveries, List<DeliveryProductDM> _Products)
        {
            _Deliveries = SortDeliveries(_Deliveries, _Products);

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

            //Set NbOrderLines to Deliveries.
            /*
            foreach (DeliveryDM _delivery in _Deliveries)
            {
                int deliveryID = _delivery.DeliveryID;

                int nCount = 0;
                foreach (DeliveryProductDM _deliveryproduct in _Products)
                {
                    if (_deliveryproduct.DeliveryID == deliveryID)
                    {
                        nCount++;
                    }
                }

                _delivery.NbOrderLines = nCount;

            }

            _Deliveries = _Deliveries.OrderBy(m => m.NbOrderLines).ToList();
            */
            foreach (DeliveryDM _delivery in _Deliveries)
            {
                bool isProductExist = false;
                int deliveryID = _delivery.DeliveryID;
                List<ExcelDM> _tmpresult = new List<ExcelDM>();
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
                        _tmpresult.Add(_exceldata);
                    }
                }


                if (!isProductExist)
                {
                    ExcelDM _exceldata = new ExcelDM();
                    _exceldata.Date = _delivery.ShippingDate.ToString();
                    _exceldata.CustomerName = _delivery.RecipLastName;
                    _exceldata.OrderKey = _delivery.OrderKey;
                    _exceldata.DeliveryCity = _delivery.RecipCity;
                    _tmpresult.Add(_exceldata);
                }

                _tmpresult = _tmpresult.OrderBy(m => m.Location).ToList();

                foreach (ExcelDM result in _tmpresult)
                {
                    _result.Add(result);
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