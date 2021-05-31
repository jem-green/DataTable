using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Data;
using System.Collections;

namespace DataTable
{
    public class DataRowCollection : ICollection<DataRow>, IEnumerator<DataRow>, IList<DataRow>
    {
        #region Variables

        private DataHandler _handler;
        private int _cursor;
        private bool disposedValue;

        #endregion
        #region Constructors
        public DataRowCollection(DataHandler handler)
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
                return (_handler.Records);
            }
        }

        public DataRow this[int index]
        {
            get
            {
                if ((index < 0) || (index > _handler.Records))
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    return (_handler.Read(index));
                }
            }
            set
            {
                if ((index < 0) || (index > _handler.Records))
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    _handler.Update(value,index);
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
        public void Add(DataRow row)
        {
            _handler.Create(row);
        }
		
		public bool Remove(DataRow row)
        {
            int itemIndex = -1;
            bool removed = false;
            for (int item = 0; item < _handler.Records; item++)
            {
                // This is much more complex as == is not based on
                // object references so they will never match.

                break;
            }

            if (itemIndex >= 0)
            {
                _handler.Delete(itemIndex);
                removed = true;
                return (removed);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void RemoveAt(int index)
        {
            throw new IndexOutOfRangeException();
        }

        public void Insert(int index, DataRow row)
        {
            if ((index < 0) || (index > _handler.Records))
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                _handler.Update(row, index);
            }
        }

        bool IEnumerator.MoveNext()
        {
            bool moved = false;
            if (_cursor < _handler.Records)
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
                if ((_cursor < 0) || (_cursor == _handler.Records))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return (_handler.Read(_cursor));
                }
            }
        }

        DataRow IEnumerator<DataRow>.Current => throw new NotImplementedException();

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

        public void CopyTo(DataRow[] rows, int index)
        {
            throw new NotImplementedException();
        }

        public bool Contains(DataRow row)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
		
        public IEnumerator<DataRow> GetEnumerator()
        {
            for (int cursor = 0; cursor < _handler.Records; cursor++)
            {
                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return (_handler.Read(cursor));
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
        // ~DataRowCollection()
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
		
        public int IndexOf(DataRow row)
        {
            throw new NotImplementedException();
        }





        #endregion
        #region Private
        #endregion
    }
}
