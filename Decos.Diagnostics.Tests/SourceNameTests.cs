using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.Diagnostics.Tests
{
    [TestClass]
    public class SourceNameTests
    {
        [TestMethod]
        public void SourceNameShouldEqualSourceNameWithEqualCapitalization_IEquatable()
        {
            SourceName a = "Decos.Diagnostics.Tests";
            SourceName b = "Decos.Diagnostics.Tests";
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void SourceNameShouldNotEqualSourceNameWithDifferentCapitalization_IEquatable()
        {
            SourceName a = "Decos.Diagnostics.Tests";
            SourceName b = "decos.diagnostics.tests";
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void SourceNameShouldEqualSourceNameWithEqualCapitalization_ObjectEquals()
        {
            SourceName a = "Decos.Diagnostics.Tests";
            SourceName b = "Decos.Diagnostics.Tests";
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void SourceNameShouldNotEqualSourceNameWithDifferentCapitalization_ObjectEquals()
        {
            SourceName a = "Decos.Diagnostics.Tests";
            SourceName b = "decos.diagnostics.tests";
            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void SourceNameShouldEqualSourceNameWithEqualCapitalization_EqualityOperator()
        {
            SourceName a = "Decos.Diagnostics.Tests";
            SourceName b = "Decos.Diagnostics.Tests";
            Assert.IsTrue(a == b);
        }

        [TestMethod]
        public void SourceNameShouldNotEqualSourceNameWithDifferentCapitalization_EqualityOperator()
        {
            SourceName a = "Decos.Diagnostics.Tests";
            SourceName b = "decos.diagnostics.tests";
            Assert.IsFalse(a == b);
        }

        [TestMethod]
        public void SourceNameShouldEqualSourceNameWithEqualCapitalization_InequalityOperator()
        {
            SourceName a = "Decos.Diagnostics.Tests";
            SourceName b = "Decos.Diagnostics.Tests";
            Assert.IsFalse(a != b);
        }

        [TestMethod]
        public void SourceNameShouldNotEqualSourceNameWithDifferentCapitalization_InequalityOperator()
        {
            SourceName a = "Decos.Diagnostics.Tests";
            SourceName b = "decos.diagnostics.tests";
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void SourceNameHashCodeIsEqualForEqualNames()
        {
            SourceName a = "A";
            SourceName b = "A";
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void SourceNameHashCodeIsUniqueForDifferentNames()
        {
            SourceName a = "A";
            SourceName b = "a";
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void SourceNameCanBeCastFromString()
        {
            const string name = "Decos.Diagnostics.Tests";
            SourceName a = name;
            Assert.AreEqual(a.Name, "Decos.Diagnostics.Tests");
        }

        [TestMethod]
        public void SourceNameCanBeCastFromNull()
        {
            const string name = null;
            SourceName a = name;
            Assert.IsNull(a);
        }

        [TestMethod]
        public void SourceNameCanBeCastToString()
        {
            SourceName a = "Decos.Diagnostics.Tests";
            string name = a;
            Assert.AreEqual(name, a.Name);
        }

        [TestMethod]
        public void SourceNameCanBeCastToNull()
        {
            SourceName a = null;
            string name = a;
            Assert.IsNull(name);
        }

        [TestMethod]
        public void SourceNameUsesFullNameFromType()
        {
            var sourceName = SourceName.FromType<SourceNameTests>();
            Assert.AreEqual(sourceName.Name, GetType().FullName);
        }

        [DataTestMethod]
        [DataRow("A", "B")]
        [DataRow("A", "a")]
        [DataRow(null, "A")]
        [DataRow("Decos.Diagnostics.Tests", "Decos.Diagnostics.Trace")]
        [DataRow("Decos.Diagnostics.Trace", "Decos.Diagnostics")]
        [DataRow("Decos.Diagnostics.Trace", "Decos")]
        [DataRow("Decos.Diagnostics", "Decos")]
        public void SourceNameShouldBeSortedAlphabetically_LessThan(string a, string b)
        {
            SourceName x = a;
            SourceName y = b;
            Assert.IsTrue(x < y);
        }

        [DataTestMethod]
        [DataRow("B", "A")]
        [DataRow("a", "A")]
        [DataRow("A", null)]
        [DataRow("Decos", "Decos.Diagnostics")]
        [DataRow("Decos.Diagnostics", "Decos.Diagnostics.Trace")]
        [DataRow("Decos", "Decos.Diagnostics.Trace")]
        public void SourceNameShouldBeSortedAlphabetically_GreaterThan(string a, string b)
        {
            SourceName x = a;
            SourceName y = b;
            Assert.IsTrue(x > y);
        }

        [DataTestMethod]
        [DataRow("A", "B")]
        [DataRow("A", "a")]
        [DataRow("A", "A")]
        [DataRow(null, "A")]
        [DataRow(null, null)]
        [DataRow("Decos.Diagnostics.Trace", "Decos.Diagnostics")]
        [DataRow("Decos.Diagnostics.Trace", "Decos")]
        [DataRow("Decos.Diagnostics", "Decos")]
        public void SourceNameShouldBeSortedAlphabetically_LessThanOrEqual(string a, string b)
        {
            SourceName x = a;
            SourceName y = b;
            Assert.IsTrue(x <= y);
        }

        [DataTestMethod]
        [DataRow("B", "A")]
        [DataRow("a", "A")]
        [DataRow("A", "A")]
        [DataRow("A", null)]
        [DataRow(null, null)]
        [DataRow("Decos", "Decos.Diagnostics")]
        [DataRow("Decos.Diagnostics", "Decos.Diagnostics.Trace")]
        [DataRow("Decos", "Decos.Diagnostics.Trace")]
        public void SourceNameShouldBeSortedAlphabetically_GreaterThanOrEqual(string a, string b)
        {
            SourceName x = a;
            SourceName y = b;
            Assert.IsTrue(x >= y);
        }

        [DataTestMethod]
        [DataRow("Decos.Diagnostics.Tests", "Decos.Diagnostics")]
        [DataRow("Decos.Diagnostics", "Decos")]
        public void SourceNameWithMultiplePartsShouldHaveParent(string value, string expected)
        {
            var x = new SourceName(value);
            Assert.AreEqual(expected, x.Parent.Name);
        }

        [TestMethod]
        public void SourceNameWithSinglePartShouldNotHaveParent()
        {
            var x = new SourceName("Decos");
            Assert.IsNull(x.Parent);
        }

        [DataTestMethod]
        [DataRow("Decos.Diagnostics.Trace", "Decos.Diagnostics.Trace", true)]
        [DataRow("Decos.Diagnostics.Trace", "Decos.Diagnostics", true)]
        [DataRow("Decos.Diagnostics.Trace", "Decos", true)]
        [DataRow("Decos.Diagnostics.Trace", "Deco", false)]
        [DataRow("Decos.Diagnostics.Trace", "Decos.Diagnostics.Trace.Tests", false)]
        [DataRow("Decos.Diagnostics.Trace", "Decos.Diagnostics.Tests", false)]
        [DataRow("Decos.Diagnostics.Trace", "Microsoft", false)]
        public void SourceNameShouldMatchFilter(string value, string filter, bool expected)
        {
            var sourceName = new SourceName(value);
            Assert.AreEqual(expected, sourceName.Matches(filter));
        }
    }
}
