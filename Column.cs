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
            DataHandler.Field f = new DataHandler.Field(c.ColumnName, c.Flag, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Add(f);
            c = new DataColumn();
            c.ColumnName = "second";
            c.DataType = System.Type.GetType("System.String");
            f = new DataHandler.Field(c.ColumnName, c.Flag, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Add(f);
            c = new DataColumn();
            c.ColumnName = "third";
            c.DataType = System.Type.GetType("System.Int16");
            f = new DataHandler.Field(c.ColumnName, c.Flag, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Add(f);

            // 

            foreach (DataHandler.Field field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

            // Update - shorter string

            c = new DataColumn();
            c.ColumnName = "2";
            c.DataType = System.Type.GetType("System.String");
            f = new DataHandler.Field(c.ColumnName, c.Flag, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Set(f, 1);

            foreach (DataHandler.Field field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

            // Update - longer string

            c = new DataColumn();
            c.ColumnName = "deuxième";
            c.DataType = System.Type.GetType("System.String");
            f = new DataHandler.Field(c.ColumnName, c.Flag, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Set(f, 1);

            foreach (DataHandler.Field field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

            f = dh.Get(0);
            Console.WriteLine("Get(0)" + f.Name);

            // Remove - column

            Console.WriteLine("Remove");
            c = new DataColumn();
            c.ColumnName = "second";
            dh.RemoveAt(1);

            foreach (DataHandler.Field field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

            // Remove - column

            Console.WriteLine("Remove");
            c = new DataColumn();
            c.ColumnName = "second";
            f = new DataHandler.Field(c.ColumnName, c.Flag, Type.GetTypeCode(c.DataType), c.MaxLength, c.Primary);
            dh.Remove(f);

            foreach (DataHandler.Field field in dh.Fields)
            {
                Console.WriteLine(field.ToString());
            }

        }
    }
}

