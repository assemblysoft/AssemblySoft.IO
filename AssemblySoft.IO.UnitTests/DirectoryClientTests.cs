using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblySoft.IO.UnitTests
{
    [TestFixture]
    public class DirectoryClientTests
    {
        public string GetTemporaryDirectory()
            {
                string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempDirectory);
                return tempDirectory;
            }            

            [Test]
            public void CanCopyC5PrebuildAssetsToTarget()
            {                

            DirectoryClient.DirectoryCopy(@"c:\test-dev-source1", @"C:\test-dev-dest1", true, false);            

            Assert.True(true);

            }
        }
}
