using System;
using DataTableLibrary;

namespace DataTableConsole
{
    class Column
    {
        static void Main(string[] args)
        {
            // Test the column routines

            DataHandler dh = new DataHandler("", "test");
            dh.Reset();
            dh.Open();
            DataColumn c = new DataColumn();
            c.ColumnName = "first";
            c.Ordinal = 1;
            c.DataType = System.Type.GetType("System.String");
            c.MaxLength = 10;
            DataHandler.Property p = new DataHandler.Property(c.ColumnName, c.Flag, c.Ordinal, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Add(p);
            c = new DataColumn();
            c.ColumnName = "second";
            c.DataType = System.Type.GetType("System.String");
            p = new DataHandler.Property(c.ColumnName, c.Flag, c.Ordinal, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Add(p);
            c = new DataColumn();
            c.ColumnName = "third";
            c.DataType = System.Type.GetType("System.Int16");
            p = new DataHandler.Property(c.ColumnName, c.Flag, c.Ordinal, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Add(p);
            Console.WriteLine("Items=" + dh.Items);

            // Display the data

            foreach (DataHandler.Property field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

            // Update - shorter string

            c = new DataColumn();
            c.ColumnName = "2";
            c.DataType = System.Type.GetType("System.String");
            p = new DataHandler.Property(c.ColumnName, c.Flag, c.Ordinal, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Set(p, 1);
            Console.WriteLine("Items=" + dh.Items);

            foreach (DataHandler.Property field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

            // Update - longer string

            c = new DataColumn();
            c.ColumnName = "deuxième";
            c.DataType = System.Type.GetType("System.String");
            p = new DataHandler.Property(c.ColumnName, c.Flag, c.Ordinal, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Set(p, 1);
            Console.WriteLine("Items=" + dh.Items);

            foreach (DataHandler.Property field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

            p = dh.Get(0);
            Console.WriteLine("Get(0)" + p.Name);

            // Remove - column

            Console.WriteLine("RemoveAt");
            c = new DataColumn();
            c.ColumnName = "second";
            dh.RemoveAt(1);

            foreach (DataHandler.Property field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

            // Remove - column

            Console.WriteLine("Remove");
            c = new DataColumn();
            c.ColumnName = "second";
            p = new DataHandler.Property(c.ColumnName, c.Flag, c.Ordinal, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Remove(p);

            foreach (DataHandler.Property field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

            //

        }
    }
}

