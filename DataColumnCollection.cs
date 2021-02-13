using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DataTable
{
    public class DataColumnCollection<DataColumn> : Collection<DataColumn>
    {
        public new void Add(DataColumn column)
        {
            // Actually need to add the column to the file but 
            // dont have a reference here
            // 
            base.Add(column);
        }
    }
}
