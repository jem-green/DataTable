using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataTable
{
    public class DataHandler
    {
        // Solve problem of passing the PersistentDataTable to the other
        // classes, so nest the class 
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
        // 00 - LEB128 - Length of element handled by the binary writer and reader in LEB128 format
        // bytes - string
        // ...
        // The structure repeats
        // 
        // Data
        //
        // 1 - string
        // 00 - LEB128 - Length of element handled by the binary writer and reader in LEB128 format
        // bytes - string
        //
        // 2 - bool
        // 0 - unsigned byte - 0 = false, 1 = true
        // 
        // 3 - int
        // 0000 - 32 bit - defaults to zero
        // 
        // 4 - double
        // 000000000 - 64 bit - defaults to zero
        //
        // 5 - blob
        // 0000 - unsigned int32 - Length of blob
        // bytes - data
        //
        // Index
        //
        // 00 - unsigned int16 - pointer to data
        // 00 - unsigned int16 - length of data
        // ...
        // 00 - unsigned int16 - pointer to data + 1 
        // 00 - unsigned int16 - length of data + 1
        //

        #region variables

        private string _path = "";
        private string _name = "";

        private readonly object _lockObject = new Object();
        private UInt16 _size = 0;
        private UInt16 _pointer = 7;
        private UInt16 _data = 7;
        private byte _items = 0;
        private Field[] _fields;


        internal struct Field
        {
            string _name;
            TypeCode _type;
            sbyte _length;
            bool _primary;

            internal Field(string name, TypeCode type, sbyte length, bool primary)
            {
                _name = name;
                _type = type;
                _length = length;
                _primary = primary;
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

        public byte Fields
        {
            get
            {
                return (_items);
            }
        }

        #endregion
        #region Methods

        internal ArrayList List
        {
            get
            {
                return (new ArrayList(_fields));
            }
        }

        internal void Add(DataColumn column)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
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

                // Update the local cache
               
                Array.Resize(ref _fields, _items  + 1);
                Field field = new Field(column.ColumnName, typeCode, column.MaxLength, column.Primary);
                _fields[_items] = field;

                // Calcualte the data size

                int offset = 0;
                int length = column.ColumnName.Length;
                offset = offset + 3 + LEB128.Size(length) + length;

                // move the data

                // Add the new field

                BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.Open));
                binaryWriter.Seek(_data, SeekOrigin.Begin);

                binaryWriter.Write((byte)typeCode);             // write the field Type
                binaryWriter.Write((sbyte)column.MaxLength);     // write the field Length
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
                _items = (byte)(_items + 1);

                binaryWriter.Seek(0, SeekOrigin.Begin);
                binaryWriter.Write(_size);                  // Write the number of records - size
                binaryWriter.Write(_pointer);               // Write pointer to new current record
                binaryWriter.Write(_data);                  // Write pointer to new data area
                binaryWriter.Write(_items);               // write new number of records
                binaryWriter.Close();
                binaryWriter.Dispose();
            }
        }
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
                for (int count = 0; count < _items; count++)
                {
                    TypeCode typeCode = (TypeCode)binaryReader.ReadByte();  // Read the field Type
                    sbyte length = binaryReader.ReadSByte();                // Read the field Length
                    bool primary = false;                                    // Read if the primary key
                    if (binaryReader.ReadByte() == 1)
                    {
                        primary = true;
                    }
                    string name = binaryReader.ReadString();                // Read the field Name
                    Field field = new Field(name,typeCode,length,primary);
                    _fields[count] = field;
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
                Reset();
            }
        }

        internal void Reset()
        {
            // Reset the file
            string filenamePath = System.IO.Path.Combine(_path, _name);
            BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filenamePath + ".dbf", FileMode.OpenOrCreate));
            binaryWriter.Seek(0, SeekOrigin.Begin); // Move to start of the file
            _size = 0;
            _pointer = 7;                           // Start of the data 2 x 16 bit
            _data = 7;
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

        internal void Remove()
        {
        }

        internal DataRow Read(int index)
        {
            DataRow data = new DataRow();
            data.ItemArray = new object[_items];

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
                        case TypeCode.String:
                            {
                                // should not need to lenght check again here
                                data.ItemArray[count] = binaryReader.ReadString();
                                break;
                            }
                        case TypeCode.Int32:
                            {
                                data.ItemArray[count] = binaryReader.ReadInt32();
                                break;
                            }
                    }
                }
                binaryReader.Close();
                indexReader.Close();
            }
            return (data);
        }

        internal void Write(DataRow row)
        {
            string filenamePath = System.IO.Path.Combine(_path, _name);
            lock (_lockObject)
            {
                // Write the data


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
                    switch (_fields[i].Type)
                    {
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
                size += 1;
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
                size += 1;
            } while (value != 0);
            return (size);
        }
    }
}
