using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Type.BigInteger.Tests
{
    [TestClass]
    public class BigIntegerTests
    {
        [DataTestMethod]
        [DynamicData(nameof(DataSet.Add), typeof(DataSet), DynamicDataSourceType.Method)]
        public void AddTest(string input1, string input2, string output)
        {
            var operand1 = new BigInteger(input1);
            var operand2 = new BigInteger(input2);
            var actual = operand1 + operand2;
            var expected = new BigInteger(output);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataSet.Subtract), typeof(DataSet), DynamicDataSourceType.Method)]
        public void SubtractTest(string input1, string input2, string output)
        {
            var operand1 = new BigInteger(input1);
            var operand2 = new BigInteger(input2);
            var actual = operand1 - operand2;
            var expected = new BigInteger(output);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataSet.Multiply), typeof(DataSet), DynamicDataSourceType.Method)]
        public void MultiplyTest(string input1, string input2, string output)
        {
            var operand1 = new BigInteger(input1);
            var operand2 = new BigInteger(input2);
            var actual = operand1 * operand2;
            var expected = new BigInteger(output);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataSet.Divide), typeof(DataSet), DynamicDataSourceType.Method)]
        public void DivideTest(string input1, string input2, string quotient, string _)
        {
            var operand1 = new BigInteger(input1);
            var operand2 = new BigInteger(input2);
            var actual = operand1 / operand2;
            var expected = new BigInteger(quotient);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataSet.Divide), typeof(DataSet), DynamicDataSourceType.Method)]
        public void ModulusTest(string input1, string input2, string _, string remainder)
        {
            var operand1 = new BigInteger(input1);
            var operand2 = new BigInteger(input2);
            var actual = operand1 % operand2;
            var expected = new BigInteger(remainder);
            Assert.AreEqual(expected, actual);
        }
    }
}