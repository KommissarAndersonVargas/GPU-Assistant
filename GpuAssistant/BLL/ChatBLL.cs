using Microsoft.Extensions.AI;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpuAssistant.BLL
{
    public class ChatBLL
    {
        private static IChatClient chatClient;
        private static List<ChatMessage> chatMessages = new(); // a lista de mensagens possibilita a LLM lembrar do contexto 
        private static readonly ChatOptions options = new() { MaxOutputTokens = 80 };

        private static readonly string llmModel = "qwen2.5:1.5b";
        private static readonly string ollamaServerAddres = "http://localhost:11434";

        public static void InitializeAPI()
        {
            chatClient = new OllamaApiClient(new Uri(ollamaServerAddres), llmModel);
        }


        public static async IAsyncEnumerable<string> ReturnChatAnswer(string userInput)
        {
            chatMessages.Add(new ChatMessage(ChatRole.User, userInput));

            var botMessage = new StringBuilder();

            await foreach (var update in chatClient.GetStreamingResponseAsync(chatMessages, options))
            {
                botMessage.Append(update.Text);
                yield return update.Text;
            }

            // executa quando o streaming termina
            chatMessages.Add(new ChatMessage(ChatRole.Assistant, botMessage.ToString()));
        }
    }
}
