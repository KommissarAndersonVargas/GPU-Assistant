namespace GpuAssistant
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            input = new TextBox();
            pnlTextBox = new Panel();
            tableLayoutPnlChat = new TableLayoutPanel();
            btnSendPrompt = new Button();
            pnlChatView = new Panel();
            responseRctb = new RichTextBox();
            pnlTextBox.SuspendLayout();
            tableLayoutPnlChat.SuspendLayout();
            pnlChatView.SuspendLayout();
            SuspendLayout();
            // 
            // input
            // 
            input.BackColor = SystemColors.ControlText;
            input.Dock = DockStyle.Fill;
            input.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            input.ForeColor = Color.Transparent;
            input.Location = new Point(3, 3);
            input.Multiline = true;
            input.Name = "input";
            input.Size = new Size(731, 62);
            input.TabIndex = 1;
            input.TextChanged += textBox1_TextChanged;
            // 
            // pnlTextBox
            // 
            pnlTextBox.BackColor = Color.Transparent;
            pnlTextBox.Controls.Add(tableLayoutPnlChat);
            pnlTextBox.Dock = DockStyle.Bottom;
            pnlTextBox.Location = new Point(0, 382);
            pnlTextBox.Name = "pnlTextBox";
            pnlTextBox.Size = new Size(800, 68);
            pnlTextBox.TabIndex = 3;
            pnlTextBox.Paint += pnlTextBox_Paint;
            // 
            // tableLayoutPnlChat
            // 
            tableLayoutPnlChat.BackColor = Color.Black;
            tableLayoutPnlChat.ColumnCount = 2;
            tableLayoutPnlChat.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 92.125F));
            tableLayoutPnlChat.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7.875F));
            tableLayoutPnlChat.Controls.Add(btnSendPrompt, 1, 0);
            tableLayoutPnlChat.Controls.Add(input, 0, 0);
            tableLayoutPnlChat.Dock = DockStyle.Fill;
            tableLayoutPnlChat.ForeColor = Color.Black;
            tableLayoutPnlChat.Location = new Point(0, 0);
            tableLayoutPnlChat.Name = "tableLayoutPnlChat";
            tableLayoutPnlChat.RowCount = 1;
            tableLayoutPnlChat.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPnlChat.Size = new Size(800, 68);
            tableLayoutPnlChat.TabIndex = 4;
            // 
            // btnSendPrompt
            // 
            btnSendPrompt.BackColor = Color.Transparent;
            btnSendPrompt.BackgroundImage = Properties.Resources.send;
            btnSendPrompt.BackgroundImageLayout = ImageLayout.Stretch;
            btnSendPrompt.Dock = DockStyle.Fill;
            btnSendPrompt.FlatStyle = FlatStyle.Flat;
            btnSendPrompt.Location = new Point(740, 3);
            btnSendPrompt.Name = "btnSendPrompt";
            btnSendPrompt.Size = new Size(57, 62);
            btnSendPrompt.TabIndex = 4;
            btnSendPrompt.UseVisualStyleBackColor = false;
            btnSendPrompt.Click += btnSendPrompt_ClickAsync;
            // 
            // pnlChatView
            // 
            pnlChatView.Controls.Add(responseRctb);
            pnlChatView.Dock = DockStyle.Fill;
            pnlChatView.Location = new Point(0, 0);
            pnlChatView.Name = "pnlChatView";
            pnlChatView.Size = new Size(800, 382);
            pnlChatView.TabIndex = 4;
            // 
            // responseRctb
            // 
            responseRctb.BackColor = SystemColors.InactiveCaptionText;
            responseRctb.Dock = DockStyle.Fill;
            responseRctb.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            responseRctb.ForeColor = SystemColors.Info;
            responseRctb.Location = new Point(0, 0);
            responseRctb.Name = "responseRctb";
            responseRctb.Size = new Size(800, 382);
            responseRctb.TabIndex = 0;
            responseRctb.Text = "";
            // 
            // MainForm
            // 
            AcceptButton = btnSendPrompt;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pnlChatView);
            Controls.Add(pnlTextBox);
            Name = "MainForm";
            ShowIcon = false;
            Text = "GPU Assitent";
            pnlTextBox.ResumeLayout(false);
            tableLayoutPnlChat.ResumeLayout(false);
            tableLayoutPnlChat.PerformLayout();
            pnlChatView.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TextBox input;
        private Panel pnlTextBox;
        private TableLayoutPanel tableLayoutPnlChat;
        private Button btnSendPrompt;
        private Panel pnlChatView;
        private RichTextBox responseRctb;
    }
}
