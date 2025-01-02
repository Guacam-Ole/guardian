using Newtonsoft.Json;

using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limerick
{
    public class OpenAIGen
    {
        public async Task<string> GetLimerickFromTitle(string title)
        {
            var service = Login();

            var request = new ChatCompletionCreateRequest
            {
                Model = Models.Gpt_3_5_Turbo,
                Temperature = 0.9f,
                MaxTokens = 250,
                Messages=[]
            };
            request.Messages.Add(ChatMessage.FromSystem("You always create a Limerick from the topic entered"));
            request.Messages.Add(ChatMessage.FromSystem("Make sure to enter a newline after each part of the limerick that should not contain more than 350 characters"));
            request.Messages.Add(ChatMessage.FromSystem("If the topic is about a hate crime, murder, racism, sexism, antisemitism or death return 'Sorry, Dave, I cannot do that'"));
            request.Messages.Add(ChatMessage.FromUser(title));


            var result = await service.ChatCompletion.CreateCompletion(request);

            if (result.Successful)
            {
                var content=result.Choices.First().Message.Content;
                if (content.Contains("Sorry, Dave"))
                {
                    Console.WriteLine($"Did not create a Limerick for '{title}' because it seems to be inappropriate");
                    return null;

                }
                return content;
            }
            Console.WriteLine($"OpenAI failed");
            return null;
        }


        private static OpenAIService Login()
        {
            var secrets = JsonConvert.DeserializeObject<Secrets>(File.ReadAllText("secrets.json")) ?? throw new Exception("cannot read secrets");
            var openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = secrets.OpenAiKey
            });
            return openAiService;
        }
    }
}
