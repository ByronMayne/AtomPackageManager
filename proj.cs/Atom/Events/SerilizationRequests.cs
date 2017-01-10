using AtomPackageManager.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AtomPackageManager
{
    public struct SerilizationRequest<T>
    {
		private string m_SerializedData;
		private T m_Target;

		public Type type
		{
			get { return typeof(T); }
		}

		public string serializedData
		{
			get
			{
				return m_SerializedData; 
			}
		}

		public SerilizationRequest(T target)
		{
			m_Target = target;
			m_SerializedData = string.Empty;
		}

		public void SetResult(string serializedData)
		{
			m_SerializedData = serializedData;
		}
    }

	public struct DeserializeRequest<T>
	{
		private T m_Result;
		private string m_SerializedData;

		public Type type
		{
			get { return typeof(T); }
		}

		public T result
		{
			get
			{
				return m_Result; 
			}
		}

		public string SerializedData
		{
			get
			{
				return m_SerializedData; 
			}
		}

		public DeserializeRequest(string serializedData)
		{
			m_SerializedData = serializedData;
			m_Result = default(T);
		}

		public void SetResult(T result)
		{
			m_Result = result;
		}

		public static DeserializeRequest<T> FromString(string serializedData)
		{
			DeserializeRequest<T> request = new DeserializeRequest<T>(serializedData);
			return request; 
		}

		/// <summary>
		/// Creates a new Deserializion request but reads the data from a file on disk 
		/// instead of the data directly. 
		/// </summary>
		public static DeserializeRequest<T> FromFile(string filePath)
		{
			string serilizedData = File.ReadAllText(filePath);
			DeserializeRequest<T> request = new DeserializeRequest<T>(serilizedData);
			return request; 
		}
	}
}
