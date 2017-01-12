using AtomPackageManager.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AtomPackageManager
{
    public struct SerilizationRequest
    {
        private string m_SerializedData;
        private object m_Target;
        private string m_FilePath;


        public object target
        {
            get { return m_Target; }
        }

        public string filePath
        {
            get { return m_FilePath; }
        }

        public SerilizationRequest(object target, string filePath)
        {
            m_Target = target;
            m_SerializedData = string.Empty;
            m_FilePath = filePath;
        }

        public void SetResult(string serializedData)
        {
            m_SerializedData = serializedData;
        }
    }

    public struct DeserializeRequest
    {
        private object m_Result;
        private Type m_Type;
        private string m_SerializedData;

        public object result
        {
            get
            {
                return m_Result; 
            }
        }

        public Type type
        {
            get { return m_Type; }
        }

        public string SerializedData
        {
            get
            {
                return m_SerializedData; 
            }
        }

        public DeserializeRequest(string serializedData, Type type)
        {
            m_SerializedData = serializedData;
            m_Result = null;
            m_Type = type;
        }

        public void SetResult(object result)
        {
            m_Result = result;
        }

        public static DeserializeRequest FromString(string serializedData, Type type)
        {
            DeserializeRequest request = new DeserializeRequest(serializedData, type);
            return request; 
        }

        /// <summary>
        /// Creates a new Deserializion request but reads the data from a file on disk 
        /// instead of the data directly. 
        /// </summary>
        public static DeserializeRequest FromFile(string filePath, Type type)
        {
            string serilizedData = File.ReadAllText(filePath);
            DeserializeRequest request = new DeserializeRequest(serilizedData, type);
            return request; 
        }
    }
}
