using Penaltykick_Game;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Penaltykick_Game
{
    public partial class StartForm : Form
    {
        private Label lblTitle;
        private TextBox txtID;
        private TextBox txtPW;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblStatus;
        private Label lblRole;
        private Label lblScore;
        private Label lblRound;
        private NetClient net = new NetClient();

        public StartForm()
        {
            InitUI();
            this.Load += StartForm_Load;

            net.OnLine += OnLine;
        }

        private void InitUI()
        {
            // 창 기본 세팅
            this.Text = "Penalty Kick Game - Start";
            this.ClientSize = new Size(860, 800);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackgroundImage = Properties.Resources.start_bg;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            //// 타이틀 라벨
            //lblTitle = new Label();
            //lblTitle.Text = "PENALTY KICK GAME";
            //lblTitle.Font = new Font("Impact", 36F, FontStyle.Bold);
            //lblTitle.ForeColor = Color.Gold;
            //lblTitle.BackColor = Color.Transparent;
            //lblTitle.AutoSize = true;
            //lblTitle.Location = new Point((this.Width - lblTitle.Width) / 2, 80);
            //lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            //this.Controls.Add(lblTitle);

            // ID 입력
            txtID = new TextBox();
            txtID.PlaceholderText = "ID";
            txtID.Font = new Font("Segoe UI", 12F);
            txtID.Location = new Point(280, 520);
            txtID.Width = 300;
            this.Controls.Add(txtID);

            // PW 입력
            txtPW = new TextBox();
            txtPW.PlaceholderText = "Password";
            txtPW.Font = new Font("Segoe UI", 12F);
            txtPW.PasswordChar = '●';
            txtPW.Location = new Point(280, 560);
            txtPW.Width = 300;
            this.Controls.Add(txtPW);

            // 로그인 버튼
            btnLogin = new Button();
            btnLogin.Text = "Login";
            btnLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnLogin.Location = new Point(280, 610);
            btnLogin.Size = new Size(140, 40);
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            // 회원가입 버튼
            btnRegister = new Button();
            btnRegister.Text = "Register";
            btnRegister.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRegister.Location = new Point(440, 610);
            btnRegister.Size = new Size(140, 40);
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);

            // 상태 라벨
            lblStatus = new Label();
            lblStatus.Text = "서버에 연결 중...";
            lblStatus.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblStatus.ForeColor = Color.White;
            lblStatus.BackColor = Color.Transparent;
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(280, 680);
            this.Controls.Add(lblStatus);

            lblRole = new Label()
            {
                Left = 20,
                Top = 80,
                Width = 200,
                Text = "역할: -",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold)
            };

            lblScore = new Label()
            {
                Left = 350,
                Top = 80,
                Width = 200,
                Text = "Score 0:0",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold)
            };

            lblRound = new Label()
            {
                Left = 600,
                Top = 80,
                Width = 250,
                Text = "Round 0",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold)
            };
        }

        private async void StartForm_Load(object sender, EventArgs e)
        {
            // 🟡 프로그램 시작 시 자동 서버 연결
            bool connected = await net.Connect("127.0.0.1", 9000);
            lblStatus.Text = connected ? "✅ 서버 연결 완료!" : "❌ 서버 연결 실패";
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string id = txtID.Text.Trim();
            string pw = txtPW.Text.Trim();
            await net.Send($"LOGIN {id} {pw}");
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            string id = txtID.Text.Trim();
            string pw = txtPW.Text.Trim();
            await net.Send($"REGISTER {id} {pw}");
        }

        // ✅ 로그인 성공 시 메인 게임으로 전환하는 메서드
        public void OpenGameForm(string username, string wins, string losses, string rank)
        {
            Form1 game = new Form1(net, username, wins, losses, rank);

            game.StartPosition = FormStartPosition.Manual;
            game.Location = this.Location; 
            game.Size = this.Size;         

            this.Hide();
            game.ShowDialog();
            this.Close();
        }

        private void OnLine(string line)
        {
            this.BeginInvoke(new Action(() => HandleServerMessage(line)));
        }

        private void HandleServerMessage(string line)
        {
            if (line.StartsWith("LOGIN_OK"))
            {
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string username = parts[1];
                string wins = parts[2];
                string losses = parts[3];
                string rank = parts[4];

                this.Invoke((MethodInvoker)(() =>
                {
                    OpenGameForm(username, wins, losses, rank);
                }));
            }

            else if (line == "LOGIN_FAIL")
            {
                lblStatus.Text = "로그인 실패. 다시 시도하세요.";
            }
            else if (line == "REGISTER_OK")
            {
                lblStatus.Text = "회원가입 성공!";
            }
            else if (line == "REGISTER_FAIL")
            {
                lblStatus.Text = "회원가입 실패. 이미 존재하는 ID일 수 있습니다.";
            }
        }

    }
}

