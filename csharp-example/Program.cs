using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace csharp_example
{
    class Program
    {
        // You can find these values by visiting https://app.ip1sms.com/settings/#api
        const string account = "ip1-yyyy";
        const string apiKey = "xxxxxxxxxxxxxxxxxxxxxxx";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Welcome to the SMS sending service");
                Console.WriteLine("Please provide a phone number");
                var recipient = Console.ReadLine();

                Console.WriteLine("Please write the message you want to send");
                var message = Console.ReadLine();

                Console.WriteLine("Who's sending the message");
                var sender = Console.ReadLine();

                Console.WriteLine("This is a summary of your message");
                Console.WriteLine("\tFrom: " + sender);
                Console.WriteLine("\tTo: " + recipient);
                Console.WriteLine("\tMessage: " + message);

                Console.WriteLine("Do you want to send it? Write yes or no");
                var answer = Console.ReadLine();
                if (answer == "yes")
                {
                    var task = SendSms(sender, recipient, message);
                    task.Wait();
                }

                Console.WriteLine();
            }
        }
        public static async Task SendSms(string sender, string recipient, string messageBody)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.ip1sms.com");

                // Setting authentication
                byte[] authBytes = Encoding.UTF8.GetBytes($"{account}:{apiKey}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

                // Setting user agent
                var version = typeof(Program).Assembly
                    .GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("IP1.Example", version));

                var outgoingSms = new OutgoingSMS()
                {
                    Numbers = new List<string>() { recipient },
                    Message = messageBody,
                    From = sender

                };

                // Serialize the object into JSON
                var json = JsonConvert.SerializeObject(outgoingSms);
                StringContent requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/sms/send", requestContent);


                // The body is only filled with messages if the response is a success
                if (response.IsSuccessStatusCode)
                {
                    var messages = JsonConvert.DeserializeObject<List<ProcessedSms>>(await response.Content.ReadAsStringAsync());

                    // We can be certain that there is at least one message returned as we would have other wise gotten Bad Request as a response
                    var message = messages[0];

                    // Induvidual SMS might fail, this still is checked here
                    // Description for what all these mean can be found here: https://developer.ip1sms.com/REST-API#status-codes
                    switch (message.Status)
                    {
                        case 0:
                        case 11:
                        case 21:
                        case 22:
                            Console.WriteLine("SMS was sent successfully");
                            break;
                        case 42:
                            Console.WriteLine(message.StatusDescription);
                            break;
                        case 44:
                        case 51:
                        case 52:
                            Console.WriteLine("Message failed to deliver on a carrier level");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            // Returned when either when the posted data is faulty or if there are no recipients after the blacklist has been applied.
                            break;
                        case HttpStatusCode.Forbidden:
                            // Only returned when there is no credits on the account
                            Console.WriteLine("Insufficient credits");
                            break;
                        case HttpStatusCode.Unauthorized:
                        // Api credentials are faulty
                        default:
                            Console.WriteLine(response.ReasonPhrase);
                            break;
                    }
                }
            }
            
        }

    }
}
