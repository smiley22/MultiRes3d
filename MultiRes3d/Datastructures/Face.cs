using System;

namespace MultiRes3d {
	public class Face {
		uint[] storage;
		int offset;


		public uint this[int index] {
			get {
				return storage[offset + index];
			}
			set {
				storage[offset + index] = value;
			}
		}

		public Face(uint index1, uint index2, uint index3, uint[] storage, int offset) {
			this.storage = storage;
			this.offset = offset;
			try {
				storage[offset + 0] = index1;
				storage[offset + 1] = index2;
				storage[offset + 2] = index3;
			} catch (IndexOutOfRangeException ex) {
				throw new Exception("OFFSET: " + offset, ex);
			}
		}

		public Face(uint[] indices, uint[] storage, int offset)
			: this(indices[0], indices[1], indices[2], storage, offset) {
		}
	}
}
