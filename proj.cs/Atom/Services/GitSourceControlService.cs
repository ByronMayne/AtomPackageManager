using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager.Services
{
    public class GitSourceControlService : Object, IEventListener
    {
        /// <summary>
        /// Clones a git repository to disk.
        /// </summary>
        /// <param name="gitURL">The URL of the git repository</param>
        /// <param name="repositoryLocation">The location on disk you want to clone it too</param>
        private void Clone( GitCloneRequest request)
        {
            // Get the name of the repository
            string repoName = Path.GetFileNameWithoutExtension(request.sourceURL);

            // Create a folder if it does not exist. 
            if (!Directory.Exists(request.workingDirectory))
            {
                Directory.CreateDirectory(request.workingDirectory);
            }

            // Create a new process for the git request. 
            var processInfo = new ProcessStartInfo("cmd.exe", "/c git clone -o master " + request.sourceURL + " " + request.workingDirectory);
            // We don't want to show a window.
            processInfo.CreateNoWindow = false;
            // We don't want to use Shell
            processInfo.UseShellExecute = false;
            // We work inside our new directory
            processInfo.WorkingDirectory = request.workingDirectory;
            // Start the process
            Process gitCloneProcess = Process.Start(processInfo);
            // Yield until it's done. 
            gitCloneProcess.WaitForExit();
            // Clone the window when we are complete.
            gitCloneProcess.Close();
            // Fire our event but we have to delay it so we are on the main thread
            EditorApplication.delayCall += () =>
            {
                Atom.Notify(Events.ON_CLONE_COMPLETE, request);
            };
        }

        /// <summary>
        /// Updates the git repository at a location.
        /// </summary>
        /// <param name="repositoryLocation">The location on disk you want to clone it too</param>
        private void Update(string repositoryLocation)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Commits all local changes on disk to the current repository.
        /// </summary>
        /// <param name="repositoryLocation">The location on disk you want to clone it too</param>
        private void Commit(string repositoryLocation)
        {
            throw new System.NotImplementedException();
        }

        public void OnNotify(int eventCode, object context)
        {
            if(eventCode == Events.GIT_CLONE_REQUESTED)
            {
                GitCloneRequest cloneRequest = (GitCloneRequest)context;
                // Create our start function
                ThreadStart threadStart = delegate
                {
                    Clone(cloneRequest);
                };

                // Run the tread. 
                Thread cloneThread = new Thread(threadStart);
                cloneThread.Start(); 
            }
        }
    }
}
