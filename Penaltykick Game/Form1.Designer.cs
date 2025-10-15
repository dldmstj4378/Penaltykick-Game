namespace Penaltykick_Game
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtHost;
        private TextBox txtPort;
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnConnect;
        private Button btnRegister;
        private Button btnLogin;
        private Button btnReady;

        private Label lblStatus;
        private Label lblRole;
        private Label lblScore;
        private Label lblRound;

        private PictureBox goalBackground;
        private PictureBox football;
        private PictureBox goalKeeper;
        private PictureBox topLeft;
        private PictureBox top;
        private PictureBox topRight;
        private PictureBox left;
        private PictureBox right;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // 🟡 상단 UI
            txtHost = new TextBox() { Left = 20, Top = 15, Width = 120, Text = "127.0.0.1" };
            txtPort = new TextBox() { Left = 145, Top = 15, Width = 60, Text = "9000" };
            txtUser = new TextBox() { Left = 210, Top = 15, Width = 120, PlaceholderText = "ID" };
            txtPass = new TextBox() { Left = 335, Top = 15, Width = 120, PlaceholderText = "PW", UseSystemPasswordChar = true };
            btnConnect = new Button() { Left = 460, Top = 12, Width = 80, Text = "Connect" };
            btnRegister = new Button() { Left = 545, Top = 12, Width = 80, Text = "Register" };
            btnLogin = new Button() { Left = 630, Top = 12, Width = 80, Text = "Login" };
            btnReady = new Button() { Left = 715, Top = 12, Width = 80, Text = "Ready" };

            lblStatus = new Label() { Left = 20, Top = 50, Width = 780, Text = "상태: 서버에 연결하세요." };
            lblRole = new Label() { Left = 20, Top = 80, Width = 200, Text = "역할: -" };
            lblScore = new Label() { Left = 350, Top = 80, Width = 200, Text = "Score 0:0" };
            lblRound = new Label() { Left = 600, Top = 80, Width = 250, Text = "Round 0" };

            // 🥅 골대 배경
            goalBackground = new PictureBox()
            {
                Left = 20,
                Top = 110,
                Width = 800,
                Height = 450,
                Image = Properties.Resources.background,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // ⚽ 공
            football = new PictureBox()
            {
                Width = 50,
                Height = 50,
                BackColor = Color.Transparent,
                Image = Properties.Resources.football,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new System.Drawing.Point(400, 520)
            };

            // 🧤 골키퍼
            goalKeeper = new PictureBox()
            {
                Width = 90,
                Height = 130,
                BackColor = Color.Transparent,
                Image = Properties.Resources.stand_small,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new System.Drawing.Point(380, 210)
            };

            // 🎯 타겟 (좌표 및 크기 고정)
            topLeft = CreateTarget(190, 140, "topLeft");
            top = CreateTarget(375, 140, "top");
            topRight = CreateTarget(560, 140, "topRight");
            left = CreateTarget(200, 300, "left");
            right = CreateTarget(540, 300, "right");

            // 🔹 투명 배경 설정
            goalKeeper.BackColor = Color.Transparent;
            football.BackColor = Color.Transparent;
            topLeft.BackColor = Color.Transparent;
            top.BackColor = Color.Transparent;
            topRight.BackColor = Color.Transparent;
            left.BackColor = Color.Transparent;
            right.BackColor = Color.Transparent;

            // 🧭 폼에 추가
            Controls.AddRange(new Control[]
            {
                txtHost, txtPort, txtUser, txtPass,
                btnConnect, btnRegister, btnLogin, btnReady,
                lblStatus, lblRole, lblScore, lblRound,
                goalBackground,
                topLeft, top, topRight, left, right,
                goalKeeper, football
            });

            // 🪄 폼 설정
            Text = "Penaltykick Game";
            ClientSize = new System.Drawing.Size(860, 800);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            BackgroundImageLayout = ImageLayout.Stretch;

            // 이벤트
            btnConnect.Click += btnConnect_Click;
            btnRegister.Click += btnRegister_Click;
            btnLogin.Click += btnLogin_Click;
            btnReady.Click += btnReady_Click;

            goalKeeper.BringToFront();
            football.BringToFront();
            topLeft.BringToFront();
            top.BringToFront();
            topRight.BringToFront();
            left.BringToFront();
            right.BringToFront();
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
                BackColor = Color.Transparent,
                Image = Properties.Resources.target,
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            pb.Click += Target_Click;
            return pb;
        }



    }
}
