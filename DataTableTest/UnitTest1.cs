using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;


namespace DataTableTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DataTable dt = new DataTable("Test");
            DataColumn dc= dt.Columns[0];
            dt.Columns.Add(dc);
            dt.Select("filter", "sort");


        }
    }
}
