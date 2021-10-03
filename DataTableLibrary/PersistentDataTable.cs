﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Specialized;

namespace DataTableLibrary
{
    public class PersistentDataTable : IDisposable
    {
        #region Fields

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
            if (_handler.Open() == false)
            {
                _handler.Reset();
            }

        }
        
        public PersistentDataTable(bool reset)
        {
            _handler = new DataHandler(_path, _name);
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            if (reset == true)
            {
                _handler.Reset();
            }
            if (_handler.Open() == false)
            {
                _handler.Reset();
            }
        }

        public PersistentDataTable(string name)
        {
            _handler = new DataHandler(_path, _name);
            _name = name;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            if (_handler.Open() == false)
            {
                _handler.Reset();
            }
        }

        public PersistentDataTable(string name, bool reset)
        {
            _handler = new DataHandler(_path, _name);
            _name = name;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            if (reset == true)
            {
                _handler.Reset();
            }
            if (_handler.Open() == false)
            {
                _handler.Reset();
            }
        }

        public PersistentDataTable(string path, string filename)
        {
            _handler = new DataHandler(_path, _name);
            _path = path;
            _name = filename;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            if (_handler.Open() == false)
            {
                _handler.Reset();
            }
        }

        public PersistentDataTable(string path, string filename, bool reset)
        {
            _handler = new DataHandler(_path, _name);
            _path = path;
            _name = filename;
            _columns = new DataColumnCollection(_handler);
            _rows = new DataRowCollection(_handler);
            if (reset == true)
            {
                _handler.Reset();
            }
            if (_handler.Open() == false)
            {
                _handler.Reset();
            }
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
            // of opening and closing the dbf and idx files

            int records = _handler.Size;
            DataRow[] rows = new DataRow[records];
            for (int index=0; index < records; index++)
            {
                DataRow row = new DataRow(_handler);
                row.ItemArray = _handler.Read(index);
                rows[index] = row;
            }
            return (rows);
        }

        public void Select(string filterExpression)
        {
            throw new NotImplementedException();
        }

        public void Select(string filterExpression ,string sort)
        {
            throw new NotImplementedException();
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
                    _handler.Close();   // Deletes the Persistant datatable
                }
                disposedValue = true;
            }
        }

        #endregion
        #region Private
        #endregion
    }
}
