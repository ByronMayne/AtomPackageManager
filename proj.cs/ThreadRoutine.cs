using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace AtomPackageManager
{
    public delegate void OnOperationCompleteDelegate();

    public abstract class ThreadRoutine
    {
        public enum RoutineInstructions
        {
            EndOperation = -1,
            ContinueOnThread = 0,
            ContinueOnMainThread = 1,
        }

        private event OnOperationCompleteDelegate m_OnOperationComplete;
        private Thread m_Thread;
        private bool m_WasStopped;
        private bool m_IsComplete;
        private bool m_OnMainThread = false;
        private object m_ThreadLock;
        private IList<ThreadRoutine> m_YieldingFor;
        private IEnumerator<RoutineInstructions> m_Routine;

        public ThreadRoutine()
        {
            m_YieldingFor = new List<ThreadRoutine>();
        }

        public event OnOperationCompleteDelegate MyProperty
        {
            add { m_OnOperationComplete += value; }
            remove { m_OnOperationComplete -= value; }
        }

        /// <summary>
        /// Get's or sets if this routine has been completed. 
        /// </summary>
        public virtual bool isComplete
        {
            get { return m_IsComplete; }
            protected set { m_IsComplete = value; }
        }

        /// <summary>
        /// Returns of someone stopped the threaded process. 
        /// </summary>
        public virtual bool wasStopped
        {
            get { return m_WasStopped; }
            private set { m_WasStopped = value; }
        }

        public void StartThread()
        {
            // Create our start thread. 
            ThreadStart threadStart = new ThreadStart(ThreadProcess);
            // Create our thread
            m_Thread = new Thread(ThreadProcess);
            // Send our start event
            OnOperationStarted();
            // Create our routine
            m_Routine = ProcessOperation();
            // Start it
            m_Thread.Start();
        }

        /// <summary>
        /// Stops the threaded routine from running. 
        /// </summary>
        public void StopThread()
        {
            wasStopped = true;
        }

        /// <summary>
        /// Stops this processing from running until the threaded routine
        /// stops running. 
        /// </summary>
        public void YieldTo(ThreadRoutine operation)
        {
            m_YieldingFor.Add(operation);
        }

        private void ThreadProcess()
        {
            while (m_Routine.MoveNext() && !wasStopped && !isComplete)
            {
                RoutineInstructions returnValue = m_Routine.Current;

                if (returnValue == RoutineInstructions.EndOperation)
                {
                    break;
                }
                else if (returnValue == RoutineInstructions.ContinueOnMainThread)
                {
                    m_OnMainThread = true;
                    EditorApplication.delayCall += () =>
                    {
                        m_Routine.MoveNext();
                        m_OnMainThread = false;
                    };

                }

                while (m_OnMainThread || m_WasStopped)
                {
                    Thread.Sleep(10);
                }

            }

            isComplete = true;
            // When it's done delay our call onto the main thread.
            EditorApplication.delayCall += () =>
            OnOperationComplete();
        }

        /// <summary>
        /// Called on the Main Thread just before starting the operation. 
        /// </summary>
        protected virtual void OnOperationStarted()
        {
        }

        /// <summary>
        /// The threaded process that we are using.
        /// </summary>
        protected abstract IEnumerator<RoutineInstructions> ProcessOperation();


        /// <summary>
        /// Called on the Main Thread after the operation has complete. 
        /// </summary>
        protected virtual void OnOperationComplete()
        {

        }
    }
}
