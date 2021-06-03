using System;
using System.Collections.Generic;
using System.Text;

namespace DataTable
{
    class Row
    {
        static void Main(string[] args)
        {
            // Test the row routines

            DataHandler dh = new DataHandler("", "test");
            dh.Open(true);
            DataColumn c = new DataColumn
            {
                ColumnName = "first",
                DataType = System.Type.GetType("System.String"),
                MaxLength = 10
            };
            dh.Add(c);
            c = new DataColumn
            {
                ColumnName = "second",
                DataType = System.Type.GetType("System.String")
            };
            dh.Add(c);
            c = new DataColumn
            {
                ColumnName = "third",
                DataType = System.Type.GetType("System.Int16")
            };
            dh.Add(c);

            // Test row routines

            DataRow r = new DataRow(dh);
            r[0] = "hello";
            r[1] = "goodby";
            r[2] = 1;
            dh.Create(r);

            r = new DataRow(dh);
            r[0] = "begging";
            r[1] = "end";
            r[2] = 2;
            dh.Create(r);

            r = new DataRow(dh);
            r[0] = "from";
            r[1] = "to";
            r[2] = 3;
            dh.Create(r);

            r = new DataRow(dh);
            r[0] = "start";
            r[1] = "finish";
            r[2] = 4;
            dh.Create(r);

            // Get the data

            Console.WriteLine("Records=" + dh.Records);

            for (int i = 0; i < dh.Records; i++)
            {
                DataRow dr;
                dr = dh.Read(i);
                for (int j=0; j < dh.Items; j++)
                {
                    Console.WriteLine(i + " " + j + " '" + dr.ItemArray[j] + "'");
                }
            }

            // Delete some data

            dh.Delete(2);

            Console.WriteLine("Records=" + dh.Records);

            for (int i = 0; i < dh.Records; i++)
            {
                DataRow dr;
                dr = dh.Read(i);
                for (int j = 0; j < dh.Items; j++)
                {
                    Console.WriteLine(i + " " + j + " '" + dr.ItemArray[j] + "'");
                }
            }

        }
    }
}
