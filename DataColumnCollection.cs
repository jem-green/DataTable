using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Data;

namespace DataTable
{
    public class DataColumnCollection : ObservableCollection<DataColumn>
    {
        #region Variables
        DataHandler _handler;
        #endregion
        #region Constructors
        public DataColumnCollection(DataHandler handler)
        {
            _handler = handler;
        }
        #endregion

        public new void Add(DataColumn column)
        {
            // Actually need to add the column to the file but 
            // dont have a reference here, testing out using
            // the collection changed event to trigger the update
            // to occur in the calling class.

            IList<DataColumn> items = base.Items;
            bool match = false;
            foreach (DataColumn item in items)
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
