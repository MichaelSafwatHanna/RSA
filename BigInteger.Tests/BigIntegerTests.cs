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
        public void DivideTest(string input1, string input2, string outputQ, string outputR)
        {
            var operand1 = new BigInteger(input1);
            var operand2 = new BigInteger(input2);
            var actualQ = operand1 / operand2;
            var actualR = operand1 % operand2;
            var expectedQ = new BigInteger(outputQ);
            var expectedR = new BigInteger(outputR);
            Assert.AreEqual(expectedQ, actualQ);
            Assert.AreEqual(expectedR, actualR);
        }
    }
}