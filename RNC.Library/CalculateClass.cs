using System;
using System.Collections.Generic;
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
            _BitwiseOperators = new List<string> { "|", "&", "^" };
            _AllValidOperators = new List<string> { "+", "-", "*", "/", "÷", "**", "%" };
            _AllValidOperators.AddRange(_SingleOperandOperators);
            _AllValidOperators.AddRange(_BitwiseOperators);
        }

        public double? Calculate(ReverseNotationCalculatorClass rnc)
        {
            string value1 = rnc.Value1 != null ? rnc.Value1.ToString() : null;
            string value2 = rnc.Value2 != null ? rnc.Value2.ToString() : null;

            if (IsValidOperation(rnc.Operation) == true)
            {
                if (IsNumeric(value1) == true)
                {
                    if (IsSingleOperandOperation(rnc) == true)
                    {
                        if (rnc.Value2 != null)
                            throw RNCArgumentException(value2, "Value2", nonNullValue2Error: true);
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

        private bool IsSingleOperandOperation(ReverseNotationCalculatorClass rnc) => _SingleOperandOperators.Contains(rnc.Operation) == true;

        private bool IsNegative(string value) => value != null && (value.StartsWith("-") || value.EndsWith("-"));

        private double? BitwiseOperation(ReverseNotationCalculatorClass rnc)
        {
            long? v1 = Convert.ToInt64(rnc.Value1);
            long? v2 = Convert.ToInt64(rnc.Value2);

            switch (rnc.Operation)
            {
                case "&": return v1 & v2;
                case "|": return v1 | v2;
                case "^": return v1 ^ v2;
                default: return null;
            }
        }

        private double? NonBitwiseOperation(ReverseNotationCalculatorClass rnc)
        {
            double? v1 = Convert.ToDouble(rnc.Value1.ToString());
            double? v2 = Convert.ToDouble(rnc.Value2.ToString());

            switch (rnc.Operation)
            {
                case "+": return v1 + v2;
                case "-": return v1 - v2;
                case "*": return v1 * v2;
                case "/":
                case "÷":
                    {
                        if (v2 != 0D)
                            return v1 / v2;
                        else
                            throw new DivideByZeroException("Cannot divide by zero.");
                    }
                case "**":
                    return Math.Pow(v1.Value, v2.Value);
                case "%":
                    return v1 % v2;
                default:
                    return null;
            }
        }

        private double? SingleOperandOperation(string value1, string operation)
        {
            switch(operation)
            {
                case "!":
                    {
                        if (IsFloatingPoint(value1) == true)
                            throw RNCArgumentException(value1, "Value1", floatingPointError: true);
                        else if (IsNegative(value1) == true)
                            throw RNCArgumentException(value1, "Value1", negativeValueError: true);
                        else
                        {
                            ulong value = Convert.ToUInt64(value1);
                            for (ulong index = (value - 1); index > 1; index--)
                                value *= index;
                            return value;
                        }
                    }
                case "sqrt": return Math.Sqrt(Convert.ToDouble(value1));
                case "cbrt": return Math.Cbrt(Convert.ToDouble(value1));
                case "log": return Math.Log(Convert.ToDouble(value1));
                case "log2": return Math.Log2(Convert.ToDouble(value1));
                case "log10": return Math.Log10(Convert.ToDouble(value1));
                case "logB": return Math.ILogB(Convert.ToDouble(value1));
                default: throw new NotImplementedException();
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
