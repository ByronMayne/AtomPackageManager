using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;
using Object = UnityEngine.Object;
using System.Diagnostics;
using AtomPackageManager.Popups;

namespace AtomPackageManager.Services
{
    [System.Serializable]
    public class GitSourceControlService : ISourceControlService
    {
        private string m_RepositoryURL;
        private string m_Directory;
        private string m_RepositoryName;
        private bool m_WasSuccessful;
        private string m_WorkingDirectory;
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
        /// Gets the name of this repository.
        /// </summary>
        public string repositoryName
        {
            get { return m_RepositoryName;  }
        }

        /// <summary>
        /// The directory where our repository is cloned too.
        /// </summary>
        public string directory
        {
            get
            {
                return m_Directory;
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
                MessagePopup.ShowSimpleMessage(EditorWindow.GetWindow<PackageEditor>(), 
                                               "Invalid git URL", "The url '" +
                                                repositoryURL + 
                                                "' is invalid. All repositories must end with the extension .git. Please correct the URL and try again",
                                                MessagePopup.Type.Error);
                return;
            }

            m_RepositoryURL = repositoryURL;
            m_Directory = workingDirectory;
            m_RepositoryName = Path.GetFileNameWithoutExtension(m_RepositoryURL);
            m_WorkingDirectory = FileUtil.GetUniqueTempPathInProject() + "/";

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
            if (!Directory.Exists(m_WorkingDirectory))
            {
                Directory.CreateDirectory(m_WorkingDirectory);
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
                processInfo.RedirectStandardOutput = false;
                // and our error logs
                processInfo.RedirectStandardError = false;
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                processInfo.FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
                // On Mac we do use shell
                processInfo.UseShellExecute = true;
            }
            // Set our arguments
            processInfo.Arguments += " git clone -o master " + repositoryURL + " " + directory + repositoryName + "/";
            // We don't want to show a window.
            processInfo.CreateNoWindow = false;
            // We work inside our new directory
            processInfo.WorkingDirectory = m_WorkingDirectory;
            // Start the process
            Process gitCloneProcess = Process.Start(processInfo);
            gitCloneProcess.StartInfo.RedirectStandardOutput = true;
            gitCloneProcess.StartInfo.RedirectStandardError = true;
            //* Set your output and error (asynchronous) handlers
            gitCloneProcess.OutputDataReceived += OutputHandler;
            gitCloneProcess.ErrorDataReceived += ErrorHandler;
            //* Start process and handlers
            gitCloneProcess.Start();
            gitCloneProcess.BeginOutputReadLine();
            gitCloneProcess.BeginErrorReadLine();
            gitCloneProcess.WaitForExit();
            m_WasSuccessful = gitCloneProcess.ExitCode == 0;
            UnityEngine.Debug.Log(m_WasSuccessful + " | " + gitCloneProcess.ExitCode);
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
        /// Our error handler that logs all results to our Unity console. 
        /// </summary>
        private void ErrorHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if(!string.IsNullOrEmpty(outLine.Data))
            {
                UnityEngine.Debug.LogError(outLine.Data);
            }
        }


        /// <summary>
        /// Our log handler that logs all results to our Unity console. 
        /// </summary>
        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                UnityEngine.Debug.Log(outLine.Data);
            }
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
