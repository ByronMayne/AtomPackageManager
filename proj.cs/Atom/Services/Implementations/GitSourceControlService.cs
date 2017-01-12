using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace AtomPackageManager.Services
{
    [System.Serializable]
    public class GitSourceControlService : ISourceControlService
    {
        private string m_RepositoryURL;
        private string m_WorkingDirectory;
        private bool m_WasSuccessful;

        /// <summary>
        /// The URL of the repository we are trying to clone.
        /// </summary>
        public string repositoryURL
        {
            get
            {
                return m_RepositoryURL;
            }
        }

        /// <summary>
        /// The directory where our repository is cloned too.
        /// </summary>
        public string workingDirectory
        {
            get
            {
                return m_WorkingDirectory;
            }
        }

        /// <summary>
        /// Did we clone with no issues?
        /// </summary>
        public bool wasSuccessful
        {
            get
            {
                return m_WasSuccessful;
            }
        }

        public void Clone(string respiratoryURL, string workingDirectory, OnCloneCompletedDelegate onComplete)
        {
            // Start a new thread process.
            ThreadStart threadStart = delegate
            {
                CloneThreadProcess(respiratoryURL, workingDirectory, onComplete);
            };

            // Create the thread
            Thread cloneThread = new Thread(threadStart);
            // The start it.
            cloneThread.Start();
        }

        /// <summary>
        /// Clones a git repository to disk.
        /// </summary>
        /// <param name="gitURL">The URL of the git repository</param>
        /// <param name="repositoryLocation">The location on disk you want to clone it too</param>
        private void CloneThreadProcess(string respiratoryURL, string workingDirectory, OnCloneCompletedDelegate onComplete)
        {
            // Create a folder if it does not exist. 
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }

            // Create a new process for the git request. 
            var processInfo = new ProcessStartInfo();
            // Set our file depending on platform
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                processInfo.FileName = "cmd.exe";
                // On windows we don't use shell
                processInfo.UseShellExecute = false;
                // On Windows '/c' closes the console when it's done
                processInfo.Arguments = "/c ";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                processInfo.FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
                // On Mac we do use shell
                processInfo.UseShellExecute = true;
            }
            // Set our arguments
            processInfo.Arguments += " git clone -o master " + respiratoryURL + " " + workingDirectory;
            // We don't want to show a window.
            processInfo.CreateNoWindow = false;

            // We work inside our new directory
            processInfo.WorkingDirectory = workingDirectory;
            // Start the process
            Process gitCloneProcess = Process.Start(processInfo);
            // Yield until it's done. 
            gitCloneProcess.WaitForExit();
            // Clone the window when we are complete.
            gitCloneProcess.Close();
            // Clean up our process
            gitCloneProcess.Dispose();

            // Fire our event but we have to delay it as we are currently
            // executing on a custom thread. delayCall is always called in the main thread.
            EditorApplication.delayCall += () =>
            {
                // Invoke our callback. 
                if(onComplete != null)
                {
                    onComplete(this);
                }
            };
        }

        /// <summary>
        /// Creates a clone of this object.
        /// </summary>
        public ISourceControlService CreateCopy()
        {
            return MemberwiseClone() as ISourceControlService;
        }
    }
}
