using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RNC.Library
{
    public class CalculateClass
    {
        private readonly List<string> _SingleOperandOperators;
        private readonly List<string> _BitwiseOperators;
        private readonly List<string> _AllValidOperators;

        public CalculateClass()
        {
            _SingleOperandOperators = new List<string> { "!", "sqrt", "cbrt", "log", "log2", "log10", "logB" };
            _BitwiseOperators = new List<string> { "|", "&", "^", "OR", "AND", "XOR" };
            _AllValidOperators = new List<string> { "+", "-", "*", "/", "÷", "**", "%" };
            _AllValidOperators.AddRange(_SingleOperandOperators);
            _AllValidOperators.AddRange(_BitwiseOperators);
        }

        public bool IsSingleOperandOperator(string operation)  // used by HttpClient programs
        {
            if (IsValidOperation(operation) == false)
                throw new InvalidOperationException("Operator symbol not valid.");
            else
                return IsSingleOperandOperation(operation);
        }

        public string Calculate(ReverseNotationCalculatorClass rnc)
        {
            string value1 = rnc.Value1?.ToString();
            string value2 = rnc.Value2?.ToString();

            if (IsValidOperation(rnc.Operation) == true)
            {
                if (IsNumeric(value1) == true)
                {
                    if (IsSingleOperandOperation(rnc.Operation) == true)
                    {
                        if (rnc.Value2 != null)
                            throw RNCArgumentException(value2, "Value2", nonNullValue2Error: true);
                        else if (rnc.Operation == "!")
                            return Factorial(rnc.Value1);
                        else
                            return SingleOperandOperation(value1, rnc.Operation);
                    }
                    else if (IsNumeric(value2) == true)
                    {
                        if (IsBitwiseOperation(rnc.Operation) == false)
                            return NonBitwiseOperation(rnc);
                        else
                        {
                            if (IsFloatingPoint(value1) == true)
                                throw RNCArgumentException(value1, "Value1", floatingPointError: true);
                            else if (IsFloatingPoint(value2) == true)
                                throw RNCArgumentException(value2, "Value2", floatingPointError: true);
                            else
                                return BitwiseOperation(rnc);
                        }
                    }
                    else
                        throw RNCArgumentException(value2, "Value2");
                }
                else
                    throw RNCArgumentException(value1, "Value1");
            }
            else
                throw new InvalidOperationException("Operator symbol not valid.");
        }

        private bool IsValidOperation(string operation) => _AllValidOperators.Contains(operation);

        private bool IsNumeric(string value) => double.TryParse(value, out _);

        private bool IsBitwiseOperation(string operation) => _BitwiseOperators.Contains(operation);

        private bool IsFloatingPoint(string value) => double.TryParse(value, out _) == true && long.TryParse(value, out _) == false;

        private bool IsSingleOperandOperation(string operation) => _SingleOperandOperators.Contains(operation) == true;

        private bool IsNegative(string value) => value != null && (value.StartsWith("-") || value.EndsWith("-"));

        private string BitwiseOperation(ReverseNotationCalculatorClass rnc)
        {
            long? v1 = Convert.ToInt64(rnc.Value1.ToString());
            long? v2 = Convert.ToInt64(rnc.Value2.ToString());
            long? result;

            switch (rnc.Operation)
            {
                case "&":
                case "AND":
                    result = v1 & v2;
                    break;
                case "|":
                case "OR":
                    result = v1 | v2;
                    break;
                case "^":
                case "XOR":
                    result = v1 ^ v2;
                    break;
                default:
                    result = null;
                    break;
            }

            return result?.ToString();
        }

        private string NonBitwiseOperation(ReverseNotationCalculatorClass rnc)
        {
            double? v1 = Convert.ToDouble(rnc.Value1.ToString());
            double? v2 = Convert.ToDouble(rnc.Value2.ToString());
            double? result;

            switch (rnc.Operation)
            {
                case "+": result = v1 + v2; break;
                case "-": result = v1 - v2; break;
                case "*": result = v1 * v2; break;
                case "/":
                case "÷":
                    {
                        if (v2 != 0D)
                        {
                            result = v1 / v2;
                            break;
                        }
                        else
                            throw new DivideByZeroException("Cannot divide by zero.");
                    }
                case "**":
                    result = Math.Pow(v1.Value, v2.Value); break;
                case "%":
                    result = v1 % v2; break;
                default:
                    result = null; break;
            }

            return result?.ToString();
        }

        private string SingleOperandOperation(string value1, string operation)
        {
            double? result;

            switch(operation)
            {
                case "sqrt": result = Math.Sqrt(Convert.ToDouble(value1));  break;
                case "cbrt": result = Math.Cbrt(Convert.ToDouble(value1)); break;
                case "log": result = Math.Log(Convert.ToDouble(value1)); break;
                case "log2": result = Math.Log2(Convert.ToDouble(value1)); break;
                case "log10": result = Math.Log10(Convert.ToDouble(value1)); break;
                case "logB": result = Math.ILogB(Convert.ToDouble(value1)); break;
                default: throw new NotImplementedException();
            }

            return result?.ToString();
        }

        private string Factorial(object valueObj)
        {
            string valueStr = valueObj?.ToString();

            if (IsFloatingPoint(valueStr) == true)
                throw RNCArgumentException(valueStr, "Value1", floatingPointError: true);
            else if (IsNegative(valueStr) == true)
                throw RNCArgumentException(valueStr, "Value1", negativeValueError: true);
            else
            {
                ulong valueULong = Convert.ToUInt64(((System.Text.Json.JsonElement)valueObj).ToString());
                BigInteger valueBigInt = valueULong;
                for (BigInteger index = (valueBigInt - 1); index > 1; index--)
                    valueBigInt *= index;
                return valueBigInt.ToString();
            }
        }

        private Exception RNCArgumentException(string value, string valueName,
                                               bool floatingPointError = false, bool negativeValueError = false, bool nonNullValue2Error = false)
        {
            if (value == null)
                throw new ArgumentNullException("Value cannot be null.");
            else
            {
                var msg = new StringBuilder($"Invalid argument ({value.ToString()}) for [{valueName}].");

                if (!floatingPointError && !negativeValueError && !nonNullValue2Error)
                    return new ArgumentException($"Invalid argument ({value.ToString()}) for [{valueName}].");
                else
                {
                    if (floatingPointError)
                        msg.AppendLine("All values must be integers for this operation.");
                    if (negativeValueError)
                        msg.AppendLine("Value cannot be negative.");
                    if (nonNullValue2Error)
                        msg.AppendLine("Value2 must be null. Only one value is allowed.");

                    return new ArgumentException(msg.ToString());
                }
            }
        }
    }
}
