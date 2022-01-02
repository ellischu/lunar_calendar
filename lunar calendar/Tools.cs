using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace lunar_calendar
{
    [Serializable]
    public class Tools
    {
        public static int CheckRange(int num, int Min, int Max)
        {
            while (num < Min || num > Max)
            {
                if (num > Max) { num -= Max; }
                if (num < Min) { num += Max; }
            }
            return num;
        }

        public static double CheckRange(double num, double Min, double Max)
        {
            while (num < Min || num > Max)
            {
                if (num > Max) { num %= Max; }
                if (num < Min) { num += Max; }
            }
            return num;
        }

        public static string ConvertNameId(string strStarId, ConverOption converOption)
        {
            Dictionary<string, string> dicOutput = new();
            using DataTable dtDataTable = ResourceToDataTable(Properties.Resources.IDName);
            switch (converOption)
            {
                case ConverOption.IDtoName:
                    foreach (DataRow drRow in dtDataTable.Rows)
                    {
                        dicOutput.Add(drRow["strID"].ToString(), drRow["strIDName"].ToString());
                    }
                    break;
                case ConverOption.NametoID:
                    foreach (DataRow drRow in dtDataTable.Rows)
                    {
                        dicOutput.Add(drRow["strIDName"].ToString(), drRow["strID"].ToString());
                    }
                    break;
            }
            return dicOutput.ContainsKey(strStarId) ? dicOutput[strStarId] : strStarId;
        }

        internal static DataTable ResourceToDataTable(string resource)
        {
            XmlReader reader = XmlReader.Create(new StringReader(resource), new XmlReaderSettings() { XmlResolver = null });
            XElement xElement = XDocument.Load(reader).Root.Element("DataTable");
            return XmlToDataTable(xElement);
        }


        public static string GetPage(Uri targetUrl)
        {
            string paramList = null;
            int timeout = 300000;
            return GetPage(targetUrl, paramList, timeout);
        }

        public static string GetPage(Uri targetUrl, string paramList, int timeout)
        {
            HttpWebResponse hwrResponse = null;
            string strResult = string.Empty;
            try
            {
                HttpWebRequest hwrRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
                hwrRequest.Method = "GET";
                hwrRequest.ContentType = "application/x-www-form-urlencoded";
                hwrRequest.KeepAlive = true;
                hwrRequest.Timeout = timeout;
                CookieContainer cookieCon = new();
                hwrRequest.CookieContainer = cookieCon;
                //hwrRequest.CookieContainer.SetCookies(new Uri(url), cookieheader);
                StringBuilder UrlEncoded = new();
                char[] reserved = { '?', '=', '&' };
                byte[] SomeBytes = null;

                if (paramList != null)
                {
                    int i = 0, j;
                    while (i < paramList.Length)
                    {
                        j = paramList.IndexOfAny(anyOf: reserved, startIndex: i);
                        if (j == -1)
                        {
                            UrlEncoded.Append(value: HttpUtility.UrlEncode(str: paramList.Substring(startIndex: i, length: paramList.Length - i)));
                            break;
                        }
                        UrlEncoded.Append(value: HttpUtility.UrlEncode(paramList.Substring(startIndex: i, length: j - i)));
                        UrlEncoded.Append(value: paramList.Substring(startIndex: j, length: 1));
                        i = j + 1;
                    }
                    SomeBytes = Encoding.UTF8.GetBytes(s: UrlEncoded.ToString());
                    hwrRequest.ContentLength = SomeBytes.Length;
                    Stream newStream = hwrRequest.GetRequestStream();
                    newStream.Write(buffer: SomeBytes, offset: 0, count: SomeBytes.Length);
                    newStream.Close();
                }
                else
                {
                    hwrRequest.ContentLength = 0;
                }

                hwrResponse = (HttpWebResponse)hwrRequest.GetResponse();
                Stream ReceiveStream = hwrResponse.GetResponseStream();
                Encoding encode = Encoding.GetEncoding(name: "utf-8");
                using StreamReader srReader = new(stream: ReceiveStream, encoding: encode);
                strResult = srReader.ReadToEnd();
                //Char[] read = new Char[256];
                //int count = srReader.Read(read, 0, 256);
                //while (count > 0)
                //{
                //    String str = new String(read, 0, count);
                //    strResult += str;
                //    count = srReader.Read(read, 0, 256);
                //}
            }
            catch (HttpRequestValidationException e)
            {
                strResult = e.ToString();
            }
            finally
            {
                if (hwrResponse != null)
                {
                    hwrResponse.Close();
                }
            }

            return strResult;
        }


        public static void SaveToXml(XElement xElement, string strXmlDirectory, string strXmlFileName)
        {
            XDocument xDoc = new(new XDeclaration("1.0", "utf-8", "true"));
            xDoc.AddFirst(new XElement("FileName", new XAttribute("Name", strXmlFileName)));
            xDoc.Element("FileName").Add(xElement);
            xDoc.Save(Path.Combine(strXmlDirectory, strXmlFileName));
        }

        public static XElement DataSetToXml(DataSet dsInput)
        {
            if (dsInput is null)
            {
                throw new ArgumentNullException(nameof(dsInput));
            }

            XElement xDataSet = new("DataSet", new XAttribute("DataSetName", dsInput.DataSetName));
            foreach (DataTable dtTable in dsInput.Tables)
            {
                xDataSet.Add(DataTableToXml(dtTable));
            }
            return xDataSet;
        }

        public static DataSet XmlToDataSet(XElement xElement)
        {
            if (xElement is null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            using DataSet dsDataSet = new()
            {
                DataSetName = xElement.Attribute("DataSetName").Value,
                Locale = CultureInfo.InvariantCulture
            };
            foreach (var DataTable in xElement.Elements("DataTable"))
            {
                dsDataSet.Tables.Add(XmlToDataTable(DataTable));
            }
            return dsDataSet;
        }

        public static XElement DataTableToXml(DataTable dtInput)
        {
            if (dtInput is null)
            {
                throw new ArgumentNullException(nameof(dtInput));
            }
            XElement xDataTable = new("DataTable", new XAttribute("TableName", dtInput.TableName));
            #region Set Columns
            XElement xColumns = new("DataColumns");
            foreach (DataColumn dcColumn in dtInput.Columns)
            {
                XElement xColumn = new("DataColumn", new XAttribute("ColumnName", dcColumn.ColumnName), new XAttribute("DataType", dcColumn.DataType), new XAttribute("Caption", dcColumn.Caption));
                xColumns.Add(xColumn);
            }
            #endregion Set Columns
            xDataTable.Add(xColumns);
            #region Set Rows
            XElement xRows = new("DataRows");
            foreach (DataRow drRow in dtInput.Rows)
            {
                XElement xRow = new("DataRow");
                foreach (DataColumn dcColumn in dtInput.Columns)
                {
                    XElement rowitem = new("items");
                    rowitem.Add(new XAttribute("ColumnName", dcColumn.ColumnName), drRow[dcColumn.ColumnName]);
                    xRow.Add(rowitem);
                }
                xRows.Add(xRow);
            }
            #endregion Set Rows
            xDataTable.Add(xRows);
            return xDataTable;
        }

        public static DataTable XmlToDataTable(XElement xElement)
        {
            if (xElement is null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }
            using DataTable dtDataTable = new(xElement.Attribute("TableName").Value);
            #region DataColumns
            foreach (var datacolumn in xElement.Element("DataColumns").Elements("DataColumn"))
            {
                dtDataTable.Columns.Add(new DataColumn
                {
                    ColumnName = datacolumn.Attribute("ColumnName").Value,
                    Caption = datacolumn.Attribute("Caption").Value,
                    DataType = Type.GetType(datacolumn.Attribute("DataType").Value)
                });
            }
            #endregion DataColumns
            #region DataRows
            foreach (var datarow in xElement.Element("DataRows").Elements("DataRow"))
            {
                if (!datarow.IsEmpty)
                {
                    DataRow drRow = dtDataTable.NewRow();
                    foreach (var item in datarow.Elements("items"))
                    {
                        drRow[item.Attribute("ColumnName").Value] = item.Value;
                    }
                    dtDataTable.Rows.Add(drRow);
                }
            }
            #endregion DataRows
            return dtDataTable;
        }

        /// <summary>
        /// 六十花甲定五形局
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string GetC60To5Element(string strInput)
        {
            Dictionary<string, string> result = new()
            {
                { "NF01", "NE04" },
                { "NF02", "NE06" },
                { "NF03", "NE03" },
                { "NF04", "NE05" },
                { "NF05", "NE04" },
                { "NF06", "NE06" },
                { "NF07", "NE02" },
                { "NF08", "NE05" },
                { "NF09", "NE04" },
                { "NF10", "NE03" },
                { "NF11", "NE02" },
                { "NF12", "NE05" },
                { "NF13", "NE06" },
                { "NF14", "NE03" },
                { "NF15", "NE02" },
                { "NF16", "NE04" },
                { "NF17", "NE06" },
                { "NF18", "NE03" },
                { "NF19", "NE05" },
                { "NF20", "NE04" },
                { "NF21", "NE06" },
                { "NF22", "NE02" },
                { "NF23", "NE05" },
                { "NF24", "NE04" },
                { "NF25", "NE03" },
                { "NF26", "NE02" },
                { "NF27", "NE05" },
                { "NF28", "NE06" },
                { "NF29", "NE03" },
                { "NF30", "NE02" }
            };
            return result.ContainsKey(strInput) ? result[strInput] : strInput;
        }
    }
}
