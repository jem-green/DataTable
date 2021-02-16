using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Data;

namespace DataTable
{
    public class DataColumnCollection : ObservableCollection<Datacolumn>
    {
        public new void Add(Datacolumn column)
        {
            // Actually need to add the column to the file but 
            // dont have a reference here

            IList<Datacolumn> items = base.Items;
            bool match = false;
            foreach (Datacolumn item in items)
            {
                if (item.ColumnName == column.ColumnName)
                {
                    match = true;
                }
            }

            if ((match == false) || (items.Count == 0))
            {
                base.Add(column);
            }
            else
            {
                throw new DuplicateNameException();
            }
        }
    }
}
