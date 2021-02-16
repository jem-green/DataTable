using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataTable
{
    public class Datacolumn : IComparable, IComparer, IEqualityComparer
    {
        #region Variables
        private string _name;
        private Type _type;
        private sbyte _length = -1;           // Default no maximum
        private bool _primary = false;      //

        #endregion
        #region Constructors

        public Datacolumn()
        {
        }

        public Datacolumn(string columnName)
        {
            _name = columnName;
        }

        public Datacolumn(string columnName, Type dataType)
        {
            _name = columnName;
            _type = dataType;
        }

        public Datacolumn(string columnName, Type dataType, string expr)
        {
            _name = columnName;
            _type = dataType;
        }
        
        public Datacolumn(string columnName, Type dataType, string expr, MappingType type)
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
            Datacolumn c1 = (Datacolumn)x;
            Datacolumn c2 = (Datacolumn)y;
            return (String.Compare(c1._name, c2.ColumnName));
        }

        public new bool Equals(object x, object y)
        {
            bool match = false;
            Datacolumn c1 = (Datacolumn)x;
            Datacolumn c2 = (Datacolumn)y;
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
            Datacolumn c = (Datacolumn)obj;
            return(String.Compare(this._name, c.ColumnName));
        }

        #endregion
    }
}
