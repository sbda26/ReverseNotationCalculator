using Flurl.Http;
using Newtonsoft.Json;
using RNC.Library;
using RestSharp;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RNC.ConsoleAppTest
{
    class Program
    {
        private const string _URL = "http://localhost:34626/api/RNC";

        private delegate Task<string> ApiClientDelegate(ReverseNotationCalculatorClass data);
        private readonly static ApiClientDelegate[] _Methods = { GetResult_HttpClient, GetResult_RestSharp, GetResult_Flurl };

        static async Task Main(string[] args)
        {
            await RunMain();
            return;
        }

        static async Task RunMain()
        {
            // https://www.yogihosting.com/aspnet-core-consume-api/#httpclient
            // https://code-maze.com/different-ways-consume-restful-api-csharp/#HttpClient

            bool done = false;
            do
            {
                string operation = GetMathematicalOperation();
                if (operation.ToUpper() == "Q")
                    done = true;
                else
                {
                    bool singleOperandOperator = OperatorsClass.IsSingleOperandOperator(operation);
                    string operand1 = GetOperand1(singleOperandOperator);
                    string operand2 = (singleOperandOperator == true) ? null : GetOperand2();
                    var data = new ReverseNotationCalculatorClass { Value1 = operand1, Value2 = operand2, Operation = operation };

                    await RunApiClients(data);
                    Console.WriteLine("***");
                }
            } while (done == false);
        }

        static string GetMathematicalOperation()
        {
            int limit = OperatorsClass.AllValidOperators.Count - 1;

            Console.Write("Enter mathematical operation (");
            for (int index = 0; index <= limit; index++)
            {
                Console.Write($"{OperatorsClass.AllValidOperators[index]}");
                if (index < limit)
                    Console.Write(", ");
                else
                {
                    Console.WriteLine(")");
                    Console.Write("or 'Q' to quit: ");
                }
            }

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

        private static async Task<string> GetResult_HttpClient(ReverseNotationCalculatorClass data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string result;

            using (var client = new HttpClient())
            {
                using (var response = await client.PostAsync(_URL, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<string>(apiResponse);
                }
            }

            return result;
        }

        // https://stackoverflow.com/questions/31937939/restsharp-post-a-json-object
        private static async Task<string> GetResult_RestSharp(ReverseNotationCalculatorClass data)
        {
            var request = new RestRequest(_URL, Method.Post);
            request.AddJsonBody(data);

            var client = new RestClient(_URL);
            var response = await client.ExecuteAsync(request);

            return response.Content.Replace("\"", string.Empty);
        }

        // https://flurl.dev/docs/fluent-http/
        // https://jonathancrozier.com/blog/calling-all-apis-how-to-use-flurl-with-c-sharp
        private static async Task<string> GetResult_Flurl(ReverseNotationCalculatorClass data) =>
            await _URL.PostJsonAsync(data).ReceiveString();

        private static async Task RunApiClients(ReverseNotationCalculatorClass data)
        {
            foreach (ApiClientDelegate method in _Methods)
            {
                DateTime start = DateTime.Now;
                string result = await method.Invoke(data);
                double elapsedTime = (DateTime.Now - start).Milliseconds;
                Console.WriteLine("***");
                Console.WriteLine($"RESULT: {result}");
                Console.WriteLine($"METHOD: {method.Method.Name}");
                Console.WriteLine($"ELAPSED TIME (ms): {elapsedTime}");
                Console.WriteLine();
            }
        }

    }
}
