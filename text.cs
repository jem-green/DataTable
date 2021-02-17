using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using DataTable;

namespace DataTable
{
    public class text
    {
        #region Variables

        // After a few challenges of using the ObservableCollection and intercpting the
        // Collection changes events and Add method as I want to read data into the collection
        // and also support adding columns to the collection. This seems to be the same method
        // So decided to try and replicate the behavior of the collection so that the Add method 
        // Add methods so that the 
        // Store a odered list of DataColums

        private DataColumn[] _columns;
        private bool _readOnly = false;
        private PersistentDataTable _table;

        #endregion
        #region Constructors
        public text(PersistentDataTable table)
        {
            _table = table;
            _columns = null;
        }

        #endregion
        #region Properties

        public int Count
        {
            get
            {
                return (_columns.Length);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return (_readOnly);
            }
            set
            {
                _readOnly = value;
            }
        }

        public DataColumn this[int index]
        {
            get
            {
                if ((index >= 0) && (index < _columns.Length))
                {
                    return (_columns[index]);
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            set
            {
                if ((index >= 0) && (index < _columns.Length))
                {
                    _columns[index] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        //Datacolumn IList<Datacolumn>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion
        #region Methods

        public ArrayList List
        {
            get
            {
                throw new NotImplementedException();
                return (new ArrayList());
            }
        }

        public DataColumn Add()
        {
            // Need to resize the Array
            DataColumn column = new DataColumn();
            Array.Resize(ref _columns, _columns.Length + 1);
            _columns[_columns.Length] = column;
            return (column);
        }

        public DataColumn Add(string columnName, Type type)
        {
            // Need to resize the Array
            DataColumn column = new DataColumn(columnName);
            column.DataType = type;
            // check if the column name exists
            if (Array.Exists(_columns, element => element.ColumnName == columnName) == false)
            {

                Array.Resize(ref _columns, _columns.Length + 1);
                _columns[_columns.Length] = column;
                return (column);
            }
            else
            {
                throw new DuplicateNameException();
            }

        }
        public DataColumn Add(string columnName)
        {
            return (Add(columnName,null));
        }

        public DataColumn Add(DataColumn column)
        {
            // Need to resize the Array
            Array.Resize(ref _columns, _columns.Length + 1);
            _columns[_columns.Length] = column;
            return (column);
        }

        public bool CanRemove(System.Data.DataColumn column)
        {
            return (true);
        }

        public void Clear()
        {
            Array.Resize(ref _columns, 0);
        }

        public bool Contains(DataColumn item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(DataColumn[] array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<DataColumn> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(string columnName)
        {
            throw new NotImplementedException();
        }
        public int IndexOf(DataColumn column)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, DataColumn item)
        {
            throw new NotImplementedException();
        }

        public void Remove(DataColumn column)
        {
            throw new NotImplementedException();
        }

        public void Remove(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public bool CanRemove(DataColumn column)
        {
            return (true);
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Contains(string name)
        //{
        //    return (true);
        //}

        //void ICollection<Datacolumn>.Add(Datacolumn item)
        //{
        //    throw new NotImplementedException();
        //}

        //bool ICollection<Datacolumn>.Remove(Datacolumn item)
        //{
        //    throw new NotImplementedException();
        //}

        public DataTable.DataColumn this[string name]
        {
            get
            {
                DataTable.DataColumn dataColumn = new DataTable.DataColumn(name);
                return (dataColumn);
            }
            set
            {
                // ToDo
            }
        }


        #endregion
    }
}
