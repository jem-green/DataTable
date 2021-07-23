using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataTable
{
    public class DataHandler
    {
        /*
        Solve problem of passing the PersistentDataTable to the other
        classes, so nest the class 
        
        Header
        ------
        
        00 - unsigned int16 - number of records _size
        00 - unsigned int16 - pointer to current record _pointer
        00 - unsigned int16 - pointer to start of data _data
        
        Field
        -----
         
        0 - unsigned byte - number of fields (0-255) _items
        00 - unsigned int16 - pointer to current field _fieldPointer

        From the type definition
        
        Empty	    0 - A null reference.
        Object	    1 - A general type representing any reference or value type not explicitly represented by another TypeCode.
        DBNull	    2 - A database null (column) value.
        Boolean	    3 - A simple type representing Boolean values of true or false.
        Char	    4 - An integral type representing unsigned 16-bit integers with values between 0 and 65535. The set of possible values for the Char type corresponds to the Unicode character set.
        SByte	    5 - An integral type representing signed 8-bit integers with values between -128 and 127.
        Byte	    6 - An integral type representing unsigned 8-bit integers with values between 0 and 255.
        Int16	    7 - An integral type representing signed 16-bit integers with values between -32768 and 32767.
        UInt16	    8 - An integral type representing unsigned 16-bit integers with values between 0 and 65535.
        Int32	    9 - An integral type representing signed 32-bit integers with values between -2147483648 and 2147483647.
        UInt32	    10 - An integral type representing unsigned 32-bit integers with values between 0 and 4294967295.
        Int64	    11 - An integral type representing signed 64-bit integers with values between -9223372036854775808 and 9223372036854775807.
        UInt64	    12 - An integral type representing unsigned 64-bit integers with values between 0 and 18446744073709551615.
        Single	    13 - A floating point type representing values ranging from approximately 1.5 x 10 -45 to 3.4 x 10 38 with a precision of 7 digits.
        Double	    14 - A floating point type representing values ranging from approximately 5.0 x 10 -324 to 1.7 x 10 308 with a precision of 15-16 digits.
        Decimal	    15 - A simple type representing values ranging from 1.0 x 10 -28 to approximately 7.9 x 10 28 with 28-29 significant digits.
        DateTime	16 - A type representing a date and time value.
        ?
        String      18 - A sealed class type representing Unicode character strings.
         
        The offset is needed if the field is shortened.

        0 - unsigned byte - Offset assuming a field name is less than 255 characters (0-255)
        0 - unsigned byte - Flag 0 = normal, 1 = deleted, 2 = spare
        0 - unsigned byte - Field type enum value (0-255)
        0 - unsigned byte - If string or blob set the length (0-255)
        0 - unsigned byte - Primary key 0 - No, 1 - Yes (0-1)
        00 - LEB128 - Length of element handled by the binary writer and reader in LEB128 format
        bytes - string
        ...
        
        Data
        ----
        
        0 - unsigned byte - flag 0 = normal, 1 = deleted, 2 = spare
        
        1 - string
        00 - LEB128 - Length of element handled by the binary writer and reader in LEB128 format
        bytes - string
        
        2 - bool
        0 - unsigned byte - 0 = false, 1 = true
         
        3 - int
        0000 - 32 bit - defaults to zero
         
        4 - double
        000000000 - 64 bit - defaults to zero
        
        5 - blob
        0000 - unsigned int32 - Length of blob
        bytes - data
        
        The date repeats for each field, there is no record separator
        
        Index
        -----

        00 - unsigned int16 - pointer to data
        00 - unsigned int16 - length of data
        ...
        00 - unsigned int16 - pointer to data + 1 
        00 - unsigned int16 - length of data + 1
        
        */

        #region Fields

        private string _path = "";
        private string _name = "";

        private readonly object _lockObject = new Object();
        private UInt16 _size = 0;               // number of elements
        private readonly UInt16 _start = 7;     // Pointer to the start of the field area
        private UInt16 _pointer = 7;            // Pointer to current element
        private UInt16 _data = 7;               // pointer to start of data area

        private byte _items = 0;                // number of field items

        private Field[] _fields;                // Cache of fields

        internal struct Field
        {
            string _name;
            byte _flag;
            TypeCode _type;
            sbyte _length;
            bool _primary;

            internal Field(string name, byte flag, TypeCode type, sbyte length, bool primary)
            {
                _name = name;
                _flag = flag;
                _type = type;
                _length = length;
                _primary = primary;
            }

            internal byte Flag
            {
                set
                {
                    _flag = value;
                }
                get
                {
                    return (_flag);
                }
            }

            internal sbyte Length
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

            internal string Name
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
            internal bool Primary
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

            internal TypeCode Type
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
        }


        #endregion
        #region Constructor

        public DataHandler(string path, string name)
        {
            _path = path;
            _name = name;
        }

        #endregion
        #region Properties

        public string Path
        {
            set
            {
                _path = value;
            }
            get
            {
                return (_path);
            }
        }

        public string Name
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

        public UInt16 Records
        {
            get
            {
                return (_size);
            }
        }

        public byte Items
        {
            get
            {
                return (_items);
            }
        }

        internal Field[] Fields
        {
            get
            {
                return (_fields);
            }
        }

        //internal ArrayList List
        //{
        //    get
        //    {
        //        return (new ArrayList(_fields));
        //    }
        //}

        #endregion
        #region Methods

        // General methods
        // Open -
        // Close - 
        // Reset -
        //
        // Column methods
        // Add -
        // Remove -
        // Set -
        // Get -
        // 
        // Row methods (CRUD) 
        // Create -
        // Read -
        // Update -
        // Delete - 

        #region General Methods

        /// <summary>
        /// Open an existing database file or create a new file
        /// </summary>
        /// <param name="reset"></param>
        internal void Open(bool reset)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            if ((File.Exists(filenamePath + ".dbf") == true) && (reset == false))
            {
                // Assume we only need to read the data and not the index

                BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".dbf", FileMode.Open));
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);      // Move to position of the current
                _size = binaryReader.ReadUInt16();                      // Read in the size of data
                _pointer = binaryReader.ReadUInt16();                   // Read in the current record
                _data = binaryReader.ReadUInt16();                      // Read in the data pointer
                _items = binaryReader.ReadByte();                       // Read in the number of fields

                Array.Resize(ref _fields, _items);
                UInt16 pointer = _start;
                for (int count = 0; count < _items; count++)
                {
                    binaryReader.BaseStream.Seek(pointer, SeekOrigin.Begin);    // Move to the field as may have been updated
                    byte offset = binaryReader.ReadByte();                      // Read the field offset
                    byte flag = binaryReader.ReadByte();                        // Read the status flag
                    TypeCode typeCode = (TypeCode)binaryReader.ReadByte();      // Read the field Type
                    sbyte length = binaryReader.ReadSByte();                    // Read the field Length
                    bool primary = false;                                       // Read if the primary key
                    if (binaryReader.ReadByte() == 1)
                    {
                        primary = true;
                    }
                    string name = binaryReader.ReadString();                    // Read the field Name
                    if (flag == 0)  // Not deleted or spare
                    {
                        Field field = new Field(name, flag, typeCode, length, primary);
                        _fields[count] = field;
                    }
                    pointer = (UInt16)(pointer + offset);
                }
                binaryReader.Close();
            }
            else
            {
                // Need to delete both data and index
                File.Delete(filenamePath + ".dbf");
                // Assumption here is the the index also exists
                File.Delete(filenamePath + ".idx");
                Reset();
            }
        }
        
        /// <summary>
        /// Reset the database file and clear any index files
        /// </summary>
        internal void Reset()
        {
            // Reset the file
            string filenamePath = System.IO.Path.Combine(_path, _name);
            BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.OpenOrCreate));
            binaryWriter.Seek(0, SeekOrigin.Begin); // Move to start of the file

            _size = 0;
            _pointer = _start;                     // Start of the data 3 x 16 bit + 1 x 8 bit
            _data = _start;
            _items = 0;

            binaryWriter.Write(_size);                  // Write the size of data
            binaryWriter.Write(_pointer);               // Write pointer to new current record
            binaryWriter.Write(_data);                  // Write pointer to new data area
            binaryWriter.Write(_items);                 // write new number of fields
            binaryWriter.BaseStream.SetLength(7);       // Fix the size as we are resetting
            binaryWriter.Close();

            // Create the index

            binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".idx", FileMode.OpenOrCreate));
            binaryWriter.BaseStream.SetLength(0);
            binaryWriter.Close();
        }

        /// <summary>
        /// Delete the datatable file and index file
        /// </summary>
        internal void Close()
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);

            if (File.Exists(filenamePath + ".dbf") == true)
            {
                // Need to delete both data and index
                File.Delete(filenamePath + ".dbf");
                // Assumption here is the the index also exists
                File.Delete(filenamePath + ".idx");
            }
        }

        #endregion
        #region Column methods

        /// <summary>
        /// Add a new DataColumn
        /// Not sure if we need this helper methods as ties us to DataColumns
        /// </summary>
        /// <param name="column"></param>
        public void Add(DataColumn column)
        {
            Field field = new Field(column.ColumnName, column.Flag, Type.GetTypeCode(column.DataType), column.MaxLength, column.Primary);
            Add(field);
        }

        /// <summary>
        /// Add a new database field
        /// </summary>
        /// <param name="field"></param>
        private void Add(Field field)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
                // need to calculate the space needed
                //
                // Field
                //
                // 0 - unsigned byte - offset
                // 0 - unsigned byte - flag 0 = normal, 1 = deleted, 2 = Spare
                // 0 - unsigned byte - Field type enum value (0-255)
                // 0 - unsigned byte - If string or blob set the length (0-255)
                // 0 - unsigned byte - Primary key 0 - No, 1 - Yes (0-1)
                // 00 - leb128 - Length of element handled by the binary writer and reader in LEB128 format
                // bytes - string
                // ...
                // The structure repeats
                //

                TypeCode typeCode = field.Type;

                // Update the local cache

                Array.Resize(ref _fields, _items + 1);
                _fields[_items] = field;

                // Calcualte the data size

                int offset = 0;
                int length = field.Name.Length;
                offset = offset + 5 + LEB128.Size(length) + length;

                // move the data

                // this would need to shift the data area upwards to accomodate the
                // new column entry. Seems like a design problem, but may be somthing
                // to start with assuming that columns are not generally added. Could
                // create some spare space when we initialise the database.

                // Add the new field

                BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.Open));
                binaryWriter.Seek(_data, SeekOrigin.Begin);

                byte flag = 0;                                  // Normal
                binaryWriter.Write((byte)offset);               // write the offset to next field
                binaryWriter.Write(flag);                       // write the field Flag
                binaryWriter.Write((byte)typeCode);             // write the field Type
                binaryWriter.Write((sbyte)field.Length);        // write the field Length
                if (field.Primary == true)                      // write the primary key indicator (byte)
                {
                    binaryWriter.Write((byte)1);
                }
                else
                {
                    binaryWriter.Write((byte)0);
                }

                binaryWriter.Write(field.Name);          // Write the field Name

                _pointer = (UInt16)(_pointer + offset);
                _data = (UInt16)(_data + offset);
                _items = (byte)(_items + 1);

                binaryWriter.Seek(0, SeekOrigin.Begin);         //
                binaryWriter.Write(_size);                      // Skip over just re-write size
                binaryWriter.Write(_pointer);                   // Write pointer to new current record
                binaryWriter.Write(_data);                      // Write pointer to new data area
                binaryWriter.Write(_items);                     // write new number of records
                binaryWriter.Close();                           //
                binaryWriter.Dispose();                         //
            }
        }

        /// <summary>
        /// Remove a DataColumn
        /// Not sure if we need this helper methods as ties us to DataColumns
        /// </summary>
        /// <param name="column"></param>
        public void Remove(DataColumn column)
        {
            Field field = new Field(column.ColumnName, column.Flag, Type.GetTypeCode(column.DataType), column.MaxLength, column.Primary);
            Remove(field);
        }

        /// <summary>
        /// Delete an existing database field
        /// </summary>
        /// <param name="field"></param>
        private void Remove(Field field)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Delete an existing database field by index
        /// </summary>
        /// <param name="field"></param>
        private void Remove(int index)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
                if (index < _items)
                {

                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Set or update the DataColumn attributes
        /// </summary>
        /// <param name="column"></param>
        /// <param name="index"></param>
        public void Set(DataColumn column, int index)
        {
            Field field = new Field(column.ColumnName, column.Flag, Type.GetTypeCode(column.DataType), column.MaxLength, column.Primary);
            Set(field, index);
        }

        /// <summary>
        /// Set or update the field attributes
        /// </summary>
        /// <param name="field"></param>
        /// <param name="index"></param>
        private void Set(Field field, int index)
        {
            // Update the local cache then write to disk but
            // this is more complex as need to reinsert the column name 
            // if it is longer then move the data.
            // At the moment just update the local cache

            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
                if (index < _items)
                {
                    _fields[index] = field;

                    // Calculate the new data size

                    int offset = 5;
                    int l = field.Name.Length;
                    offset = offset + LEB128.Size(l) + l;

                    // The problem here is i dont know the length of the column field
                    // without reading the actual reecord
                    // could assume the cache is correct

                    BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".dbf", FileMode.Open));
                    UInt16 pointer = _start; // Skip over header and size
                    byte length = 0;
                    for (int counter = 0; counter < _size; counter++)
                    {
                        binaryReader.BaseStream.Seek(pointer, SeekOrigin.Begin);
                        length = binaryReader.ReadByte();
                        if (counter != index)
                        {
                            pointer = (UInt16)(pointer + length);
                        }
                        else
                        {
                            break;
                        }
                    }
                    binaryReader.Close();

                    if (offset > length)
                    {
                        // The new field is longer than the old field
                        // not sure what im doing here now as looks wrong
                        // what it should do it mark the column as deleted and 
                        // insert the new field at the end if no data written yet

                        BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.Open));
                        binaryWriter.Seek(_data, SeekOrigin.Begin);

                        byte flag = 0;
                        binaryWriter.Write((byte)length);              // write the length
                        binaryWriter.Write(flag);                      // write the field Flag
                        binaryWriter.Write((byte)field.Type);          // write the field Type
                        binaryWriter.Write((sbyte)field.Length);       // write the field Length
                        if (field.Primary == true)                     // write the primary key indicator (byte)
                        {
                            binaryWriter.Write((byte)1);
                        }
                        else
                        {
                            binaryWriter.Write((byte)0);
                        }
                        binaryWriter.Write(field.Name);                // Write the field Name

                        _pointer = (UInt16)(_pointer + offset);
                        _data = (UInt16)(_data + offset);
                        _items = (byte)(_items + 1);

                        binaryWriter.Seek(0, SeekOrigin.Begin);
                        _size++;                                        //
                        binaryWriter.Write(_size);                      // Write the number of records - size
                        binaryWriter.Write(_pointer);                   // Write pointer to new current record
                        binaryWriter.Write(_data);                      // Write pointer to new data area
                        binaryWriter.Write(_items);                     // write new number of records
                        binaryWriter.Close();                           //
                        binaryWriter.Dispose();                         //
                    }
                    else
                    {
                        // The new field name is shorter so can overrite

                        BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.Open));
                        binaryWriter.Seek(pointer + 2, SeekOrigin.Begin);

                        // Keep the field space as original
                        // No need to overwrite the flag
                        binaryWriter.Write((byte)field.Type);          // write the field Type
                        binaryWriter.Write((sbyte)field.Length);       // write the field Length
                        if (field.Primary == true)                     // write the primary key indicator (byte)
                        {
                            binaryWriter.Write((byte)1);
                        }
                        else
                        {
                            binaryWriter.Write((byte)0);
                        }
                        binaryWriter.Write(field.Name);                // Write the field Name
                        binaryWriter.Dispose();
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Get the DataColumn attributes
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal DataColumn Get(int index)
        {
            if (index < _items)
            {
                // Build from cache

                DataColumn column = new DataColumn(_fields[index].Name);
                column.Flag = _fields[index].Flag;
                column.DataType = Type.GetType("System." + Enum.GetName(typeof(TypeCode), _fields[index].Type));
                column.Primary = _fields[index].Primary;
                column.MaxLength = _fields[index].Length;
                return (column);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        #endregion
        #region Row Methods

        internal DataRow Read(int index)
        {
            DataRow data = new DataRow(this);
            //data.ItemArray = new object[_items];

            lock (_lockObject)
            {
                string filenamePath = System.IO.Path.Combine(_path, _name);
                // Need to search the index file

                BinaryReader indexReader = new BinaryReader(new FileStream(filenamePath + ".idx", FileMode.Open));
                BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".dbf", FileMode.Open));
                indexReader.BaseStream.Seek(index * 4, SeekOrigin.Begin);                               // Get the pointer from the index file
                UInt16 pointer = indexReader.ReadUInt16();                                              // Reader the pointer from the index file
                binaryReader.BaseStream.Seek(pointer, SeekOrigin.Begin);                                // Move to the correct location in the data file

                byte flag = binaryReader.ReadByte();
                for (int count = 0; count < _items; count++)
                {
                    switch (_fields[count].Type)
                    {
                        case TypeCode.Int16:
                            {
                                data.ItemArray[count] = binaryReader.ReadInt16();
                                break;
                            }
                        case TypeCode.Int32:
                            {
                                data.ItemArray[count] = binaryReader.ReadInt32();
                                break;
                            }
                        case TypeCode.String:
                            {
                                // should not need to lenght check again here
                                data.ItemArray[count] = binaryReader.ReadString();
                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }
                }
                binaryReader.Close();
                indexReader.Close();
            }
            return (data);
        }
        internal void Update(DataRow row, int Index)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
                // Write the data
                throw new NotImplementedException();


            }
        }
        internal void Create(DataRow row)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
                // append the new pointer the new index file

                BinaryWriter indexWriter = new BinaryWriter(new FileStream(filenamePath + ".idx", FileMode.Append));
                indexWriter.Write(_pointer);  // Write the pointer

                int offset = 0;
                offset += 1;    // Including the flag
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    object data = row.ItemArray[i];
                    switch (_fields[i].Type)
                    {
                        case TypeCode.Int16:
                            {
                                offset += 2;
                                break;
                            }
                        case TypeCode.Int32:
                            {
                                offset += 4;
                                break;
                            }
                        case TypeCode.String:
                            {
                                int length = _fields[i].Length;
                                if (length < 0)
                                {
                                    length = Convert.ToString(data).Length;
                                }
                                offset = offset + LEB128.Size(length) + length;     // Includes the byte length parameter
                                                                                    // ** need to watch this as can be 2 bytes if length is > 127 characters
                                                                                    // ** https://en.wikipedia.org/wiki/LEB128

                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException();
                            }
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
                _pointer = (UInt16)(_pointer + offset);                         //
                binaryWriter.Write((UInt16)(_pointer));                         // Write the pointer
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
                    switch (_fields[i].Type)
                    {
                        case TypeCode.Int16:
                            {
                                binaryWriter.Write((Int16)data);
                                break;
                            }
                        case TypeCode.Int32:
                            {
                                binaryWriter.Write((int)data);
                                break;
                            }
                        case TypeCode.String:
                            {
                                string text = Convert.ToString(data);
                                if (_fields[i].Length < 0)
                                {
                                    binaryWriter.Write(text);
                                }
                                else
                                {
                                    if (text.Length > _fields[i].Length)
                                    {
                                        text = text.Substring(0, _fields[i].Length);
                                    }
                                    else
                                    {
                                        text = text.PadRight(_fields[i].Length, '\0');
                                    }
                                    binaryWriter.Write(text);
                                }
                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }
                }
                binaryWriter.Close();
                binaryWriter.Dispose();
            }
        }
        internal void Delete(int index)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
                // Write the data
                throw new NotImplementedException();
            }
        }

        #endregion
        #region Private

        private Field GetField(int index)
        {
            if (index < _size)
            {
                Field field = new Field();
                string filenamePath = System.IO.Path.Combine(_path, _name);
                lock (_lockObject)
                {
                    BinaryReader binaryReader = new BinaryReader(new FileStream(filenamePath + ".dbf", FileMode.Open));
                    UInt16 pointer = _start; // Skip over header and size
                    byte offset = 0;
                    for (int counter = 0; counter < _size; counter++)
                    {
                        binaryReader.BaseStream.Seek(pointer, SeekOrigin.Begin);
                        offset = binaryReader.ReadByte();
                        if (counter != index)
                        {
                            pointer = (UInt16)(pointer + offset);
                        }
                        else
                        {
                            byte flag = binaryReader.ReadByte();                    // Read the status flag
                            TypeCode typeCode = (TypeCode)binaryReader.ReadByte();  // Read the field Type
                            sbyte length = binaryReader.ReadSByte();                // Read the field Length
                            bool primary = false;                                   // Read if the primary key
                            if (binaryReader.ReadByte() == 1)
                            {
                                primary = true;
                            }
                            string name = binaryReader.ReadString();                // Read the field Name
                            field = new Field(name, flag, typeCode, length, primary);
                            break;
                        }
                    }
                    binaryReader.Close();
                }
                return (field);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        #endregion
        #endregion
    }

    #region Static
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
                size += 1;
            } while (value != 0);
            return (data);
        }

        public static int Size(int value)
        {
            int size = 0;
            do
            {
                value >>= 7;
                size += 1;
            } while (value != 0);
            return (size);
        }
    }
    #endregion
}
