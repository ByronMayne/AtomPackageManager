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
using System.Collections.Generic;

namespace AtomPackageManager.Services
{
    [System.Serializable]
    public class GitSourceControlService : ThreadRoutine, ISourceControlService
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
                MessagePopup.ShowSimpleMessage("Invalid git URL", "The url '" +
                                                repositoryURL + 
                                                "' is invalid. All repositories must end with the extension .git. Please correct the URL and try again",
                                                MessagePopup.Type.Error);
                return;
            }

            m_RepositoryURL = repositoryURL;
            m_RepositoryName = Path.GetFileNameWithoutExtension(m_RepositoryURL);
            m_Directory = workingDirectory + "/" + repositoryName + "/";
            m_WorkingDirectory = FileUtil.GetUniqueTempPathInProject() + "/";
            m_OnCompleteCallback = onComplete;
            m_WasSuccessful = true;
            StartThread();
        }

        protected override IEnumerator<RoutineInstructions> ProcessOperation()
        {
            yield return RoutineInstructions.ContinueOnThread;

            // Create a folder if it does not exist. 
            if (!Directory.Exists(m_WorkingDirectory))
            {
                Directory.CreateDirectory(m_WorkingDirectory);
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
            processInfo.Arguments += " git clone -o master " + repositoryURL + " " + directory;
            // We don't want to show a window.
            processInfo.CreateNoWindow = false;
            // We work inside our new directory
            processInfo.WorkingDirectory = m_WorkingDirectory;
            // Start the process
            Process gitCloneProcess = Process.Start(processInfo);
            //* Set your output and error (asynchronous) handlers
            gitCloneProcess.WaitForExit(10000);
            m_WasSuccessful = gitCloneProcess.ExitCode == 0;
        }

        protected override void OnOperationComplete()
        {
            if(m_OnCompleteCallback != null)
            {
                m_OnCompleteCallback(this);
            }
        }

        /// <summary>
        /// Our error handler that logs all results to our Unity console. 
        /// </summary>
        private void ErrorHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if(!string.IsNullOrEmpty(outLine.Data))
            {
                MessagePopup.ShowSimpleMessage( "Git Clone Error", "An error was received when trying to clone the following repository '" +
                                                repositoryURL +
                                                "'. " + System.Environment.NewLine + outLine.Data,
                                                MessagePopup.Type.Error);
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
