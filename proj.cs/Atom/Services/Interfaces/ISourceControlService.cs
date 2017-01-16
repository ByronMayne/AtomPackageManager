using System;
namespace AtomPackageManager.Services
{
    public delegate void OnCloneCompletedDelegate(ISourceControlService service);

    public interface ISourceControlService
    {
        /// <summary>
        /// The remote url where the source code lives. 
        /// </summary>
        string repositoryURL { get; }

        /// <summary>
        /// The directory where we will be cloning our repository into.
        /// </summary>
        string workingDirectory { get; }

        /// <summary>
        /// Was the clone successful?
        /// </summary>
        bool wasSuccessful { get; }

        /// <summary>
        /// Request to clone a repository on to disk.
        /// </summary>
        /// <param name="repositoryURL">The url of the repository.</param>
        /// <param name="workingDirectory">Where our content will be cloned too</param>
        void Clone(string repositoryURL, string workingDirectory, OnCloneCompletedDelegate onComplete);

        /// Creates a shallow copy of this object
        /// </summary>
        ISourceControlService CreateCopy();
    }
}
