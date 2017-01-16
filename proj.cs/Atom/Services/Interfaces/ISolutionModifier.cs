namespace AtomPackageManager.Services
{
    public interface ISolutionModifier
    {
        /// <summary>
        /// Invoked when Unity has generated our solution. 
        /// </summary>
        /// <param name="solutionPath">The path on disk to the solution</param>
        void ModifySolution(string solutionPath, PackageManager packageManager);

        /// <summary>
        ///  The function is used to clone our object when it's needed.
        /// </summary>
        ISolutionModifier CreateCopy();
    }
}
