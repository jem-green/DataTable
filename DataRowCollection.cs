using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Data;
using System.Collections;

namespace DataTable
{
    public class DataRowCollection : ObservableCollection<DataRow>, IEnumerator<DataRow>
    {
        #region Variables

        DataHandler _handler;
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

        public new int Count
        {
            get
            {
                return (_handler.Records);
            }
        }

        #endregion
        #region Methods

        public DataRow Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public bool MoveNext()
        {
            bool moved = false;
            if (_cursor < _handler.Records)
            {
                moved = true;
            }
            return (moved);
        }

        public void Reset()
        {
            throw new NotImplementedException();
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
