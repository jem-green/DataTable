using System;
using System.Data;
using DataTableLibrary;

namespace DataTableConsole
{
    class Program
    {
        static void Main(string[] args)
        {

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
            Console.WriteLine("Fields=" + dr.ItemArray.Length); ;
            dt.Rows.Add(dr);
            Console.WriteLine("Records=" + dt.Rows.Count);

            foreach (System.Data.DataRow r in dt.Rows)
            {
                Console.WriteLine(r["id"] + "," + r["name"]);
            }

            try
            {
                dt.Columns.Remove("Test");
                Console.WriteLine("Removed");
            }
            catch { };

            System.Data.DataRow[] rs = dt.Select();
            foreach (System.Data.DataRow r in rs)
            {
                Console.WriteLine(r["id"] + "," + r["name"]);
            }

            dt.Dispose();

            Console.WriteLine("----------");

            PersistentDataTable pdt = new PersistentDataTable("test_table",true);

            DataTableLibrary.DataColumn pdc;
            try
            {
                pdc = new DataTableLibrary.DataColumn("id");
                pdc.DataType = System.Type.GetType("System.Int32");
                //pdc.Unique = true;
                pdt.Columns.Add(pdc);
            }
            catch { };

            try
            {
                pdc = new DataTableLibrary.DataColumn("name");
                pdc.DataType = System.Type.GetType("System.String");
                pdt.Columns.Add(pdc);
            }
            catch { };

            DataTableLibrary.DataRow pdr = pdt.NewRow();
            pdr["id"] = 1;
            pdr["name"] = "jeremy";
            Console.WriteLine("Fields=" + pdr.ItemArray.Length);
            pdt.Rows.Add(pdr);
            Console.WriteLine("Records=" + pdt.Rows.Count);

            foreach (DataTableLibrary.DataRow r in pdt.Rows)
            {
                Console.WriteLine(r["id"] + "," + r["name"]);
            }

            try
            {
                pdt.Columns.Remove("Test");
                Console.WriteLine("Removed");
            }
            catch { };

            DataTableLibrary.DataRow[] prs = pdt.Select();
            foreach (DataTableLibrary.DataRow r in prs)
            {
                Console.WriteLine(r["id"] + "," + r["name"]);
            }

            pdt.Dispose();  // Need to agree if this deletes the datatable files

        }
    }
}

