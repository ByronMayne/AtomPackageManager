using AtomPackageManager.Packages;
using System.CodeDom.Compiler;

namespace AtomPackageManager.Services
{
    /// <summary>
    /// This delegate is used when a <see cref="ICompilerService.CompilePackage(AtomPackage)"/> has
    /// completed compiling. 
    /// </summary>
    /// <param name="ICompilerService">The service that compiled the code.</param>
    /// <param name="package">The package that was requested to be compiled.</param>
    public delegate void OnCompileCompleteDelegate(ICompilerService compilerService, AtomPackage package);

    public interface ICompilerService
    {
        /// <summary>
        /// Returns true if the compile service was successful and false if it was not.
        /// </summary>
        bool wasSuccessful { get; }
        
        /// <summary>
        /// Used to invoke to start compiling a package. 
        /// </summary>
        /// <param name="package">The package that is being requested to compile</param>
        void CompilePackage(AtomPackage package, OnCompileCompleteDelegate onComplete);

        /// <summary>
        /// Gets the Compiler Errors that were generated if there was any. 
        /// </summary>
        CompilerErrorCollection GetErrors();

        /// <summary>
        /// Creates a shallow copy of this object
        /// </summary>
        ICompilerService CreateCopy();
    }


}
