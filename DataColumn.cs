using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataTable
{
    public class DataColumn : IComparable, IComparer, IEqualityComparer
    {
        #region Variables
        private string _name;
        private Type _type;
        private sbyte _length = -1;           // Default no maximum
        private bool _primary = false;      //

        #endregion
        #region Constructors

        public DataColumn()
        {
        }

        public DataColumn(string columnName)
        {
            _name = columnName;
        }

        public DataColumn(string columnName, Type dataType)
        {
            _name = columnName;
            _type = dataType;
        }

        public DataColumn(string columnName, Type dataType, string expr)
        {
            _name = columnName;
            _type = dataType;
        }
        
        public DataColumn(string columnName, Type dataType, string expr, MappingType type)
        {
            _name = columnName;
            _type = dataType;
        }

        #endregion
        #region Properties

        public string ColumnName
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
        public Type DataType
        {
            set
            {
                _type = value;
            }

            get
            {
                return (_type);
            }
        }
        public sbyte MaxLength
        {
            set
            {
                _length = value;
            }

            get
            {
                return (_length);
            }
        }
        public bool Primary
        {
            set
            {
                _primary = value;
            }

            get
            {
                return (_primary);
            }
        }

        #endregion
        #region Methods

        public int Compare(object x, object y)
        {
            DataColumn c1 = (DataColumn)x;
            DataColumn c2 = (DataColumn)y;
            return (String.Compare(c1._name, c2.ColumnName));
        }

        public new bool Equals(object x, object y)
        {
            bool match = false;
            DataColumn c1 = (DataColumn)x;
            DataColumn c2 = (DataColumn)y;
            if (String.Compare(c1._name, c2.ColumnName) == 0)
            {
                match = true;
            }
            return (match);
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        int IComparable.CompareTo(object obj)
        {
            DataColumn c = (DataColumn)obj;
            return(String.Compare(this._name, c.ColumnName));
        }

        #endregion
    }
}
