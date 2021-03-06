﻿/*
    Copyright (C) 2011-2012 de4dot@gmail.com

    This file is part of de4dot.

    de4dot is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    de4dot is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with de4dot.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using de4dot.PE;

namespace de4dot.code.deobfuscators.MaxtoCode {
	class McKey {
		PeHeader peHeader;
		byte[] data;

		public byte this[int index] {
			get { return data[index]; }
		}

		public McKey(PeImage peImage, PeHeader peHeader) {
			this.peHeader = peHeader;
			try {
				this.data = peImage.readBytes(peHeader.getMcKeyRva(), 0x2000);
			}
			catch (IOException) {
				this.data = peImage.readBytes(peHeader.getMcKeyRva(), 0x1000);
			}
		}

		public byte[] readBytes(int offset, int len) {
			byte[] bytes = new byte[len];
			Array.Copy(data, offset, bytes, 0, len);
			return bytes;
		}

		public byte readByte(int offset) {
			return data[offset];
		}

		public uint readUInt32(int offset) {
			return BitConverter.ToUInt32(data, offset);
		}
	}
}
