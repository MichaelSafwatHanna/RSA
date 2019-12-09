using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Type.BigInteger.Tests
{
    [TestClass]
    public class BigIntegerTests
    {
        [DataTestMethod]
        [DataRow(Cases.Add.Case1.Input1, Cases.Add.Case1.Input2, Cases.Add.Case1.Output, DisplayName = Cases.Add.Case1.Name)]
        [DataRow(Cases.Add.Case2.Input1, Cases.Add.Case2.Input2, Cases.Add.Case2.Output, DisplayName = Cases.Add.Case2.Name)]
        [DataRow(Cases.Add.Case3.Input1, Cases.Add.Case3.Input2, Cases.Add.Case3.Output, DisplayName = Cases.Add.Case3.Name)]
        [DataRow(Cases.Add.Case4.Input1, Cases.Add.Case4.Input2, Cases.Add.Case4.Output, DisplayName = Cases.Add.Case4.Name)]
        [DataRow(Cases.Add.Case5.Input1, Cases.Add.Case5.Input2, Cases.Add.Case5.Output, DisplayName = Cases.Add.Case5.Name)]
        [DataRow(Cases.Add.Case6.Input1, Cases.Add.Case6.Input2, Cases.Add.Case6.Output, DisplayName = Cases.Add.Case6.Name)]
        [DataRow(Cases.Add.Case7.Input1, Cases.Add.Case7.Input2, Cases.Add.Case7.Output, DisplayName = Cases.Add.Case7.Name)]
        [DataRow(Cases.Add.Case8.Input1, Cases.Add.Case8.Input2, Cases.Add.Case8.Output, DisplayName = Cases.Add.Case8.Name)]
        public void AddTest(string input1, string input2, string output)
        {
            var operand1 = new BigInteger(input1);
            var operand2 = new BigInteger(input2);
            var actual = operand1 + operand2;
            var expected = new BigInteger(output);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(Cases.Sub.Case1.Input1, Cases.Sub.Case1.Input2, Cases.Sub.Case1.Output, DisplayName = Cases.Sub.Case1.Name)]
        [DataRow(Cases.Sub.Case2.Input1, Cases.Sub.Case2.Input2, Cases.Sub.Case2.Output, DisplayName = Cases.Sub.Case2.Name)]
        [DataRow(Cases.Sub.Case3.Input1, Cases.Sub.Case3.Input2, Cases.Sub.Case3.Output, DisplayName = Cases.Sub.Case3.Name)]
        [DataRow(Cases.Sub.Case4.Input1, Cases.Sub.Case4.Input2, Cases.Sub.Case4.Output, DisplayName = Cases.Sub.Case4.Name)]
        [DataRow(Cases.Sub.Case5.Input1, Cases.Sub.Case5.Input2, Cases.Sub.Case5.Output, DisplayName = Cases.Sub.Case5.Name)]
        [DataRow(Cases.Sub.Case6.Input1, Cases.Sub.Case6.Input2, Cases.Sub.Case6.Output, DisplayName = Cases.Sub.Case6.Name)]
        [DataRow(Cases.Sub.Case7.Input1, Cases.Sub.Case7.Input2, Cases.Sub.Case7.Output, DisplayName = Cases.Sub.Case7.Name)]
        [DataRow(Cases.Sub.Case8.Input1, Cases.Sub.Case8.Input2, Cases.Sub.Case8.Output, DisplayName = Cases.Sub.Case8.Name)]
        [DataRow(Cases.Sub.Case9.Input1, Cases.Sub.Case9.Input2, Cases.Sub.Case9.Output, DisplayName = Cases.Sub.Case9.Name)]
        [DataRow(Cases.Sub.Case10.Input1, Cases.Sub.Case10.Input2, Cases.Sub.Case10.Output, DisplayName = Cases.Sub.Case10.Name)]
        [DataRow(Cases.Sub.Case11.Input1, Cases.Sub.Case11.Input2, Cases.Sub.Case11.Output, DisplayName = Cases.Sub.Case11.Name)]
        [DataRow(Cases.Sub.Case12.Input1, Cases.Sub.Case12.Input2, Cases.Sub.Case12.Output, DisplayName = Cases.Sub.Case12.Name)]
        [DataRow(Cases.Sub.Case13.Input1, Cases.Sub.Case13.Input2, Cases.Sub.Case13.Output, DisplayName = Cases.Sub.Case13.Name)]
        [DataRow(Cases.Sub.Case14.Input1, Cases.Sub.Case14.Input2, Cases.Sub.Case14.Output, DisplayName = Cases.Sub.Case14.Name)]
        [DataRow(Cases.Sub.Case15.Input1, Cases.Sub.Case15.Input2, Cases.Sub.Case15.Output, DisplayName = Cases.Sub.Case15.Name)]
        public void SubtractTest(string input1, string input2, string output)
        {
            var operand1 = new BigInteger(input1);
            var operand2 = new BigInteger(input2);
            var actual = operand1 - operand2;
            var expected = new BigInteger(output);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(Cases.Mul.Case1.Input1, Cases.Mul.Case1.Input2, Cases.Mul.Case1.Output, DisplayName = Cases.Mul.Case1.Name)]
        [DataRow(Cases.Mul.Case2.Input1, Cases.Mul.Case2.Input2, Cases.Mul.Case2.Output, DisplayName = Cases.Mul.Case2.Name)]
        [DataRow(Cases.Mul.Case3.Input1, Cases.Mul.Case3.Input2, Cases.Mul.Case3.Output, DisplayName = Cases.Mul.Case3.Name)]
        [DataRow(Cases.Mul.Case4.Input1, Cases.Mul.Case4.Input2, Cases.Mul.Case4.Output, DisplayName = Cases.Mul.Case4.Name)]
        public void MultiplyTest(string input1, string input2, string output)
        {
            var operand1 = new BigInteger(input1);
            var operand2 = new BigInteger(input2);
            var actual = operand1 * operand2;
            var expected = new BigInteger(output);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(Cases.Div.Case1.Input1, Cases.Div.Case1.Input2, Cases.Div.Case1.OutputQ, Cases.Div.Case1.OutputR, DisplayName = Cases.Div.Case1.Name)]
        [DataRow(Cases.Div.Case2.Input1, Cases.Div.Case2.Input2, Cases.Div.Case2.OutputQ, Cases.Div.Case2.OutputR, DisplayName = Cases.Div.Case2.Name)]
        [DataRow(Cases.Div.Case3.Input1, Cases.Div.Case3.Input2, Cases.Div.Case3.OutputQ, Cases.Div.Case3.OutputR, DisplayName = Cases.Div.Case3.Name)]
        [DataRow(Cases.Div.Case4.Input1, Cases.Div.Case4.Input2, Cases.Div.Case4.OutputQ, Cases.Div.Case4.OutputR, DisplayName = Cases.Div.Case4.Name)]
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