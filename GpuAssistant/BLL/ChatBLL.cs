using GpuAssistant.BLL;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Text;

public static class ChatBLL
{
    private static IChatClient? chatClient;
    private static ChatOptions? options;
    private static readonly List<ChatMessage> chatMessages = new(); // a lista de mensagens possibilita a LLM lembrar do contexto

    //private static readonly string llmModel = "qwen2.5:1.5b";
    private static readonly string llmModel = "phi3:mini";
    private static readonly string ollamaServerAddress = "http://localhost:11434";
    private static readonly int maxOutputTokens = 200;

    // Regras de comportamento do assistente (os dados são anexados a cada pergunta)
    private static readonly string basePrompt =
        "Você é um assistente da infraestrutura de GPUs da equipe. " +
        "Responda SOMENTE com base nos dados da infraestrutura informados abaixo. " +
        "Nunca invente modelos, quantidades ou memória. " +
        "Se a informação não estiver nos dados, diga que não encontrou. " +
        "Se a pergunta for ambígua (por exemplo, pedir disponibilidade sem informar o modelo de GPU), peça ao usuário para especificar. " +
        "Se a pergunta não for sobre a infraestrutura de GPUs, informe educadamente que você só pode ajudar com esse assunto. " +
        "Responda em português, de forma curta e objetiva."+
        "Usuário: Quais GPUs existem?\nAssistente: Temos NVIDIA A100, NVIDIA L40S, NVIDIA RTX 4090, NVIDIA H100 e NVIDIA A40.\n" +
        "Usuário: Existem duas A100 disponíveis?\nAssistente: Sim, existem 3 A100 disponíveis.\n" +
        "Usuário: Quantas GPUs temos?\nAssistente: Temos 19 GPUs no total, sendo 13 disponíveis."+
        "Você PODE e DEVE listar as GPUs a partir dos dados quando o usuário pedir. " +
        "Perguntas curtas como 'liste elas' ou 'e a H100?' se referem ao assunto da pergunta anterior. "+
        "Usuário: Quantas GPUs temos?\nAssistente: Temos 19 GPUs no total, sendo 13 disponíveis.\n" +
        "Usuário: liste elas\nAssistente: NVIDIA A100 (3 de 4 disponíveis), NVIDIA L40S (4 de 6), NVIDIA RTX 4090 (0 de 3), NVIDIA H100 (2 de 2), NVIDIA A40 (3 de 4).";

    public static void InitializeAPI()
    {
        if (chatClient is not null)
            return;

        // timeout maior para máquinas sem GPU dedicada
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(ollamaServerAddress),
            Timeout = TimeSpan.FromMinutes(3)
        };

        chatClient = new OllamaApiClient(httpClient, llmModel);

        ConfigureAPIOptions();
    }

    private static void ConfigureAPIOptions()
    {
        options = new ChatOptions
        {
            MaxOutputTokens = maxOutputTokens,
            Temperature = 0.1f
        };
    }

    // Atualiza o system prompt com os dados atuais das GPUs consultados no sistema
    private static void RefreshSystemPrompt()
    {
        var prompt = $"{basePrompt}\n\nDados atuais da infraestrutura:\n{GpuBLL.ListGpus()}";

        if (chatMessages.Count > 0 && chatMessages[0].Role == ChatRole.System)
            chatMessages[0] = new ChatMessage(ChatRole.System, prompt);
        else
            chatMessages.Insert(0, new ChatMessage(ChatRole.System, prompt));
    }

    public static async IAsyncEnumerable<string> ReturnChatAnswer(string userInput)
    {
        if (chatClient is null)
            throw new InvalidOperationException("Chame ChatBLL.InitializeAPI() antes de usar o chat.");

        RefreshSystemPrompt();

        // se a pergunta citar alguma GPU, anexa os dados exatos dela
        var relevant = GpuBLL.FindRelevantData(userInput);
        var content = relevant is null
            ? userInput
            : $"{userInput}\n\n(Dado do sistema: {relevant})";

        chatMessages.Add(new ChatMessage(ChatRole.User, content));

        var botMessage = new StringBuilder();

        await foreach (var update in chatClient.GetStreamingResponseAsync(chatMessages, options))
        {
            botMessage.Append(update.Text);
            yield return update.Text;
        }

        // executa quando o streaming termina
        chatMessages.Add(new ChatMessage(ChatRole.Assistant, botMessage.ToString()));

        const int maxHistory = 6; // mantem as últimas 3 perguntas e respostas

        while (chatMessages.Count > maxHistory + 1)
            chatMessages.RemoveAt(1);
    }
}