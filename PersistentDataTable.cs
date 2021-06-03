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
            _handler = new DataHandler(_path, _name);
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
            //_columns = _handler.Items;
        }

        public PersistentDataTable(bool reset)
        {
            _handler = new DataHandler(_path, _name);
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
        }

        public PersistentDataTable(string name)
        {
            _handler = new DataHandler(_path, _name);
            _name = name;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
        }

        public PersistentDataTable(string name, bool reset)
        {
            _handler = new DataHandler(_path, _name);
            _name = name;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(reset);
        }

        public PersistentDataTable(string path, string filename)
        {
            _handler = new DataHandler(_path, _name);
            _path = path;
            _name = filename;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
        }

        public PersistentDataTable(string path, string filename, bool reset)
        {
            _handler = new DataHandler(_path, _name);
            _path = path;
            _name = filename;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            _handler.Open(false);
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

        public DataRow NewRow()
        {
            DataRow dr = new DataRow(this);
            dr.ItemArray = new object[_columns.Count];
            for (int item = 0; item < _columns.Count; item++)
            {
                // Set the default value for the type
                DataColumn column = _columns[item];
                if (column.DataType.IsValueType == true)
                {
                    dr.ItemArray[item] = Activator.CreateInstance(column.DataType);
                }
                else
                {
                    dr.ItemArray[item] = null;
                }
            }
            return (dr);
        }

        public DataRow[] Select()
        {
            // this might be the slow approach

            int records = _handler.Records;
            DataRow[] rows = new DataRow[records];
            for (int index=0; index < records; index++)
            {
                rows[index] = _handler.Read(index);
            }
            return (rows);
        }

        public void Select(string filterExpression)
        {

        }

        public void Select(string filterExpression ,string sort)
        {

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
                    _handler.Close();
                }
                disposedValue = true;
            }
        }

        #endregion
        #region Private
        #endregion
    }
}
