using GpuAssistant.BLL;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Text;

public static class ChatBLL
{
    private static IChatClient? _chatClient;
    private static ChatOptions? _options;
    private static readonly List<ChatMessage> _chatMessages = new(); // a lista de mensagens possibilita a LLM lembrar do contexto
    private static readonly string _llmModel = "qwen2.5:1.5b";
    private static readonly string _ollamaServerAddress = "http://localhost:11434";
    private static readonly int _maxOutputTokens = 200;
    private static readonly float _defaultTemperature = 0.1f;

    // Regras de comportamento do assistente
    private static readonly string _basePrompt =
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
        if (_chatClient is not null)
            return;

        // timeout maior para máquinas sem GPU dedicada
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_ollamaServerAddress),
            Timeout = TimeSpan.FromMinutes(3)
        };

        _chatClient = new OllamaApiClient(httpClient, _llmModel);

        ConfigureAPIOptions();
    }

    private static void ConfigureAPIOptions()
    {
        _options = new ChatOptions
        {
            MaxOutputTokens = _maxOutputTokens,
            Temperature = _defaultTemperature // diminui a predição do modelo, a fim de deixa-lo restrito aos dados
        };
    }

    // Atualiza o system prompt com os dados atuais das GPUs consultados no sistema
    private static void RefreshSystemPrompt()
    {
        var example = $"Usuário: Quantas GPUs temos?\nAssistente: Temos {GpuBLL.TotalGpus()} GPUs no total, sendo {GpuBLL.TotalAvailable()} disponíveis.\n";

        var prompt = $"{_basePrompt}\n{example}\nDados atuais da infraestrutura:\n{GpuBLL.ListGpus()}";

        if (_chatMessages.Count > 0 && _chatMessages[0].Role == ChatRole.System)
            _chatMessages[0] = new ChatMessage(ChatRole.System, prompt);
        else
            _chatMessages.Insert(0, new ChatMessage(ChatRole.System, prompt));
    }

    public static async IAsyncEnumerable<string> ReturnChatAnswer(string userInput)
    {
        if (_chatClient is null)
            throw new InvalidOperationException("Chame ChatBLL.InitializeAPI() antes de usar o chat.");

        RefreshSystemPrompt();

        // se a pergunta citar alguma GPU, anexa os dados exatos dela
        var relevant = GpuBLL.FindRelevantData(userInput);
        var content = relevant is null
            ? userInput
            : $"{userInput}\n\n(Dado do sistema: {relevant})";

        _chatMessages.Add(new ChatMessage(ChatRole.User, content));

        var botMessage = new StringBuilder();

        await foreach (var update in _chatClient.GetStreamingResponseAsync(_chatMessages, _options))
        {
            botMessage.Append(update.Text);
            yield return update.Text;
        }

        // Salva a resposta do assistente no histórico, para o modelo lembrar o que já respondeu evitando assim
        // responder as perguntas anteriores, dá memória de conversa ao modelo de IA

        _chatMessages.Add(new ChatMessage(ChatRole.Assistant, botMessage.ToString()));

        const int maxHistory = 6; // mantem as últimas 3 perguntas e respostas para otimizar o código

        while (_chatMessages.Count > maxHistory + 1)
            _chatMessages.RemoveAt(1);
    }
}