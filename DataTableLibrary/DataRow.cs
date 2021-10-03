using System;
using System.Collections.Generic;
using System.Text;

namespace DataTableLibrary
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
            _items = new object[_handler.Items];
            for (int item = 0; item < _handler.Items; item++)
            {
                // Set the default value for the type
                Type type = Type.GetType("System." + Enum.GetName(typeof(TypeCode), _handler.Fields[item].Type));
                if (type.IsValueType == true)
                {
                    _items[item] = Activator.CreateInstance(type);
                }
                else
                {
                    _items[item] = null;
                }
            }
        }

        public DataRow(PersistentDataTable table)
        {
            _table = table;
            _handler = _table.Handler;
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
                    for (int index = 0; index < _handler.Items; index++)
                    {
                        if (_handler.Get(index).Name == columnName)
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
                    for (int index = 0; index < _handler.Items; index++)
                    {
                        if (_handler.Get(index).Name == columnName)
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
                    // The behavior seems to be to convert the data into the correct data type

                    if (_table.Columns[columnIndex].DataType == value.GetType())
                    {
                        // This is where we need to trap the string field length
                        if (_table.Columns[columnIndex].DataType.GetType() == typeof(string))
                        {
                            sbyte length = _table.Columns[columnIndex].MaxLength;
                            if (length > 0)
                            {
                                string s = value.ToString().PadRight(length, '\0');
                                s = s.Substring(0, length);
                                _items[columnIndex] = value;
                            }
                            else
                            {
                                _items[columnIndex] = value;
                            }
                        }
                        else
                        {
                            _items[columnIndex] = value;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (_table.Columns[columnIndex].DataType.GetType() == typeof(string))
                            {
                                sbyte length = _table.Columns[columnIndex].MaxLength;
                                if (length > 0)
                                {
                                    string s = value.ToString().PadRight(length, '\0');
                                    s = s.Substring(0, length);
                                    _items[columnIndex] = value;
                                }
                                else
                                {
                                    _items[columnIndex] = value;
                                }
                            }
                            else
                            {
                                _items[columnIndex] = Convert.ChangeType(value, _table.Columns[columnIndex].DataType);
                            }
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
                if ((columnIndex < 0) || (columnIndex > _handler.Items))
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
                if ((columnIndex < 0) || (columnIndex > _handler.Items))
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    if (_handler.Fields[columnIndex].Type == Type.GetTypeCode(value.GetType()))
                    {
                        _items[columnIndex] = value;
                    }
                    else
                    {
                        try
                        {
                            _items[columnIndex] = Convert.ChangeType(value, _handler.Fields[columnIndex].Type);
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
        public void Delete()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
