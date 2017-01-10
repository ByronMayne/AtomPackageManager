using UnityEngine;
using System.Collections.Generic;

namespace AtomPackageManager.Packages
{
	/// <summary>
	/// A class that contains all the packages that are included in this project.
	/// </summary>
	public class AtomProjectPackages
	{
		[SerializeField]
		private List<AtomPackage> m_Packages;

		/// <summary>
		/// Gets the list of packages in our project. 
		/// </summary>
		public List<AtomPackage> packages
		{
			get { return m_Packages; }
		}

		/// <summary>
		/// Gets the package at an index.
		/// </summary>
		/// <param name="index">Index.</param>
		public AtomPackage this[int index]
		{
			get
			{
				return m_Packages[index]; 
			}
		}

		/// <summary>
		/// Returns the number of Atom Packages we have in our project. 
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get { return m_Packages.Count; }
		}
	}
}

