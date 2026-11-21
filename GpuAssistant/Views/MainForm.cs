using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Web;
using System.Windows.Forms;

namespace GpuAssistant
{
    public partial class MainForm : Form
    {
        IChatClient chatClient;
        List<ChatMessage> chatMessages = new(); // a lista de mensagens possibilita a LLM lembrar do contexto 
        private readonly ChatOptions options = new() { MaxOutputTokens = 80 };

        public MainForm()
        {
            chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "qwen2.5:1.5b");

            InitializeComponent();
            responseRctb.AppendText("Chat Qwen 2.5 - Olá no que posso ajudar?" + Environment.NewLine);

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pnlTextBox_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void btnSendPrompt_ClickAsync(object sender, EventArgs e)
        {
            var userInput = input.Text.Trim();

            responseRctb.SelectionStart = responseRctb.TextLength;
            responseRctb.SelectionColor = Color.LightGreen;
            responseRctb.AppendText( string.Concat("Voce: ", userInput, Environment.NewLine));

            input.Clear();
            input.Focus();


            if (string.IsNullOrEmpty(userInput))
                return;

            responseRctb.SelectionColor = Color.White;
            responseRctb.AppendText("Espere a resposta");
            responseRctb.AppendText(Environment.NewLine);
            responseRctb.AppendText(Environment.NewLine);

            
            chatMessages.Add(new ChatMessage(ChatRole.User, userInput));

            responseRctb.AppendText("Assistente: ");

            var botMessage = "";

            // recebe a mensagem em chuncks
            await foreach (var update in chatClient.GetStreamingResponseAsync(chatMessages, options))
            {
                responseRctb.AppendText(update.Text);

                botMessage += update.Text;

            }

            chatMessages.Add(new ChatMessage(ChatRole.Assistant, botMessage)); // salva a mensagem do bot 
            
            responseRctb.AppendText(Environment.NewLine);
            responseRctb.AppendText("----End---"+Environment.NewLine);
        }
    }
}
