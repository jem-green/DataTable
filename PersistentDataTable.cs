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

        private DataColumnCollection _columns;
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
            _columns = new DataColumnCollection();
            _rows = new ObservableCollection<DataRow>();
            Open(_path, _name, false);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);

        }

        public PersistentDataTable(bool reset)
        {
            _columns = new DataColumnCollection();
            _rows = new ObservableCollection<DataRow>();
            Open(_path, _name, reset);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        public PersistentDataTable(string name)
        {
            _name = name;
            _columns = new DataColumnCollection();
            _rows = new ObservableCollection<DataRow>();
            Open(_path, _name, false);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        public PersistentDataTable(string path, string filename)
        {
            _path = path;
            _name = filename;
            _columns = new DataColumnCollection();
            _rows = new ObservableCollection<DataRow>();
            Open(_path, _name, false);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        public PersistentDataTable(string path, string filename, bool reset)
        {
            _path = path;
            _name = filename;
            _columns = new DataColumnCollection();
            _rows = new ObservableCollection<DataRow>();
            Open(_path, _name, reset);
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
            _rows.CollectionChanged += new NotifyCollectionChangedEventHandler(_row_CollectionChanged);
        }

        #endregion
        #region Properties

        public DataColumnCollection Columns
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
                Datacolumn column = _columns[i];
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
                foreach(Datacolumn column in e.NewItems)
                {
                    Add(_path, _name, column);
                }
            }
        }

        private void _row_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Need to add the new column to the header
            // Need to add the new column to the header
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DataRow row in e.NewItems)
                {
                    Create(_path, _name, row);
                }
            }
        }

        private void Add(string path, string filename, Datacolumn column)
        {
            string filenamePath = System.IO.Path.Combine(path, filename);
            lock (_lockObject)
            {   
                //BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".dbf", FileMode.Open));
                //binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
                //_size = binaryReader.ReadUInt16();
                //_pointer = binaryReader.ReadUInt16();
                //_data = binaryReader.ReadUInt16();
                //_records = binaryReader.ReadByte();
                //binaryReader.Close();
                //binaryReader.Dispose();

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
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);      // Move to position of the current
                _size = binaryReader.ReadUInt16();                      // Read in the data pointer
                _pointer = binaryReader.ReadUInt16();                   // Read in the current record
                _data = binaryReader.ReadUInt16();                      // Read in the data pointer
                _records = binaryReader.ReadByte();                     // Read in the number of records

                _columns.Clear();
                for (int count = 0; count < _records; count++)
                {
                    TypeCode typeCode = (TypeCode)binaryReader.ReadByte();  // Read the field Type
                    sbyte length = binaryReader.ReadSByte();                // Read the field Length
                    bool primary =false;                                    // Read if the primary key
                    if (binaryReader.ReadByte() == 1)
                    {
                        primary = true;
                    }
                    string name = binaryReader.ReadString();                // Read the field Name
                    Datacolumn field = new Datacolumn(name);
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

        private void Write(string path, string filename, DataRow row)
        {
            string filenamePath = System.IO.Path.Combine(path, filename);
            lock (_lockObject)
            {
                // Write the data

                
            }
        }

        private void Create(string path, string filename, DataRow row)
        {
            string filenamePath = System.IO.Path.Combine(path, filename);
            lock (_lockObject)
            {
                // append the new pointer the new index file

                BinaryWriter indexWriter = new BinaryWriter(new FileStream(filenamePath + ".idx", FileMode.Append));
                indexWriter.Write(_pointer);  // Write the pointer


                int offset = 0;
                offset = offset + 1;    // Including the flag
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    object data = row.ItemArray[i];
                    Datacolumn dataColumn = _columns[i];
                    Type dataType = dataColumn.DataType;

                    if (dataType == typeof(int))
                    {
                        offset = offset + 4;
                    }
                    else if (dataType == typeof(string))
                    {
                        int length = dataColumn.MaxLength;
                        if (length < 0)
                        {
                            length = Convert.ToString(data).Length;
                        }
                        offset = offset + LEB128.Size(length) + length;     // Includes the byte length parameter
                                                                            // ** need to watch this as can be 2 bytes if length is > 127 characters
                                                                            // ** https://en.wikipedia.org/wiki/LEB128
                        
                    }
                }

                // Update the index length

                indexWriter.Write((UInt16)offset);  // Write the length
                indexWriter.Close();
                indexWriter.Dispose();

                // Write the header

                BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.OpenOrCreate));
                binaryWriter.Seek(0, SeekOrigin.Begin);                         // Move to start of the file
                _size++;                                                        // Update the size
                binaryWriter.Write(_size);                                      // Write the size
                binaryWriter.Write((UInt16)(_pointer + offset));                // Write the pointer
                binaryWriter.Close();
                binaryWriter.Dispose();

                // Write the data

                // Appending will only work if the file is deleted and the updates start again
                // Not sure if this is the best approach.
                // Need to update the 

                binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.Append));
                byte flag = 0;
                binaryWriter.Write(flag);
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    object data = row.ItemArray[i];
                    Datacolumn dataColumn = _columns[i];
                    Type dataType = dataColumn.DataType;

                    if (dataType == typeof(int))
                    {
                        binaryWriter.Write((int)data);
                    }
                    else if (dataType == typeof(string))
                    {
                        string text = Convert.ToString(data);

                        if (dataColumn.MaxLength < 0)
                        {
                            binaryWriter.Write(text);
                        }
                        else
                        {
                            if (text.Length > dataColumn.MaxLength)
                            {
                                text = text.Substring(0, dataColumn.MaxLength);
                            }
                            else
                            {
                                text = text.PadRight(dataColumn.MaxLength, '\0');
                            }
                            binaryWriter.Write(text);
                        }
                    }
                }
                binaryWriter.Close();
                binaryWriter.Dispose();
            }
        }

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
