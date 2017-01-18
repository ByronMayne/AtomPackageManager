using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AtomPackageManager
{
    public static class PathHelpers
    {
        /// <summary>
        /// The starting part of a path that is required for it to be an Asset Path. 
        /// </summary>
        public const string ROOT_FOLDER_NAME = "Assets";

        /// <summary>
        /// The char we use to split our paths.
        /// </summary>
        public const char PATH_SPLITTER = '/';

        /// <summary>
        /// Converts a a full system path to a local asset path. 
        /// <example>
        /// Input (SystemPath): C:/Users/Projects/MyProject/Asset/Images/Bear.png
        /// Output  (AssetPath) : Assets/Images/Bear.png
        /// </example>
        /// </summary>
        /// <param name="systemPath"></param>
        /// <returns></returns>
        public static string SystemToAssetPath(string systemPath)
        {
            string assetPath = string.Empty;

            // We have to make sure we are not working on a useless string. 
            if (string.IsNullOrEmpty(systemPath))
            {
                throw new System.ArgumentNullException("SystemPath", "The asset path that was sent in was null or empty. Can not convert");
            }

            // Make sure we are in the right directory
            if (!systemPath.StartsWith(Application.dataPath))
            {
                throw new System.InvalidOperationException(string.Format("The path {0} does not start with {1} which is our current directory. This can't be converted", systemPath, Application.dataPath));
            }

            // Get the number of chars in our system directory path
            int systemDirNameLength = Application.dataPath.Length;

            // Subtract the name of the asset folder since we want to keep that part. 
            systemDirNameLength -= ROOT_FOLDER_NAME.Length;

            // Get the count of the number of letter left in our path starting from '/Assets'
            int assetDirNameLength = systemPath.Length - systemDirNameLength;

            // Substring to get our result. 
            assetPath = systemPath.Substring(systemDirNameLength, assetDirNameLength);

            // Return
            return assetPath;
        }

        /// <summary>
        /// Takes a Unity asset path and converts it to a system path.
        /// <example>
        /// Input  (AssetPath) : Assets/Images/Bear.png
        /// Output (SystemPath): C:/Users/Projects/MyProject/Asset/Images/Bear.png
        /// </example>
        /// </summary>
        /// <param name="assetPath">The Unity asset path you want to convert.</param>
        /// <returns>The newly created system path.</returns>
        public static string AssetPathToSystemPath(string assetPath)
        {
            // A local holder for our working path. 
            string systemPath = string.Empty;

            // We have to make sure we are not working on a useless string. 
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new System.ArgumentNullException("AssetPath", "The asset path that was sent in was null or empty. Can not convert");
            }

            // Get the index of 'Asset/' part of the path 
            if (!assetPath.StartsWith(ROOT_FOLDER_NAME + PATH_SPLITTER))
            {
                // This is returned by Unity and we must have it to convert
                throw new System.InvalidOperationException(string.Format("Can't convert '{0}' to a System Path since it does not start with '{1}'", assetPath, ROOT_FOLDER_NAME + PATH_SPLITTER));
            }

            // Get our starting length.
            int pathLength = assetPath.Length;

            // Now subtract the root folder name 
            pathLength -= ROOT_FOLDER_NAME.Length;

            // Now get the substring
            assetPath = assetPath.Substring(ROOT_FOLDER_NAME.Length, pathLength);

            // Combine them
            systemPath = Application.dataPath + assetPath;

            // Return the result
            return systemPath;
        }
    }
}
