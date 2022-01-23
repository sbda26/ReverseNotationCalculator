using Newtonsoft.Json;
using RNC.Library;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RNC.ConsoleAppTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await RunMain();
            return;
        }

        static async Task RunMain()
        {
            // https://www.yogihosting.com/aspnet-core-consume-api/#httpclient
            using (var httpClient = new HttpClient())
            {
                bool done = false;
                do
                {
                    string operation = GetOperation();
                    if (operation.ToUpper() == "Q")
                        done = true;
                    else
                    {
                        bool singleOperandOperator = new CalculateClass().IsSingleOperandOperator(operation);
                        string operand1 = GetOperand1(singleOperandOperator);
                        string operand2 = (singleOperandOperator == true) ? null : GetOperand2();
                        double? result = await GetResult(httpClient, operand1, operand2, operation);
                        Console.WriteLine(result);
                    }
                } while (done == false);
            }
        }

        static string GetOperation()
        {
            Console.Write("Enter operation (enter 'Q' to quit): ");
            return Console.ReadLine();
        }

        static string GetOperand1(bool singleOperandOperator)
        {
            Console.Write("Enter operand");
            if (singleOperandOperator == false)
                Console.Write(" #1");
            Console.Write(": ");

            return Console.ReadLine();
        }

        static string GetOperand2()
        {
            Console.Write("Enter operand #2: ");
            return Console.ReadLine();
        }

        private static async Task<double?> GetResult(HttpClient httpClient, string operand1, string operand2, string operation)
        {
            var data = new ReverseNotationCalculatorClass { Value1 = operand1, Value2 = operand2, Operation = operation };
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            double? result;

            using (var response = await httpClient.PostAsync("http://localhost:34626/api/RNC", content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<double?>(apiResponse);
            }

            return result;
        }
    }
}
