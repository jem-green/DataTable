using System;
using System.Collections.Generic;
using System.Text;

namespace DataTable
{
    public class DataRow
    {
        #region Variables
        
        private object[] _items;
        private PersistentDataTable _table;
        private DataHandler _handler;

        #endregion
        #region Constructors

        public DataRow(DataHandler handler)
        {
            _handler = handler;
        }

        public DataRow(PersistentDataTable table)
        {
            _table = table;
        }

        #endregion
        #region Properties

        public PersistentDataTable Table
        {
            get
            {
                return (_table);
            }
        }

        public object this[string columnName]
        {
            get
            {
                int columnIndex = -1;
                if (_table == null)
                {
                    for (int index = 0; index < _handler.Fields; index++)
                    {
                        if (_handler.Get(index).ColumnName == columnName)
                        {
                            columnIndex = index;
                            break;
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < _table.Columns.Count; index++)
                    {
                        if (_table.Columns[index].ColumnName == columnName)
                        {
                            columnIndex = index;
                            break;
                        }
                    }
                }
            
                if (columnIndex < 0)
                {
                    throw new ArgumentException();
                }
                else
                {
                    return (_items[columnIndex]);
                }
            }

            set
            {
                int columnIndex = -1;
                if (_table == null)
                {
                    for (int index = 0; index < _handler.Fields; index++)
                    {
                        if (_handler.Get(index).ColumnName == columnName)
                        {
                            columnIndex = index;
                            break;
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < _table.Columns.Count; index++)
                    {
                        if (_table.Columns[index].ColumnName == columnName)
                        {
                            columnIndex = index;
                            break;
                        }
                    }
                }

                if (columnIndex < 0)
                {
                    throw new ArgumentException();
                }
                else
                {
                    // The behavior seesm to be to convert the data into the correct data type

                    if (_table.Columns[columnIndex].DataType == value.GetType())
                    {
                        _items[columnIndex] = value;
                    }
                    else
                    {
                        try
                        {
                            _items[columnIndex] = Convert.ChangeType(value, _table.Columns[columnIndex].DataType);
                        }
                        catch
                        {
                            throw new FieldAccessException();
                        }
                    }
                }
            }
        }

        public object this[int columnIndex]
        {
            get
            {
                if ((columnIndex < 0) || (columnIndex > _table.Columns.Count))
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    return (_items[columnIndex]);
                }
            }

            set
            {
                if ((columnIndex < 0) || (columnIndex > _table.Columns.Count))
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    if (_table.Columns[columnIndex].DataType == value.GetType())
                    {
                        _items[columnIndex] = value;
                    }
                    else
                    {
                        try
                        {
                            _items[columnIndex] = Convert.ChangeType(value, _table.Columns[columnIndex].DataType);
                        }
                        catch
                        {
                            throw new FieldAccessException();
                        }
                    }

                }
            }
        }

        public object[] ItemArray
        {
            set
            {
                _items = value;
            }
            get
            {
                return (_items);
            }
        }

        #endregion
        #region Methods
        public void Add()
        { }
        #endregion
    }
}
