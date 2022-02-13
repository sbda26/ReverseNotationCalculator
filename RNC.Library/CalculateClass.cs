using System;
using System.Numerics;
using System.Text;

namespace RNC.Library
{
    public class CalculateClass
    {
        public string Calculate(ReverseNotationCalculatorClass rnc)
        {
            string value1 = rnc.Value1?.ToString();
            string value2 = rnc.Value2?.ToString();

            if (OperatorsClass.IsValidOperation(rnc.Operation) == true)
            {
                if (OperatorsClass.IsNumeric(value1) == true)
                {
                    if (OperatorsClass.IsSingleOperandOperation(rnc.Operation) == true)
                    {
                        if (rnc.Value2 != null)
                            throw RNCArgumentException(value2, "Value2", nonNullValue2Error: true);
                        else
                            return SingleOperandOperation(value1, rnc.Operation);
                    }
                    else if (OperatorsClass.IsNumeric(value2) == true)
                    {
                        if (OperatorsClass.IsBitwiseOperation(rnc.Operation) == false)
                            return NonBitwiseOperation(rnc);
                        else
                        {
                            if (OperatorsClass.IsFloatingPoint(value1) == true)
                                throw RNCArgumentException(value1, "Value1", floatingPointError: true);
                            else if (OperatorsClass.IsFloatingPoint(value2) == true)
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
                case "<<":
                    result = (int)v1.Value << (int)v2.Value;
                    break;
                case ">>":
                    result = (int)v1.Value >> (int)v2.Value;
                    break;
                default:
                    result = null;
                    break;
            }

            return result?.ToString();
        }

        private string NonBitwiseOperation(ReverseNotationCalculatorClass rnc)
        {
            string value1Str = rnc.Value1.ToString();
            string value2Str = rnc.Value2.ToString();
            double? value1D = Convert.ToDouble(value1Str);
            double? value2D = Convert.ToDouble(value2Str);

            if (rnc.Operation != "**")
            {
                double? result;

                switch (rnc.Operation)
                {
                    case "+": result = value1D + value2D; break;
                    case "-": result = value1D - value2D; break;
                    case "*": result = value1D * value2D; break;
                    case "/":
                    case "÷":
                        {
                            if (value2D != 0D)
                            {
                                result = value1D / value2D;
                                break;
                            }
                            else
                                throw new DivideByZeroException("Cannot divide by zero.");
                        }
                    case "%":
                        result = value1D % value2D; break;
                    case "MED":
                        result = Median(value1D.Value, value2D.Value); break;
                    default:
                        result = null; break;
                }

                return result?.ToString();
            }
            else
            {
                if (OperatorsClass.IsFloatingPoint(value1Str) == true || OperatorsClass.IsFloatingPoint(value2Str) == true)
                    return Math.Pow(value1D.Value, value2D.Value).ToString();
                else
                {
                    BigInteger valueBigInt = Convert.ToUInt64(value1D);
                    int exponent = Convert.ToInt32(value2D);
                    BigInteger result = BigInteger.Pow(valueBigInt, exponent);
                    return result.ToString();
                }
            }
        }

        private string SingleOperandOperation(string value1, string operation)
        {
            if (operation == "!")
                return Factorial(value1);
            else
            {
                double dValue = Convert.ToDouble(value1);
                double result = operation switch
                {
                    "sqrt" => Math.Sqrt(dValue),
                    "cbrt" => Math.Cbrt(dValue),
                    "log" => Math.Log(dValue),
                    "log2" => Math.Log2(dValue),
                    "log10" => Math.Log10(dValue),
                    "logB" => Math.ILogB(dValue),
                    _ => throw new NotImplementedException(),
                };
                return result.ToString();
            }
        }

        private string Factorial(string valueStr)
        {
            if (OperatorsClass.IsFloatingPoint(valueStr) == true)
                throw RNCArgumentException(valueStr, "Value1", floatingPointError: true);
            else if (OperatorsClass.IsNegative(valueStr) == true)
                throw RNCArgumentException(valueStr, "Value1", negativeValueError: true);
            else
            {
                BigInteger valueBigInt = Convert.ToUInt64(valueStr);
                for (BigInteger index = (valueBigInt - 1); index > 1; index--)
                    valueBigInt *= index;
                return valueBigInt.ToString();
            }
        }

        private double? Median(double? v1, double? v2)
        {
            if (v1 == null || v2 == null)
                throw new ArgumentNullException("[v1] and [v2] must both be non-null.");
            else if (v1 == v2)
                return v1;
            else
            {
                if (v1 > v2)
                {
                    double? temp = v1;
                    v2 = v1;
                    v1 = temp;
                }
                return v1.Value + ((v2.Value - v1.Value) / 2);
            }
        }

        private Exception RNCArgumentException(string value, string valueName,
                                               bool floatingPointError = false, bool negativeValueError = false, bool nonNullValue2Error = false)
        {
            if (value == null)
                throw new ArgumentNullException("Value cannot be null.");
            else
            {
                var msg = new StringBuilder($"Invalid argument ({value}) for [{valueName}].");

                if (!floatingPointError && !negativeValueError && !nonNullValue2Error)
                    return new ArgumentException($"Invalid argument ({value}) for [{valueName}].");
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
