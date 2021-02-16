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
            Console.WriteLine("Count=" + dr.ItemArray.Length);
            dt.Rows.Add(dr);

            foreach (System.Data.DataRow r in dt.Rows)
            {
                Console.WriteLine(r["id"] + "," + r["name"]);
            }

            dt.Select("", "");

            dt.Dispose();

            Console.WriteLine("----------");

            PersistentDataTable pdt = new PersistentDataTable(false);

            Datacolumn pdc;
            try
            {
                pdc = new Datacolumn("id");
                pdc.DataType = System.Type.GetType("System.Int32");
                //pdc.Unique = true;
                pdt.Columns.Add(pdc);
            }
            catch { };

            try
            {
                pdc = new Datacolumn("name");
                pdc.DataType = System.Type.GetType("System.String");
                pdt.Columns.Add(pdc);
            }
            catch { };

            DataRow pdr = pdt.NewRow();
            pdr["id"] = 1;
            pdr["name"] = "jeremy";
            Console.WriteLine("Fields=" + pdr.ItemArray.Length);
            pdt.Rows.Add(pdr);
            Console.WriteLine("Count=" + pdt.Rows.Count);

            foreach (DataRow r in pdt.Rows)
            {
                Console.WriteLine(r["id"] + "," + r["name"]);
            }

            //pdt.Select("", "");

            pdt.Dispose();

        }
    }
}

