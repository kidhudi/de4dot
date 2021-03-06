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
using System.Text;
using Mono.Cecil;

namespace de4dot.code.deobfuscators.CliSecure {
	class StringDecrypter {
		ModuleDefinition module;
		TypeDefinition stringDecrypterType;
		MethodDefinition stringDecrypterMethod;
		byte[] stringDecrypterKey;

		public bool Detected {
			get { return stringDecrypterMethod != null; }
		}

		public TypeDefinition Type {
			get { return stringDecrypterType; }
		}

		public MethodDefinition Method {
			get { return stringDecrypterMethod; }
			set { stringDecrypterMethod = value; }
		}

		public StringDecrypter(ModuleDefinition module, MethodDefinition stringDecrypterMethod) {
			this.module = module;
			this.stringDecrypterMethod = stringDecrypterMethod;
		}

		public StringDecrypter(ModuleDefinition module, StringDecrypter oldOne) {
			this.module = module;
			stringDecrypterType = lookup(oldOne.stringDecrypterType, "Could not find stringDecrypterType");
			stringDecrypterMethod = lookup(oldOne.stringDecrypterMethod, "Could not find stringDecrypterMethod");
			stringDecrypterKey = oldOne.stringDecrypterKey;
		}

		T lookup<T>(T def, string errorMessage) where T : MemberReference {
			return DeobUtils.lookup(module, def, errorMessage);
		}

		public void find() {
			stringDecrypterKey = new byte[1] { 0xFF };
			foreach (var type in module.Types) {
				if (type.FullName == "<D234>" || type.FullName == "<ClassD234>") {
					stringDecrypterType = type;
					foreach (var field in type.Fields) {
						if (field.FullName == "<D234> <D234>::345" || field.FullName == "<ClassD234>/D234 <ClassD234>::345") {
							stringDecrypterKey = field.InitialValue;
							break;
						}
					}
					break;
				}
			}
		}

		public string decrypt(string es) {
			if (stringDecrypterKey == null)
				throw new ApplicationException("Trying to decrypt strings when stringDecrypterKey is null (could not find it!)");
			StringBuilder sb = new StringBuilder(es.Length);
			for (int i = 0; i < es.Length; i++)
				sb.Append(Convert.ToChar((int)(es[i] ^ stringDecrypterKey[i % stringDecrypterKey.Length])));
			return sb.ToString();
		}
	}
}
