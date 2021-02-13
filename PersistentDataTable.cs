using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Specialized;

namespace DataTable
{
    public class PersistentDataTable : IDisposable
    {
        // append the new pointer the index file

        // Try this
        //
        // Header
        //
        // 00 - unsigned int16 - number of records size
        // 00 - unsigned int16 - pointer to current record
        // 00 - unsigned int16 - pointer to data 
        //
        // Field
        // 
        // 0 - unsigned byte - number of fields (0-255)
        //
        // There is some concept of default values which means they would need storing or have a flag
        // but assume we are only storing
        //
        //   0 - null - 
        //   1 - string - default to ""
        //   2 - bool - defaul to false
        //   3 - int - 32 bit - default to zero
        //   4 - double - deafult to zero
        //   5 - blob - default to null
        //
        // 0 - unsigned byte - Field type enum value (0-255)
        // 0 - unsigned byte - If string or blob set the length (0-255)
        // 0 - unsigned byte - Primary key 0 - No, 1 - Yes (0-1)
        // 00 - leb128 - Length of element handled by the binary writer and reader in LEB128 format
        // bytes - string
        // ...
        // The structure repeats
        // 
        // Data
        //
        //  1 - string
        //  00 - leb128 - Length of element handled by the binary writer and reader in LEB128 format
        //  bytes - string
        //
        //  2 - bool
        //  0 - unsigned byte - 0 = false, 1 = true
        // 
        //  3 - int
        //  0000 - 32 bit - defaults to zero
        // 
        //  4 - double
        //  000000000 - 64 bit - 
        //
        //  5 - blob
        //  0000 - unsigned int32 - Lenght of blob
        //  bytes - data
        //
        // Index
        //
        // 00 - unsigned int16 - pointer to data
        // 00 - unsigned int16 - length of data
        // ...
        // 00 - unsigned int16 - pointer to data + 1 
        // 00 - unsigned int16 - length of data + 1
        //


        #region Variables

        private string _path = "";
        private string _name = "PersistentDataTable";

        private ObservableCollection<DataColumn> _columns;
        private ObservableCollection<DataRow> _rows;

        private readonly object _lockObject = new Object();
        private UInt16 _size = 0;
        private UInt16 _pointer = 7;
        private UInt16 _data = 7;
        private byte _records = 0;
        private int _cursor;
        private bool disposedValue;

        #endregion
        #region Constructors 

        public PersistentDataTable()
        {
            _columns = new ObservableCollection<DataColumn>();
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows = new ObservableCollection<DataRow>();
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
            Open(_path, _name, false);
        }

        public PersistentDataTable(bool reset)
        {
            _columns = new ObservableCollection<DataColumn>();
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows = new ObservableCollection<DataRow>();
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
            Open(_path, _name, reset);
        }

        public PersistentDataTable(string name)
        {
            _name = name;
            _columns = new ObservableCollection<DataColumn>();
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows = new ObservableCollection<DataRow>();
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
            Open(_path, _name, false);
        }

        public PersistentDataTable(string path, string filename)
        {
            _path = path;
            _name = filename;
            _columns = new ObservableCollection<DataColumn>();
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows = new ObservableCollection<DataRow>();
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
            Open(_path, _name, false);
        }

        public PersistentDataTable(string path, string filename, bool reset)
        {
            _path = path;
            _name = filename;
            _columns = new ObservableCollection<DataColumn>();
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows = new ObservableCollection<DataRow>();
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
            Open(_path, _name, reset);
        }

        #endregion
        #region Properties

        public ObservableCollection<DataColumn> Columns
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

        public ObservableCollection<DataRow> Rows
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

        #endregion
        #region Methods



        //public void Add(DataRow row)
        //{

        //}


        public DataRow NewRow()
        {
            DataRow dr = new DataRow(this);
            dr.ItemArray = new object[_columns.Count];
            for (int i = 0; i < _columns.Count; i++)
            {
                // Set the default value for the type
                DataColumn column = _columns[i];
                if (column.DataType.IsValueType == true)
                {
                    dr.ItemArray[i] = Activator.CreateInstance(column.DataType);
                }
                else
                {
                    dr.ItemArray[i] = null;
                }
            }
            return (dr);
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
                    // TODO: dispose managed state (managed objects)
                }
                disposedValue = true;
            }
        }

        #endregion
        #region Private

        private void _columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Need to add the new column to the header
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(DataColumn column in e.NewItems)
                {
                    Add(_path, _name, column);
                }
            }
        }

        private void _row_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Need to add the new column to the header
        }

        private void Add(string path, string filename, DataColumn column)
        {
            string filenamePath = System.IO.Path.Combine(path, filename);
            lock (_lockObject)
            {   
                BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".dbf", FileMode.Open));
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
                _size = binaryReader.ReadUInt16();
                _pointer = binaryReader.ReadUInt16();
                _data = binaryReader.ReadUInt16();
                _records = binaryReader.ReadByte();
                binaryReader.Close();
                binaryReader.Dispose();

                // need to calculate the spece needed
                //
                // Field
                //
                // 0 - unsigned byte - Field type enum value (0-255)
                // 0 - unsigned byte - If string or blob set the length (0-255)
                // 0 - unsigned byte - Primary key 0 - No, 1 - Yes (0-1)
                // 00 - leb128 - Length of element handled by the binary writer and reader in LEB128 format
                // bytes - string
                // ...
                // The structure repeats
                //
                // Typecode
                /*
                 *  Boolean	    3	A simple type representing Boolean values of true or false.
                    Byte	    6	An integral type representing unsigned 8-bit integers with values between 0 and 255.
                    Char	    4	An integral type representing unsigned 16-bit integers with values between 0 and 65535. The set of possible values for the Char type corresponds to the Unicode character set.
                    DateTime	16  A type representing a date and time value.
                    DBNull	    2   A database null (column) value.
                    Decimal	    15  A simple type representing values ranging from 1.0 x 10 -28 to approximately 7.9 x 10 28 with 28-29 significant digits.
                    Double	    14  A floating point type representing values ranging from approximately 5.0 x 10 -324 to 1.7 x 10 308 with a precision of 15-16 digits.
                    Empty	    0   A null reference.
                    Int16	    7   An integral type representing signed 16-bit integers with values between -32768 and 32767.
                    Int32	    9	An integral type representing signed 32-bit integers with values between -2147483648 and 2147483647.
                    Int64	    11  An integral type representing signed 64-bit integers with values between -9223372036854775808 and 9223372036854775807.
                    Object	    1   A general type representing any reference or value type not explicitly represented by another TypeCode.
                    SByte	    5   An integral type representing signed 8-bit integers with values between -128 and 127.
                    Single	    13  A floating point type representing values ranging from approximately 1.5 x 10 -45 to 3.4 x 10 38 with a precision of 7 digits.
                    String	    18  A sealed class type representing Unicode character strings.
                    UInt16	    8   An integral type representing unsigned 16-bit integers with values between 0 and 65535.
                    UInt32	    10  An integral type representing unsigned 32-bit integers with values between 0 and 4294967295.
                    UInt64	    12  An integral type representing unsigned 64-bit integers with values between 0 and 18446744073709551615.
                 */


                TypeCode typeCode = Type.GetTypeCode(column.DataType);
                int offset = 0;
                int l = column.ColumnName.Length;
                offset = offset + 3 + LEB128.Size(l) + l;

                // move the data

                // Add the new field

                BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.Open));
                binaryWriter.Seek(_data, SeekOrigin.Begin);

                binaryWriter.Write((byte)typeCode);             // write the field Type
                binaryWriter.Write((byte)column.MaxLength);     // write the field Length
                if (column.Primary == true)                     // write the primary key indicator (byte)
                {   
                    binaryWriter.Write((byte)1);
                }
                else
                {
                    binaryWriter.Write((byte)0);
                }
                binaryWriter.Write(column.ColumnName);             // Write the field Name

                _pointer = (UInt16)(_pointer + offset);
                _data = (UInt16)(_data + offset);
                _records = (byte)(_records + 1);

                binaryWriter.Seek(0, SeekOrigin.Begin);
                binaryWriter.Write(_size);                  // Write the number of records - size
                binaryWriter.Write(_pointer);               // Write pointer to new current record
                binaryWriter.Write(_data);                  // Write pointer to new data area
                binaryWriter.Write(_records);               // write new number of records
                binaryWriter.Close();
                binaryWriter.Dispose();
            }
        }

        private void Remove(string path, string filename)
        { }


        private void Open(string path, string filename, bool reset)
        {
            string filenamePath = System.IO.Path.Combine(path, filename);
            if ((File.Exists(filenamePath + ".dbf") == true) && (reset == false))
            {
                // Assume we only need to read the data and not the index

                BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".dbf", FileMode.Open));
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);          // Move to position of the current
                _size = (UInt16)binaryReader.ReadInt16();                   // Read in the data pointer
                _pointer = (UInt16)binaryReader.ReadInt16();                // Read in the current record
                _data = (UInt16)binaryReader.ReadInt16();                   // Read in the data pointer
                _records = binaryReader.ReadByte();

                _columns.Clear();
                for (int count = 0; count < _records; count++)
                {
                    TypeCode typeCode = (TypeCode)binaryReader.ReadByte();  // Read the field Type
                    int length = binaryReader.ReadByte();                   // Read the field Length
                    bool primary =false;                                    // Read if the primary key
                    if (binaryReader.ReadByte() == 1)
                    {
                        primary = true;
                    }
                    string name = binaryReader.ReadString();                // Read the field Name
                    DataColumn field = new DataColumn(name);
                    field.DataType = Type.GetType("System." + Enum.GetName(typeof(TypeCode), typeCode));
                    field.MaxLength = length;
                    field.Primary = primary;
                    _columns.Add(field);
                }
                binaryReader.Close();
                binaryReader.Dispose();
            }
            else
            {
                // Need to delete both data and index
                File.Delete(filenamePath + ".dbf");
                // Assumption here is the the index also exists
                File.Delete(filenamePath + ".idx");
                Reset(path, filename);
            }
        }

        private void Reset(string path, string filename)
        {
            // Reset the file
            string filenamePath = System.IO.Path.Combine(path, filename);
            BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.OpenOrCreate));
            binaryWriter.Seek(0, SeekOrigin.Begin); // Move to start of the file
            _size = 0;
            _pointer = 7;                           // Start of the data 2 x 16 bit
            _data = 7;
            _records = 0;

            binaryWriter.Write(_size);                  // Write the number of records - size
            binaryWriter.Write(_pointer);               // Write pointer to new current record
            binaryWriter.Write(_data);                  // Write pointer to new data area
            binaryWriter.Write(_records);               // write new number of records
            binaryWriter.BaseStream.SetLength(7);       // Fix the size as we are resetting
            binaryWriter.Close();

            // Create the index

            binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".idx", FileMode.OpenOrCreate));
            binaryWriter.BaseStream.SetLength(0);
            binaryWriter.Close();

        }

        //private object Read(string path, string filename, int index)
        //{
        //    KeyValuePair<TKey, TValue> keyValue;
        //    lock (_lockObject)
        //    {

        //        Type keyParameterType = typeof(TKey);
        //        Type valueParameterType = typeof(TValue);

        //        string filenamePath = System.IO.Path.Combine(path, filename);
        //        // Need to search the index file

        //        BinaryReader indexReader = new BinaryReader(new FileStream(filenamePath + ".idx", FileMode.Open));
        //        BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".bin", FileMode.Open));
        //        indexReader.BaseStream.Seek(index * 4, SeekOrigin.Begin);                               // Get the pointer from the index file
        //        UInt16 pointer = indexReader.ReadUInt16();                                              // Reader the pointer from the index file
        //        binaryReader.BaseStream.Seek(pointer, SeekOrigin.Begin);                                // Move to the correct location in the data file
                
        //        byte flag = binaryReader.ReadByte();
        //        object key = null;
        //        if (keyParameterType == typeof(int))
        //        {
        //            key = binaryReader.ReadInt32();
        //        }
        //        else if (keyParameterType == typeof(string))
        //        {
        //            key = binaryReader.ReadString();
        //        }
        //        else
        //        {
        //            key = default(TValue);
        //        }

        //        object value = null;
        //        if (valueParameterType == typeof(int))
        //        {
        //            value = binaryReader.ReadInt32();
        //        }
        //        else if (valueParameterType == typeof(string))
        //        {
        //            value = binaryReader.ReadString();
        //        }
        //        else
        //        {
        //            value = default(TValue);
        //        }

        //        keyValue = new KeyValuePair<TKey, TValue>((TKey)key, (TValue)value);

        //        binaryReader.Close();
        //        indexReader.Close();
        //    }
        //    return (keyValue);
        //}

    //    private void Write(string path, string filename, int index, object item)
    //    {
    //        lock (_lockObject)
    //        {
    //            Type keyParameterType = typeof(TKey);
    //            Type valueParameterType = typeof(TValue);

    //            string filenamePath = System.IO.Path.Combine(path, filename);

    //            // Write the data

    //            // Appending will only work if the file is deleated and the updates start again
    //            // Not sure if this is the best approach.
    //            // With strings might have to do the write first and then update the pointer.

    //            BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.Append));

    //            int offset = 0;
    //            offset = offset + 1;    // Including the flag

    //            if (keyParameterType == typeof(int))
    //            {
    //                offset = offset + 4;
    //            }
    //            else if (keyParameterType == typeof(string))
    //            {
    //                int l = (UInt16)Convert.ToString(item).Length;
    //                offset = offset + LEB128.Size(l) + l; 			// Includes the byte length parameter
    //                                                                // ** need to watch this as can be 2 bytes if length is > 127 characters
    //                                                                // ** https://en.wikipedia.org/wiki/LEB128
    //            }

    //            if (valueParameterType == typeof(int))
    //            {
    //                offset = offset + 4;
    //            }
    //            else if (valueParameterType == typeof(string))
    //            {
    //                int l = (UInt16)Convert.ToString(item).Length;
    //                offset = offset + LEB128.Size(l) + l;           // Includes the byte length parameter
    //                                                                // ** need to watch this as can be 2 bytes if length is > 127 characters
    //                                                                // ** https://en.wikipedia.org/wiki/LEB128

    //                string s = Convert.ToString(item);
    //                binaryWriter.Write(s);
    //            }

    //            byte flag = 0;
    //            binaryWriter.Write(flag);
    //            if (keyParameterType == typeof(string))
    //            {
    //                string s = Convert.ToString(item);
    //                binaryWriter.Write(s);
    //            }


    //            binaryWriter.Close();

    //            // Write the header

    //            binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".bin", FileMode.OpenOrCreate));
    //            binaryWriter.Seek(0, SeekOrigin.Begin); // Move to start of the file
    //            _size++;
    //            binaryWriter.Write(_size);                  // Write the size
    //            binaryWriter.Write((UInt16)(_pointer + offset));               // Write the pointer
    //            binaryWriter.Close();

    //            // need to insert the ponter as a new entry in the index

    //            FileStream stream = new FileStream(filenamePath + ".idx", FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    //            BinaryReader indexReader = new BinaryReader(stream);
    //            BinaryWriter indexWriter = new BinaryWriter(stream);

    //            UInt16 position;
    //            for (int counter = _size - 1; counter > index; counter--)
    //            {
    //                position = (UInt16)((counter - 1) * 4);
    //                indexReader.BaseStream.Seek(position, SeekOrigin.Begin);       // Move to location of the index
    //                UInt16 pointer = indexReader.ReadUInt16();                              // Read the pointer from the index file
    //                UInt16 off = indexReader.ReadUInt16();
    //                position = (UInt16)(counter * 4);
    //                indexWriter.Seek(counter * 4, SeekOrigin.Begin);                        // Move to location of the index
    //                indexWriter.Write(pointer);
    //                indexWriter.Write(off);
    //            }
    //            position = (UInt16)(index * 4);
    //            indexWriter.Seek(position, SeekOrigin.Begin);                        // Move to location of the index
    //            indexWriter.Write(_pointer);
    //            indexWriter.Write((UInt16)offset);
    //            indexWriter.Close();
    //            indexReader.Close();
    //            stream.Close();
    //        }
    //    }

    #endregion
    }

    public static class LEB128
    {
        public static byte[] Encode(int value)
        {
            byte[] data = new byte[5];  // Assume 32 bit max as its an int32
            int size = 0;
            do
            {
                byte byt = (byte)(value & 0x7f);
                value >>= 7;
                if (value != 0)
                {
                    byt = (byte)(byt | 128);
                }
                data[size] = byt;
                size = size + 1;
            } while (value != 0);
            return (data);
        }

        public static int Size(int value)
        {
            int size = 0;
            do
            {
                byte byt = (byte)(value & 0x7f);
                value >>= 7;
                size = size + 1;
            } while (value != 0);
            return (size);
        }
    }
}
