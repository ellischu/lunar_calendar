using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;

namespace lunar_calendar
{
    public class LifeWeight
    {
        public DataTable XmlToDataTable(DataTable dtInput, string Resource)
        {
            DataTable dtReturn = dtInput.Clone();

            XmlDocument xmldoc = new() { XmlResolver = null };
            StringReader sreader = new(Resource);
            XmlReader reader = XmlReader.Create(sreader, new XmlReaderSettings() { XmlResolver = null });
            xmldoc.Load(reader);

            foreach (XmlNode node in xmldoc.DocumentElement)
            {
                if (node.HasChildNodes) // check has data
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.Name == "tblname") //table name
                        {
                            dtReturn.TableName = child.InnerText;
                        }
                        if (child.Name == "rowsdata" && child.HasChildNodes) //datarows
                        {
                            DataRow drDataRow = dtReturn.NewRow(); // inittial datarow
                            foreach (XmlNode child1 in child.ChildNodes)
                            {
                                if (child1.Name == dtReturn.Columns[0].ColumnName) //first column
                                {
                                    drDataRow = dtReturn.NewRow();
                                }
                                drDataRow[child1.Name] = child1.InnerText;
                                if (child1.Name == dtReturn.Columns[dtReturn.Columns.Count-1].ColumnName) //last column
                                {
                                    dtReturn.Rows.Add(drDataRow);
                                }
                            }
                        }
                    }
                }
            }

            return dtReturn;
        }

        /// <summary>
        /// 獲取命重表
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, double> TblWeight()      //獲取命重表 
        {
            Dictionary<string, double> dicOutput = new();

            DataTable dataTable = new();
            dataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "strInput",
                DataType = typeof(string),
            });
            dataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "shtWeight",
                DataType = typeof(double),
            });

            using DataTable dtDataTable = new LifeWeight().XmlToDataTable(dataTable, Properties.Resources.LifeWeight);

            foreach (DataRow drRow in dtDataTable.Rows)
            {
                dicOutput.Add(drRow["strInput"].ToString(), double.Parse(drRow["shtWeight"].ToString(), CultureInfo.InvariantCulture));
            }
            return dicOutput;
        }


    }
}
