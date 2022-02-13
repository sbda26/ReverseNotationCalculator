using System;
using System.Collections.Generic;

namespace RNC.Library
{
    public static class OperatorsClass
    {
        private readonly static List<string> SingleOperandOperators = new List<string> { "!", "sqrt", "cbrt", "log", "log2", "log10", "logB" };
        private readonly static List<string> BitwiseOperators = new List<string> { "|", "&", "^", "OR", "AND", "XOR", "<<", ">>" };
        public readonly static List<string> AllValidOperators;

        static OperatorsClass()
        {
            AllValidOperators = new List<string> { "+", "-", "*", "/", "÷", "**", "%", "MED" };
            AllValidOperators.AddRange(SingleOperandOperators);
            AllValidOperators.AddRange(BitwiseOperators);
        }

        public static bool IsNumeric(string value) => double.TryParse(value, out _);

        public static bool IsBitwiseOperation(string operation) => BitwiseOperators.Contains(operation);

        public static bool IsFloatingPoint(string value) => double.TryParse(value, out _) == true && long.TryParse(value, out _) == false;

        public static bool IsNegative(string value) => value != null && (value.StartsWith("-") || value.EndsWith("-"));

        public static bool IsSingleOperandOperator(string operation)
        {
            if (IsValidOperation(operation) == false)
                throw new InvalidOperationException("Operator symbol not valid.");
            else
                return IsSingleOperandOperation(operation);
        }

        public static bool IsSingleOperandOperation(string operation) => SingleOperandOperators.Contains(operation) == true;
        public static bool IsValidOperation(string operation) => AllValidOperators.Contains(operation);
    }
}
