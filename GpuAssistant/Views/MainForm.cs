using GpuAssistant.BLL;

namespace GpuAssistant
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            ChatBLL.InitializeAPI();

            responseRctb.AppendText("Phi3 Mini - Olá no que posso ajudar?" + Environment.NewLine);

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

            if (string.IsNullOrEmpty(userInput))
                return;

            input.Clear();
            input.Focus();
            btnSendPrompt.Enabled = false;

            AppendColored(string.Concat("Você: ", userInput, Environment.NewLine), Color.LightGreen);
            responseRctb.AppendText(Environment.NewLine);
            AppendColored("Assistente: ", Color.White);

            try
            {
                await foreach (var chunk in ChatBLL.ReturnChatAnswer(userInput))
                {
                    responseRctb.AppendText(chunk);
                }

                responseRctb.AppendText(Environment.NewLine);
                responseRctb.AppendText("----End---");
                responseRctb.AppendText(Environment.NewLine + Environment.NewLine);
                responseRctb.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao falar com o Ollama: {ex.Message}");
            }
            finally
            {
                btnSendPrompt.Enabled = true;
            }
        }

        private void AppendColored(string text, Color color)
        {
            responseRctb.SelectionStart = responseRctb.TextLength;
            responseRctb.SelectionColor = color;
            responseRctb.AppendText(text);
        }
    }
}
