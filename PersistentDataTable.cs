using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Specialized;

namespace DataTable
{
    public class PersistentDataTable : IDisposable
    {
         #region Variables

        private string _path = "";
        private string _name = "PersistentDataTable";

        private DataColumnCollection _columns;
        private DataRowCollection _rows;
        private DataHandler _handler;

        private int _cursor;
        private bool disposedValue;

        #endregion
        #region Constructors 

        public PersistentDataTable()
        {
            _handler = new DataHandler(_path,_name);
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
            //_columns = _handler.Items;
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        public PersistentDataTable(bool reset)
        {
            _handler = new DataHandler(_path, _name);
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        public PersistentDataTable(string name)
        {
            _handler = new DataHandler(_path, _name);
            _name = name;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        public PersistentDataTable(string name, bool reset)
        {
            _handler = new DataHandler(_path, _name);
            _name = name;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(reset);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        public PersistentDataTable(string path, string filename)
        {
            _handler = new DataHandler(_path, _name);
            _path = path;
            _name = filename;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        public PersistentDataTable(string path, string filename, bool reset)
        {
            _handler = new DataHandler(_path, _name);
            _path = path;
            _name = filename;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        #endregion
        #region Properties

        public DataColumnCollection Columns
        {
            set
            {
                _columns = value;
            }
            get
            {
                return (_columns);
            }
        }

        public DataRowCollection Rows
        {
            set
            {
                _rows = value;
            }
            get
            {
                return (_rows);
            }
        }

        public string TableName
        {
            set
            {
                _name = value;
            }
            get
            {
                return (_name);
            }
        }

        public DataHandler Handler
        {
            set
            {
                _handler = value;
            }
            get
            {
                return (_handler);
            }
        }

        #endregion
        #region Methods



        //public void Add(DataRow row)
        //{

        //}


        public DataRow NewRow()
        {
            DataRow dr = new DataRow(this);
            dr.ItemArray = new object[_columns.Count];
            for (int i = 0; i < _columns.Count; i++)
            {
                // Set the default value for the type
                DataColumn column = _columns[i];
                if (column.DataType.IsValueType == true)
                {
                    dr.ItemArray[i] = Activator.CreateInstance(column.DataType);
                }
                else
                {
                    dr.ItemArray[i] = null;
                }
            }
            return (dr);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                disposedValue = true;
            }
        }

        #endregion
        #region Private

        private void _columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Need to add the new column to the header
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DataColumn column in e.NewItems)
                {
                    _handler.Add(column);
                }
            }
        }

        private void _row_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Need to add the new column to the header
            // Need to add the new column to the header
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DataRow row in e.NewItems)
                {
                    _handler.Create(row);
                }
            }
        }

        #endregion
    }
}
