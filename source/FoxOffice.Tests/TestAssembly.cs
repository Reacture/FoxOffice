[assembly: Microsoft.VisualStudio.TestTools.UnitTesting.Parallelize]

namespace FoxOffice
{
    using System.Threading.Tasks;
    using FoxOffice.ReadModel.DataAccess;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public static class TestAssembly
    {
        [AssemblyInitialize]
        public static async Task Initialize(TestContext context)
        {
            await CosmosDb.Initialize();
        }
    }
}
