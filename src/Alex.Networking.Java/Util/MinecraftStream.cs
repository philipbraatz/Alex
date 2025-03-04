﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Alex.Common.Data;
using Alex.Common.Utils;
using fNbt;
using Microsoft.Xna.Framework;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using BlockCoordinates = Alex.Common.Utils.Vectors.BlockCoordinates;

namespace Alex.Networking.Java.Util
{
	public class MinecraftStream : Stream
	{
		private CancellationTokenSource CancelationToken { get; }
		private Stream BaseStream { get; set; }
		private Stream ReadStream { get; set; }
		private Stream WriteStream { get; set; }
		
		public bool DataAvailable
		{
			get
			{
				if (_originalBaseStream is NetworkStream ns)
				{
					return ns.DataAvailable;
				}

				return _originalBaseStream.Position < _originalBaseStream.Length;
			}
		}

		private Stream _originalBaseStream;
		public MinecraftStream(Stream baseStream, CancellationToken cancellationToken = default)
		{
			_originalBaseStream = baseStream;
			BaseStream = baseStream;
			CancelationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			ReadStream = baseStream;
			WriteStream = baseStream;
		}

		public MinecraftStream(CancellationToken cancellationToken = default) : this(new MemoryStream(), cancellationToken)
		{
			
		}

		public static Rijndael GenerateAES(byte[] key)
		{
			var cipher = new RijndaelManaged ();
			cipher.Mode = CipherMode.CFB;
			cipher.Padding = PaddingMode.None;
			cipher.KeySize = 128;
			cipher.FeedbackSize = 8;
			cipher.Key = key;
			cipher.IV = key;

			return cipher;
		}

		public void InitEncryption(Rijndael rijndael)
		{
			var aes = rijndael;
			ICryptoTransform encryptTransform = aes.CreateEncryptor ();
			ICryptoTransform decryptTransform = aes.CreateDecryptor ();
			ReadStream = new CryptoStream(BaseStream, decryptTransform, CryptoStreamMode.Read);
			WriteStream = new CryptoStream(BaseStream, encryptTransform, CryptoStreamMode.Write);
			/*EncryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
			EncryptCipher.Init(true, new ParametersWithIV(
				new KeyParameter(key), key, 0, 16));

			DecryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
			DecryptCipher.Init(false, new ParametersWithIV(
				new KeyParameter(key), key, 0, 16));

			BaseStream = new CipherStream(BaseStream, DecryptCipher, EncryptCipher);*/
		}

		public override bool CanRead => BaseStream.CanRead;
		public override bool CanSeek => BaseStream.CanRead;
		public override bool CanWrite => BaseStream.CanRead;
		public override long Length => BaseStream.Length;

		public override long Position
		{
			get { return ReadStream.Position; }
			set { ReadStream.Position = value; }
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return BaseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			BaseStream.SetLength(value);
		}

		public void Read(Span<byte> memory, int count)
		{
			var data = ReadStream.ReadToSpan(count);
			data.CopyTo(memory);
			//BaseStream.ReadToSpan()
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return ReadStream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			WriteStream.Write(buffer, offset, count);
		}
		
		public void Write(in Memory<byte> buffer, int offset, in int bufferLength)
		{
			var bytes = buffer.Slice(offset, bufferLength).ToArray();
			
			WriteStream.Write(bytes, offset, bytes.Length);
		}

		public override void Flush()
		{
			WriteStream.Flush();
			//BaseStream.Flush();
		}

		#region Reader

		public override int ReadByte()
		{
			return ReadStream.ReadByte();
		}

		public byte[] Read(int length)
		{
			if (BaseStream is MemoryStream)
			{
				var dat = new byte[length];
				Read(dat, 0, length);
				return dat;
			}

			//SpinWait s = new SpinWait();
			int read = 0;

			var buffer = new byte[length];
			while (read < buffer.Length && !CancelationToken.IsCancellationRequested)
			{
				int oldRead = read;

				int r = this.Read(buffer, read, length - read);
				if (r == 0) //No data read?
				{
					break;
				}

				read += r;

				if (CancelationToken.IsCancellationRequested) 
					throw new ObjectDisposedException("");
			}

			if (read < length)
				throw new EndOfStreamException();

			return buffer;
		}


		public int ReadInt()
		{
			var dat = new byte[4];
			Read(dat, 0, 4);
			var value = BitConverter.ToInt32(dat, 0);
			return IPAddress.NetworkToHostOrder(value);
		}

		public float ReadFloat()
		{
			var almost = Read(4);
			var f = BitConverter.ToSingle(almost, 0);
			return NetworkToHostOrder(f);
		}

		public bool ReadBool()
		{
			var answer = ReadByte();
			if (answer == 1)
				return true;
			return false;
		}

		public double ReadDouble()
		{
			var almostValue = Read(8);
			return NetworkToHostOrder(almostValue);
		}

		public int ReadVarInt()
		{
			return ReadVarInt(out _);
		}

		public int ReadVarInt(out int bytesRead)
		{
			int numRead = 0;
			int result = 0;
			byte read;
			do
			{
				read = (byte)ReadByte();;

				int value = (read & 0x7f);
				result |= (value << (7 * numRead));

				numRead++;
				if (numRead > 5)
				{
					throw new Exception("VarInt is too big");
				}
			} while ((read & 0x80) != 0);
			bytesRead = numRead;
			return result;
		}

		public long ReadVarLong()
		{
			int numRead = 0;
			long result = 0;
			byte read;
			do
			{
				read = (byte)ReadByte();
				int value = (read & 0x7f);
				result |= (value << (7 * numRead));

				numRead++;
				if (numRead > 10)
				{
					throw new Exception("VarLong is too big");
				}
			} while ((read & 0x80) != 0);

			return result;
		}

		public short ReadShort()
		{
			var da = Read(2);
			var d = BitConverter.ToInt16(da, 0);
			return IPAddress.NetworkToHostOrder(d);
		}

		public ushort ReadUShort()
		{
			var da = Read(2);
			return NetworkToHostOrder(BitConverter.ToUInt16(da, 0));
		}

		public ushort[] ReadUShort(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToUInt16(da, 0);
				us[i] = d;
			}
			return NetworkToHostOrder(us);
		}

		public ushort[] ReadUShortLocal(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToUInt16(da, 0);
				us[i] = d;
			}
			return us;
		}

		public short[] ReadShortLocal(int count)
		{
			var us = new short[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToInt16(da, 0);
				us[i] = d;
			}
			return us;
		}

		public string ReadString()
		{
			var length = ReadVarInt();
			var stringValue = Read(length);

			return Encoding.UTF8.GetString(stringValue);
		}

		public long ReadLong()
		{
			var l = Read(8);
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(l, 0));
		}

		public ulong ReadULong()
		{
			var l = Read(8);
			return NetworkToHostOrder(BitConverter.ToUInt64(l, 0));
		}

        public Vector3 ReadPosition()
		{
			var val = ReadULong();
			var x = Convert.ToSingle(val >> 38);
			var y = Convert.ToSingle(val & 0xFFF);

			//if (y > 2048)
			//	y = -(0xFFF - y);

			var z = Convert.ToSingle(val << 26 >> 38);  //Convert.ToSingle((val << 38 >> 38) >> 12);

			if (x >= (2^25))
			{
				x -= 2^26;
			}

			if (y >= (2^11))
			{
				y -= 2^12;
			}

			if (z >= (2^25))
			{
				z -= 2^26;
			}

            return new Vector3(x, y, z);
		}
        
        public BlockCoordinates ReadBlockCoordinates()
        {
	        var val = ReadLong();
	        var x = (val >> 38);
	        var y =(val & 0xFFF);

	        //if (y > 2048)
	        //	y = -(0xFFF - y);

	        var z = ((val << 38 >> 38) >> 12);  //Convert.ToSingle((val << 38 >> 38) >> 12);

	        if (x >= (2^25))
	        {
		        x -= 2^26;
	        }

	        //if (y >= (2^11))
	        //{
		    //    y -= 2^12;
	        //}

	        if (z >= (2^25))
	        {
		        z -= 2^26;
	        }

	        return new BlockCoordinates((int)x, (int)y, (int)z);
        }

		public SlotData ReadSlot()
		{
			bool present = ReadBool();
			if (!present) return null;

			int id = ReadVarInt();
			byte count = 0;
			NbtCompound nbt = null;
			
			count = (byte)ReadByte();
			nbt = ReadNbtCompound();
			

			SlotData slot = new SlotData();
			slot.Count = count;
			slot.Nbt = nbt;
			slot.ItemID = id;
		//	slot.ItemDamage = damage;

			return slot;
		}

		public void WriteSlot(SlotData slot)
		{
			WriteBool(slot != null && slot.ItemID != -1);
			if (slot == null)
				return;
			
			WriteVarInt(slot.ItemID);
			WriteByte(slot.Count);
			WriteNbtCompound(slot.Nbt);
		}

		private double NetworkToHostOrder(byte[] data)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data);
			}
			return BitConverter.ToDouble(data, 0);
		}

		private float NetworkToHostOrder(float network)
		{
			var bytes = BitConverter.GetBytes(network);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			return BitConverter.ToSingle(bytes, 0);
		}

		private ushort[] NetworkToHostOrder(ushort[] network)
		{
			if (BitConverter.IsLittleEndian)
				Array.Reverse(network);
			return network;
		}

		private ushort NetworkToHostOrder(ushort network)
		{
			var net = BitConverter.GetBytes(network);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(net);
			return BitConverter.ToUInt16(net, 0);
		}
		private ulong NetworkToHostOrder(ulong network)
		{
			var net = BitConverter.GetBytes(network);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(net);
			return BitConverter.ToUInt64(net, 0);
		}

        #endregion

        #region Writer

        public void Write(byte[] data)
		{
			this.Write(data, 0, data.Length);
		}

		public void WritePosition(Vector3 position)
		{
			var x = Convert.ToInt64(position.X);
			var y = Convert.ToInt64(position.Y);
			var z = Convert.ToInt64(position.Z);
			long toSend = ((x & 0x3FFFFFF) << 38) | ((z & 0x3FFFFFF) << 12) | (y & 0xFFF);
			WriteLong(toSend);
		}

	    public void WritePosition(BlockCoordinates pos)
	    {
            WritePosition(new Vector3(pos.X, pos.Y, pos.Z));
	    }
	    
	    private int WriteRawVarInt32(uint value)
	    {
		    int written = 0;
		    while ((value & -128) != 0)
		    {
			    WriteByte((byte) ((value & 0x7F) | 0x80));
			    value >>= 7;
		    }

		    WriteByte((byte) value);
		    written++;

		    return written;
	    }

		public int WriteVarInt(int value)
		{
			return WriteRawVarInt32((uint) value);
			int write = 0;
			do
			{
				byte temp = (byte)(value & 127);
				value >>= 7;
				if (value != 0)
				{
					temp |= 128;
				}
				WriteByte(temp);
				write++;
			} while (value != 0);
			return write;
		}

		public int WriteVarLong(long value)
		{
			int write = 0;
			do
			{
				byte temp = (byte)(value & 127);
				value >>= 7;
				if (value != 0)
				{
					temp |= 128;
				}
				WriteByte(temp);
				write++;
			} while (value != 0);
			return write;
		}

		public void WriteInt(int data)
		{
			var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
			Write(buffer);
		}

		public void WriteString(string data)
		{
			var stringData = Encoding.UTF8.GetBytes(data);
			WriteVarInt(stringData.Length);
			Write(stringData);
		}

		public void WriteShort(short data)
		{
			var shortData = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
			Write(shortData);
		}

		public void WriteUShort(ushort data)
		{
			var uShortData = BitConverter.GetBytes(data);
			Write(uShortData);
		}

		public void WriteBool(bool data)
		{
			Write(BitConverter.GetBytes(data));
		}

		public void WriteDouble(double data)
		{
			Write(HostToNetworkOrder(data));
		}

		public void WriteFloat(float data)
		{
			Write(HostToNetworkOrder(data));
		}

		public void WriteLong(long data)
		{
			Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data)));
		}

		public void WriteULong(ulong data)
		{
			Write(HostToNetworkOrderLong(data));
		}

        public void WriteUuid(MiNET.Utils.UUID uuid)
		{
			var guid = uuid.GetBytes();
			var long1 = new byte[8];
			var long2 = new byte[8];
			Array.Copy(guid, 0, long1, 0, 8);
			Array.Copy(guid, 8, long2, 0, 8);
			Write(long1);
			Write(long2);
		}

		public MiNET.Utils.UUID ReadUuid()
		{
			var long1 = Read(8);
			var long2 = Read(8);

			return new MiNET.Utils.UUID(long1.Concat(long2).ToArray());
		}


		private byte[] HostToNetworkOrder(double d)
		{
			var data = BitConverter.GetBytes(d);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(data);

			return data;
		}

		private byte[] HostToNetworkOrder(float host)
		{
			var bytes = BitConverter.GetBytes(host);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			return bytes;
		}

		private byte[] HostToNetworkOrderLong(ulong host)
		{
			var bytes = BitConverter.GetBytes(host);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			return bytes;
		}

        #endregion

        private object _disposeLock = new object();
		private bool _disposed = false;
		protected override void Dispose(bool disposing)
		{
			if (!Monitor.IsEntered(_disposeLock))
				return;

			try
			{
				if (disposing && !_disposed)
				{
					_disposed = true;

					if (!CancelationToken.IsCancellationRequested)
						CancelationToken.Cancel();


				}
				base.Dispose(disposing);
			}
			finally
			{
				Monitor.Exit(_disposeLock);
			}
		}

		public NbtCompound ReadNbtCompound()
		{
			NbtTagType t = (NbtTagType) ReadByte();
			if (t != NbtTagType.Compound) return null;
			Position--;

            NbtFile file = new NbtFile() { BigEndian = true, UseVarInt = false };

			file.LoadFromStream(this, NbtCompression.None);

			return (NbtCompound) file.RootTag;
		}

		public void WriteNbtCompound(NbtCompound compound)
		{
			if (compound == null)
			{
				WriteByte(0);
				return;
			}

			NbtFile f = new NbtFile(compound) {BigEndian = true, UseVarInt = false};
			f.SaveToStream(this, NbtCompression.None);
			//WriteByte(0);
		}

		public string ReadChatObject()
		{
			string raw = ReadString();
			if (ChatObject.TryParse(raw, out string result))
			{
				return result;
			}

			return raw;// new ChatObject(raw);
		}
	}
}
