﻿using System;
using System.Collections.Generic;
using System.Text;
using DataTableLibrary;

namespace DataTableConsole
{
    class Row
    {
        static void Main(string[] args)
        {
            // Test the row routines

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

            // Get the data

            Console.WriteLine("Records=" + dh.Size);

            for (int i = 0; i < dh.Size; i++)
            {
                or = dh.Read(i);
                for (int j=0; j < dh.Items; j++)
                {
                    Console.WriteLine(i + " " + j + " '" + or[j] + "'");
                }
            }

            // Delete some data

            int k = 2;  // Index 2
            dh.Delete(k);

            Console.WriteLine("Records=" + dh.Size);

            for (int i = 0; i < dh.Size; i++)
            {
                or = dh.Read(i);
                for (int j = 0; j < dh.Items; j++)
                {
                    Console.WriteLine(i + " " + j + " '" + or[j] + "'");
                }
            }

            // Rebuild the index

        }
    }
}
