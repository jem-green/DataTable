using System;
using System.Data;

namespace DataTable
{
    class Program
    {
        static void Main(string[] args)
        {
            //DataSet d = new DataSet("test");

            System.Data.DataTable dt = new System.Data.DataTable("test_table");

            System.Data.DataColumn dc = new System.Data.DataColumn("id");
            dc.DataType = System.Type.GetType("System.Int32");
            dc.Unique = true;
            dt.Columns.Add(dc);

            dc = new System.Data.DataColumn("name");
            dc.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(dc);

            System.Data.DataRow dr = dt.NewRow();
            dr["id"] = 1;
            dr["name"] = "jeremy";
            dt.Rows.Add(dr);

            foreach (System.Data.DataRow r in dt.Rows)
            {
                Console.WriteLine(r["id"] + "," + r["name"]);
            }

            dt.Select("", "");

            dt.Dispose();

            Console.WriteLine("----------");

            PersistentDataTable pdt = new PersistentDataTable(true);

            DataColumn pdc = new DataColumn("id");
            pdc.DataType = System.Type.GetType("System.Int32");
            //pdc.Unique = true;
            pdt.Columns.Add(pdc);

            pdc = new DataColumn("name");
            pdc.DataType = System.Type.GetType("System.String");
            pdt.Columns.Add(pdc);

            DataRow pdr = pdt.NewRow();
            pdr["id"] = 1;
            pdr["name"] = "jeremy";
            pdt.Rows.Add(pdr);

            foreach (DataRow r in pdt.Rows)
            {
                Console.WriteLine(r["id"] + "," + r["name"]);
            }

            //pdt.Select("", "");

            pdt.Dispose();

        }
    }
}

