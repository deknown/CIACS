using System;

namespace CIACS
{
	public class SoftWare : IEquatable<SoftWare>, IComparable<SoftWare>
	{
		public string name;
		public string version;

		public int CompareTo(SoftWare p)
		{
			int result = String.Compare(name, p.name);
			if (result == 0) result = String.Compare(p.version, version);
			return result;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SoftWare);
		}

		public bool Equals(SoftWare p)
		{
			if (Object.ReferenceEquals(p, null))
			{
				return false;
			}

			if (Object.ReferenceEquals(this, p))
			{
				return true;
			}

			if (this.GetType() != p.GetType())
			{
				return false;
			}

			return (name == p.name) && (version == p.version);
		}

		public override int GetHashCode()
		{
			return (name + version).GetHashCode();
		}

		public static bool operator ==(SoftWare lhs, SoftWare rhs)
		{
			if (Object.ReferenceEquals(lhs, null))
			{
				if (Object.ReferenceEquals(rhs, null))
				{
					return true;
				}
				return false;
			}
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SoftWare lhs, SoftWare rhs)
		{
			return !(lhs == rhs);
		}
	}
}
