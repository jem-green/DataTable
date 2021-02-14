using System;
using System.Collections.Generic;
using System.Text;

namespace DataTable
{
    public class DataColumn
    {
        #region Variables
        private string _name;
        private Type _type;
        private int _length = -1;       // Default no maximum
        private bool _primary = false;
        
        #endregion
        #region Constructors
        public DataColumn(string columnName)
        {
            _name = columnName;
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
        public int MaxLength
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
        #endregion
    }
}
