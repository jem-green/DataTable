using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Data;
using System.Collections;

namespace DataTable
{
    public class DataColumnCollection : ICollection<DataColumn>, IEnumerator<DataColumn>, IList<DataColumn>
    {
        #region Variables

        private DataHandler _handler;
        private int _cursor;
        private bool disposedValue;
        
        #endregion
        #region Constructors
        
        public DataColumnCollection(DataHandler handler)
        {
            _handler = handler;
        }
        
        #endregion
        #region Properties
        
        public bool IsSynchronized
        {
            get
            {
                return (true);
            }
        }

        public int Count
        {
            get
            {
                return (_handler.Items);
            }
        }

        public DataColumn this[int index]
        {
            get
            {
                if ((index < 0) || (index > _handler.Items))
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    return (_handler.Get(index));
                }
            }
            set
            {
                if ((index < 0) || (index > _handler.Items))
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    _handler.Set(value, index);
                }
            }
        }

        public object SyncRoot => throw new NotImplementedException();

        public object Current => throw new NotImplementedException();
        public bool IsReadOnly
        {
            get
            {
                return (false);
            }
        }

        #endregion
        #region Methods

        /// <summary>
        /// Clear the Collection
        /// </summary>
        public void Clear()
        {
            _handler.Reset();
        }

        /// <summary>
        /// Add a new item at the end of the list
        /// </summary>
        /// <param name="item"></param>
        public void Add(DataColumn column)
        {
            bool match = false;
            for (int item = 0; item<_handler.Items; item++)
            {
                if (_handler.Get(item).ColumnName == column.ColumnName)
                {
                    match = true;
                }
            }

            if ((match == false) || (_handler.Items == 0))
            {
                _handler.Add(column);
            }
            else
            {
                throw new DuplicateNameException();
            }
        }

        public bool Remove(DataColumn column)
        {
            bool removed = false;
            bool match = false;
            for (int item = 0; item < _handler.Items; item++)
            {
                if (_handler.Get(item).ColumnName == column.ColumnName)
                {
                    match = true;
                }
            }

            if (match == true)
            {
                _handler.Remove(column);
                removed = true;
            }
            else
            {
                throw new ArgumentException();
            }
            return (removed);
        }

        public void RemoveAt(int index)
        {
            throw new IndexOutOfRangeException();
        }

        public void Insert(int index, DataColumn column)
        { 
            bool match = false;
            for (int item = 0; item < _handler.Items; item++)
            {
                if (_handler.Get(item).ColumnName == column.ColumnName)
                {
                    match = true;
                }
            }

            if (match == true)
            {
                _handler.Add(column);
            }
            else
            {
                throw new DuplicateNameException();
            }
        }

        bool IEnumerator.MoveNext()
        {
            bool moved = false;
            if (_cursor < _handler.Items)
            {
                moved = true;
            }
            return (moved);
        }

        void IEnumerator.Reset()
        {
            _cursor = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                if ((_cursor < 0) || (_cursor == _handler.Items))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return (_handler.Get(_cursor));
                }
            }
        }

        DataColumn IEnumerator<DataColumn>.Current => throw new NotImplementedException();

        //DataColumn IEnumerator<DataColumn>.Current
        //{
        //    get
        //    {
        //        if ((_cursor < 0) || (_cursor == _handler.Fields))
        //        {
        //            throw new InvalidOperationException();
        //        }
        //        else
        //        {
        //            return ((T)Convert.ChangeType(Read(_path, _name, _cursor), typeof(T)));
        //        }
        //    }
        //}


        public void CopyTo(DataColumn[] columns, int index)
        {
            throw new NotImplementedException();
        }

        public bool Contains(DataColumn column)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<DataColumn> GetEnumerator()
        {
            for (int cursor = 0; cursor < _handler.Items; cursor++)
            {
            	//Return the current element and then on next function call 
                //resume from next element rather than starting all over again;
                yield return (_handler.Get(cursor));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PersistentQueue()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public int IndexOf(DataColumn item)
        {
            throw new NotImplementedException();
        }





        #endregion
        #region Private
        #endregion
    }
}
