using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPackageManager
{
    public interface ISourceControlProvider
    {
        /// <summary>
        /// Sets the web uri for this source control service
        /// </summary>
        void SetSourceURI(string uri);

        /// <summary>
        /// Clones a new copy of the source to our working location.
        /// </summary>
        void Checkout();

        /// <summary>
        /// Updates our current version to latest.
        /// </summary>
        void Update();
         
        /// <summary>
        /// Commits our local changes.
        /// </summary>
        void Commit();
    }
}