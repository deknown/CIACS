using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CIACS
{
	public static class Extensions
	{
		public static IEnumerable<T> DistinctUnionSorted<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, int> comparer, Func<T, T, int> bufferComparer = null) where T : new()
		{
			if (bufferComparer == null) bufferComparer = comparer;
			using (var firstEnumerator = first.GetEnumerator())
			using (var secondEnumerator = second.GetEnumerator())
			{
				var elementsInFirst = firstEnumerator.MoveNext();
				var elementsInSecond = secondEnumerator.MoveNext();

				T previous = new T();

				while (elementsInFirst || elementsInSecond)
				{
					if (!elementsInFirst)
					{
						do
						{
							if (bufferComparer(secondEnumerator.Current, previous) > 0)
							{
								yield return secondEnumerator.Current;
								previous = secondEnumerator.Current;
							}
						} while (secondEnumerator.MoveNext());
						yield break;
					}

					if (!elementsInSecond)
					{
						do
						{
							if (bufferComparer(firstEnumerator.Current, previous) > 0)
							{
								yield return firstEnumerator.Current;
								previous = firstEnumerator.Current;
							}
						} while (firstEnumerator.MoveNext());
						yield break;
					}
					if (comparer(firstEnumerator.Current, secondEnumerator.Current) < 0)
					{
						if (bufferComparer(firstEnumerator.Current, previous) > 0)
						{
							yield return firstEnumerator.Current;
							previous = firstEnumerator.Current;
						}
						elementsInFirst = firstEnumerator.MoveNext();

					}
					else if (comparer(firstEnumerator.Current, secondEnumerator.Current) == 0)
					{
						elementsInSecond = secondEnumerator.MoveNext();
					}
					else
					{
						if (bufferComparer(secondEnumerator.Current, previous) > 0)
						{
							yield return secondEnumerator.Current;
							previous = secondEnumerator.Current;
						}
						elementsInSecond = secondEnumerator.MoveNext();
					}
				}
			}
		}
		public static List<SoftWare> RemoveVersion(this List<SoftWare> software)
		{
			return software.Select(x =>
			{
				x.name = Regex.Replace(x.name, "\\s*\\([Xx](64|86).*\\)|\\s*[Xx](64|86)[^\\s]*", "");
				if (x.version != null)
				{
					string version = x.version;
					if (version.Length > 2 && version[version.Length - 2] == '.')
					{
						version = version.Remove(version.Length - 2); //if 1.156.45.0 => dot & zero at the end
					}
					x.name = Regex.Replace(x.name, "\\W*" + Regex.Escape(version) + "[^\\s]*", "");
				}
				return x;
			}).ToList();
		}
		public static TreeNodeCollection AddTreeLocations(this TreeNodeCollection nodes, List<Location> locations)
		{
			foreach (Location location in locations)
			{
				TreeNode treeNode = new TreeNode(location.name)
				{
					Tag = location.id
				};
				if (location.childs != null)
				{
					treeNode.Nodes.AddTreeLocations(location.childs);
				}
				nodes.Add(treeNode);
			}
			return nodes;
		}
		public static TreeNode FindNodeByTag(this TreeNodeCollection nodes, int id)
		{
			TreeNode search = null;
			foreach (TreeNode node in nodes)
			{
				if ((int)node.Tag == id)
				{
					search = node;
				}
				else if (node.Nodes.Count > 0)
				{
					search = node.Nodes.FindNodeByTag(id);
				}
				if (search != null) break;
			}
			return search;
		}
	}
}
