using Chilkat;
using DataTableLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataTableConsole
{
    internal class Table
    {
        static void Main(string[] args)
        {
            DataHandler dh = new DataHandler("", "test");
            dh.Reset();
            dh.Open();
            DataColumn c = new DataColumn
            {
                ColumnName = "first",
                DataType = System.Type.GetType("System.String"),
                MaxLength = 10
            };

            DataHandler.Property p = new DataHandler.Property(c.ColumnName, c.Flag, c.Ordinal, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Add(p);
            c = new DataColumn
            {
                ColumnName = "second",
                DataType = System.Type.GetType("System.String")
            };

            p = new DataHandler.Property(c.ColumnName, c.Flag, c.Ordinal, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Add(p);
            c = new DataColumn
            {
                ColumnName = "third",
                DataType = System.Type.GetType("System.Int16")
            };
            p = new DataHandler.Property(c.ColumnName, c.Flag, c.Ordinal, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Add(p);

            // Test row routines

            DataRow r = new DataRow(dh);
            r[0] = "hello";
            r[1] = "goodby";
            r[2] = 1;
            object[] or = new object[3] { r[0], r[1], r[2] };
            dh.Create(or);

            r = new DataRow(dh);
            r[0] = "begging";
            r[1] = "end";
            r[2] = 2;
            or = new object[3] { r[0], r[1], r[2] };
            dh.Create(or);

            r = new DataRow(dh);
            r[0] = "from";
            r[1] = "to";
            r[2] = 3;
            or = new object[3] { r[0], r[1], r[2] };
            dh.Create(or);

            r = new DataRow(dh);
            r[0] = "start";
            r[1] = "finish";
            r[2] = 4;
            or = new object[3] { r[0], r[1], r[2] };
            dh.Create(or);

        }
    }
}
