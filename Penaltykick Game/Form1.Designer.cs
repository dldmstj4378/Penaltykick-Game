namespace Penaltykick_Game
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblRound;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnReady;

        private System.Windows.Forms.PictureBox goalBackground;
        private System.Windows.Forms.PictureBox football;
        private System.Windows.Forms.PictureBox left;
        private System.Windows.Forms.PictureBox right;
        private System.Windows.Forms.PictureBox top;
        private System.Windows.Forms.PictureBox topLeft;
        private System.Windows.Forms.PictureBox topRight;
        private System.Windows.Forms.PictureBox goalkeeper;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            txtHost = new TextBox() { Left = 20, Top = 15, Width = 120, Text = "127.0.0.1" };
            txtPort = new TextBox() { Left = 145, Top = 15, Width = 60, Text = "9000" };
            txtUser = new TextBox() { Left = 210, Top = 15, Width = 120, PlaceholderText = "ID" };
            txtPass = new TextBox() { Left = 335, Top = 15, Width = 120, PlaceholderText = "PW", UseSystemPasswordChar = true };
            btnConnect = new Button() { Left = 460, Top = 12, Width = 80, Text = "Connect" };
            btnRegister = new Button() { Left = 545, Top = 12, Width = 80, Text = "Register" };
            btnLogin = new Button() { Left = 630, Top = 12, Width = 80, Text = "Login" };
            btnReady = new Button() { Left = 715, Top = 12, Width = 80, Text = "Ready" };

            lblStatus = new Label() { Left = 20, Top = 50, Width = 780, Text = "상태: 서버에 연결하세요." };
            lblRole = new Label() { Left = 20, Top = 80, Width = 300, Text = "역할: -" };
            lblScore = new Label() { Left = 340, Top = 80, Width = 280, Text = "Score 0:0" };
            lblRound = new Label() { Left = 640, Top = 80, Width = 210, Text = "Round 0" };

            goalBackground = new PictureBox()
            {
                Left = 80,
                Top = 120,
                Width = 700,
                Height = 450,
                Image = Properties.Resources.background, // ⚽ 골대 이미지
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            football = new PictureBox()
            {
                Width = 50,
                Height = 50,
                Left = 400,
                Top = 500,
                BackColor = Color.Transparent,
                Image = Properties.Resources.football,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            goalkeeper = new PictureBox()
            {
                Width = 80,
                Height = 130,
                Left = 400,
                Top = 300,
                BackColor = Color.Transparent,
                Image = Properties.Resources.stand_small,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            left = CreateTarget(200, 370, "left");
            right = CreateTarget(600, 370, "right");
            top = CreateTarget(400, 150, "top");
            topLeft = CreateTarget(200, 150, "topLeft");
            topRight = CreateTarget(600, 150, "topRight");

            // Control 추가
            Controls.AddRange(new Control[]
            {
                txtHost, txtPort, txtUser, txtPass,
                btnConnect, btnRegister, btnLogin, btnReady,
                lblStatus, lblRole, lblScore, lblRound,
                goalBackground, football, goalkeeper,
                left, right, top, topLeft, topRight
            });

            Text = "Penaltykick Game";
            ClientSize = new Size(880, 640);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Load += Form1_Load;

            btnConnect.Click += btnConnect_Click;
            btnRegister.Click += btnRegister_Click;
            btnLogin.Click += btnLogin_Click;
            btnReady.Click += btnReady_Click;
        }

        private PictureBox CreateTarget(int x, int y, string tag)
        {
            var pb = new PictureBox()
            {
                Left = x,
                Top = y,
                Width = 50,
                Height = 50,
                Tag = tag,
                BackColor = Color.Yellow,
                Image = Properties.Resources.target,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Cursor = Cursors.Hand
            };
            pb.Click += Target_Click;
            return pb;
        }
    }
}
