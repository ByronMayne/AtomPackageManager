namespace AtomPackageManager.Services
{
    /// <summary>
    /// A struct that is used as a container for a solutions project reference. 
    /// </summary>
    public struct PersistenceBlock
    {
        /// <summary>
        /// The name of the project (no extension)
        /// </summary>
        public string name;
        /// <summary>
        /// The path relative to the current solution or a full path.
        /// </summary>
        public string path;
        /// <summary>
        /// A unique GUID for the project.
        /// </summary>
        public string projectGUID;
        /// <summary>
        /// A hashed GUID that is used by the IDE to check what type it is and how to display it.
        /// </summary>
        public const string PROJECT_TYPE_GUID = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

        /// <summary>
        /// Loads this object from a string.
        /// </summary>
        public void ParseFromString(string content)
        {
            // We need to break up the string
            string[] components = content.Split('"');
            name = components[3];
            path = components[5];
            projectGUID = components[7];
        }

        /// <summary>
        /// Converts this object to it's string format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"", PROJECT_TYPE_GUID, name, path, projectGUID);
        }
    }

}
