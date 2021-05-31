using System;
using System.Data;

namespace DataTable
{
    class Column
    {
        static void Main(string[] args)
        {
            // Test the column routines

            DataHandler dh = new DataHandler("", "test");
            dh.Open(true);
            DataColumn c = new DataColumn();
            c.ColumnName = "first";
            c.DataType = System.Type.GetType("System.String");
            c.MaxLength = 10;
            dh.Add(c);
            c = new DataColumn();
            c.ColumnName = "second";
            c.DataType = System.Type.GetType("System.String");
            dh.Add(c);
            c = new DataColumn();
            c.ColumnName = "third";
            c.DataType = System.Type.GetType("System.Int16");
            dh.Add(c);

            // Update - shorter string

            c = new DataColumn();
            c.ColumnName = "2";
            c.DataType = System.Type.GetType("System.String");
            dh.Set(c, 1);

            // Update - longer string

            c = new DataColumn();
            c.ColumnName = "deuxième";
            c.DataType = System.Type.GetType("System.String");
            dh.Set(c, 1);

            c = dh.Get(0);
            Console.WriteLine(c.ColumnName);

            // Remove - column

            c = new DataColumn();
            c.ColumnName = "second";
            dh.Remove(c);

        }
    }
}

