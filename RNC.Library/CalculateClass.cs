using System;
using System.Collections.Generic;
using System.Text;

namespace RNC.Library
{
    public class CalculateClass
    {
        private readonly List<string> _BitwiseOperators;
        private readonly List<string> _AllValidOperators;

        public CalculateClass()
        {
            _BitwiseOperators = new List<string> { "|", "&", "^" };
            _AllValidOperators = new List<string> { "+", "-", "*", "/", "÷", "**" };
            _AllValidOperators.AddRange(_BitwiseOperators);
        }

        public double? Calculate(ReverseNotationCalculatorClass rnc)
        {
            if (IsValidOperation(rnc.Operation) == true)
            {
                if (IsNumeric(rnc.Value1) == true)
                {
                    if (IsNumeric(rnc.Value2) == true)
                    {
                        if (IsBitwiseOperation(rnc.Operation) == false)
                            return NonBitwiseOperation(rnc);
                        else
                        {
                            if (IsFloatingPoint(rnc.Value1) == true)
                                throw RNCArgumentException(rnc.Value1, "Value1", true);
                            else if (IsFloatingPoint(rnc.Value2) == true)
                                throw RNCArgumentException(rnc.Value2, "Value2", true);
                            else
                                return BitwiseOperation(rnc);
                        }
                    }
                    else
                        throw RNCArgumentException(rnc.Value2, "Value2", false);
                }
                else
                    throw RNCArgumentException(rnc.Value1, "Value1", false);
            }
            else
                throw new InvalidOperationException("Operator symbol not valid.");
        }

        private bool IsValidOperation(string operation) => _AllValidOperators.Contains(operation);

        private bool IsNumeric(object value) => double.TryParse(value.ToString(), out _);

        private bool IsBitwiseOperation(string operation) => _BitwiseOperators.Contains(operation);

        private bool IsFloatingPoint(object value)
        {
            string temp = value.ToString();
            return temp.Contains(".");
        }

        private double? BitwiseOperation(ReverseNotationCalculatorClass rnc)
        {
            long? v1 = Convert.ToInt64(rnc.Value1.ToString());
            long? v2 = Convert.ToInt64(rnc.Value2.ToString());

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
                default:
                    return null;
            }
        }

        private Exception RNCArgumentException(object value, string valueName, bool floatingPointError)
        {
            if (value != null)
            {
                var msg = new StringBuilder($"Invalid argument ({value.ToString()}) for [{valueName}].");

                if (floatingPointError == true)
                    msg.Append(" For a bitwise operation, both values must be integers.");

                return new ArgumentException($"Invalid argument ({value.ToString()}) for [{valueName}].");
            }
            else
                return new ArgumentNullException($"[{valueName}] cannot be null.");
        }
    }
}
