using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;
using UnityEngine.Assertions;

namespace AtomPackageManager.Services
{
    [System.Serializable]
    public class GitSourceControlService : ISourceControlService
    {
        private string m_RepositoryURL;
        private string m_WorkingDirectory;
        private bool m_WasSuccessful;
        private OnCloneCompletedDelegate m_OnCompleteCallback;

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

        public void Clone(string repositoryURL, string workingDirectory, OnCloneCompletedDelegate onComplete)
        {
            if(string.IsNullOrEmpty(repositoryURL))
            {
                throw new System.ArgumentNullException("respiratoryURL", "You must send a valid repository url to make a clone request, yours was null or empty");
            }

            if (string.IsNullOrEmpty(workingDirectory))
            {
                throw new System.ArgumentNullException("workingDirectory", "You must set a valid working directory, yours was null or empty");
            }

            if(!repositoryURL.EndsWith(".git"))
            {
                throw new System.ArgumentException("repositoryURL", "Invalid git url all must end with .git");
            }

            m_RepositoryURL = repositoryURL;
            m_WorkingDirectory = workingDirectory;

            m_WasSuccessful = true;
            // Start a new thread process.
            ThreadStart threadStart = delegate
            {
                CloneThreadProcess(onComplete);
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
        private void CloneThreadProcess(OnCloneCompletedDelegate onComplete)
        {
            // Create a folder if it does not exist. 
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }

            // Save our callback.
            m_OnCompleteCallback = onComplete;

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
                // Now forward our logs
                processInfo.RedirectStandardOutput = true;
                // and our error logs
                processInfo.RedirectStandardError = true;
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                processInfo.FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
                // On Mac we do use shell
                processInfo.UseShellExecute = true;
            }
            // Set our arguments
            processInfo.Arguments += " git clone -o master " + repositoryURL + " " + workingDirectory;
            // We don't want to show a window.
            processInfo.CreateNoWindow = true;
            // We work inside our new directory
            processInfo.WorkingDirectory = workingDirectory;
            // Start the process
            Process gitCloneProcess = Process.Start(processInfo);
            gitCloneProcess.BeginOutputReadLine();
            // Create a callback for out data output
            gitCloneProcess.OutputDataReceived += OutputDataReceived;
            // and one for our error output. 
            gitCloneProcess.ErrorDataReceived += ErrorDataReceived;
            // Once it's quit we move on.
            gitCloneProcess.Exited += GitCloneProcess_Exited;
            gitCloneProcess.Start();
        }

        private void GitCloneProcess_Exited(object sender, EventArgs e)
        {

            // Fire our event but we have to delay it as we are currently
            // executing on a custom thread. delayCall is always called in the main thread.
            EditorApplication.delayCall += () =>
            {
                // Invoke our callback. 
                if (m_OnCompleteCallback != null)
                {
                    m_OnCompleteCallback(this);
                }
            };
        }

        /// <summary>
        /// Our callback that we get when we are using the command line. It's all the output. 
        /// </summary>
        private void OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Debug.Log(e.Data);
        }

        /// <summary>
        /// Our callback that we get when we are using the command line. It's all the errors. 
        /// </summary>
        private void ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Debug.LogError(e.Data);
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
