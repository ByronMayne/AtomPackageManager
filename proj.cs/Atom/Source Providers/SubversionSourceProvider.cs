using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPackageManager
{
    public class SubversionSourceProvider : ISourceControlProvider
    {
        public void Checkout()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void SetSourceURI(string uri)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}