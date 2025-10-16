using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Penaltykick_Game
{
    public partial class Form1 : Form
    {
        private NetClient net;
        private List<PictureBox> goalTarget;
        private string myRole = "-";
        private string kickerName = "-";
        private int p1 = 0, p2 = 0;
        private System.Windows.Forms.Timer? shootTimer;
        private System.Windows.Forms.Timer? keeperTimer;

        // 🟡 초기 위치 저장용
        private Point initialBallPosition;
        private Point initialKeeperPosition;
        private Size initialKeeperSize;

        // 🛑 애니메이션 중복 방지용
        private bool isAnimating = false;
        private System.Windows.Forms.Label lblUserInfo;
        private string currentRank = "";
        private string _username;
        private string _wins;
        private string _losses;
        private string _rank;

        public Form1(NetClient netClient, string username, string wins, string losses, string rank)
        {
            InitializeComponent();
            net = netClient;
            net.OnLine += OnLine;

            _username = username;
            _wins = wins;
            _losses = losses;
            _rank = rank;

            lblStatus.Text = "로그인 완료";
            lblStatus.Font = new Font("맑은 고딕", 14F, FontStyle.Bold);  // 글씨 키우기 & Bold
            lblStatus.ForeColor = Color.Black;                            // 색상 변경 (원하는 색으로)

            lblRole.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblScore.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRound.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRole.ForeColor = Color.Black;
            lblScore.ForeColor = Color.Black;
            lblRound.ForeColor = Color.Black;


            goalTarget = new List<PictureBox> { topLeft, top, topRight, left, right };

            goalKeeper.Parent = goalBackground;
            football.Parent = goalBackground;
            foreach (var t in goalTarget) t.Parent = goalBackground;

            this.Load += Form1_Load;
            this.Resize += (s, e) => PositionElements();
            this.FormClosed += (s, e) => Application.Exit();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.lblUserInfo = new Label();
            this.lblUserInfo.AutoSize = true;
            this.lblUserInfo.Font = new Font("맑은 고딕", 14F, FontStyle.Bold);
            this.lblUserInfo.Location = new Point(200, 15);
            this.lblUserInfo.Text = "";
            this.Controls.Add(this.lblUserInfo);

            UpdateUserInfoLabel();
            this.lblUserInfo.BringToFront();
            this.FormClosed += (s, e) => Application.Exit();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            PositionElements();

            // 📌 초기 위치 저장
            initialBallPosition = football.Location;
            initialKeeperPosition = goalKeeper.Location;
            initialKeeperSize = goalKeeper.Size;
        }

        private void PositionElements()
        {
            // 🟩 초록색 배경
            goalBackground.Width = 915;
            goalBackground.Height = 717;
            goalBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            goalBackground.Left = (this.ClientSize.Width - goalBackground.Width) / 2;
            goalBackground.Top = 110;
            

            // 🧤 골키퍼
            goalKeeper.Width = 80;
            goalKeeper.Height = 130;
            goalKeeper.Location = new Point(
                (goalBackground.Width - goalKeeper.Width) / 2,
                190
            );

            // ⚽ 축구공
            football.Width = 50;
            football.Height = 50;
            football.Location = new Point(
                (goalBackground.Width - football.Width) / 2,
                530
            );

            // 🟡 타겟
            topLeft.Location = new Point(200, 80);
            top.Location = new Point(437, 80);
            topRight.Location = new Point(680, 80);
            left.Location = new Point(200, 245);
            right.Location = new Point(680, 245);

            foreach (var t in goalTarget)
            {
                t.Width = 50;
                t.Height = 50;
                t.BackColor = Color.Yellow;
                t.SizeMode = PictureBoxSizeMode.StretchImage;
                t.BringToFront();
            }

            goalKeeper.BackColor = Color.Transparent;
            football.BackColor = Color.Transparent;

            goalKeeper.BringToFront();
            football.BringToFront();
        }

        private void InitializePositions()
        {
            PositionElements();
            goalKeeper.Image = Properties.Resources.stand_small;
        }

        // 🧭 위치 리셋 함수
        private void ResetPositions()
        {
            goalKeeper.Location = initialKeeperPosition;
            goalKeeper.Size = initialKeeperSize;
            goalKeeper.Image = Properties.Resources.stand_small;

            football.Location = initialBallPosition;

            shootTimer?.Stop();
            keeperTimer?.Stop();
            shootTimer = null;
            keeperTimer = null;

            isAnimating = false;
        }

        private async void btnReady_Click(object sender, EventArgs e) =>
            await net.Send("READY");

        private async void Target_Click(object sender, EventArgs e)
        {
            // 클릭된 타겟 방향 추출
            string target = ((PictureBox)sender).Tag.ToString()!;
            if (myRole == "KICKER")
                await net.Send($"SHOOT:{target}");
            else if (myRole == "GOALKEEPER")
                await net.Send($"SAVE:{target}");

            // 클릭 후 타겟 버튼 비활성화
            SetTargetsEnabled(false);
        }

        private void OnLine(string line) => BeginInvoke(new Action(() => HandleServerMessage(line)));

        private void HandleServerMessage(string line)
        {
            // 디버그: 어떤 문자열을 받는지 항상 확인
            //Console.WriteLine($"[CLIENT RECV] {line}");

            if (line.StartsWith("LOGIN_OK"))
            {
                // 👇 여기는 그대로 둬도 됨. 단, ID/PW 입력 필드는 삭제되었으니 UI만 갱신.
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 5)
                {
                    string username = parts[1];
                    string wins = parts[2];
                    string losses = parts[3];
                    string rank = parts[4];

                    // 색상 + 텍스트 UI 업데이트만 남김
                    switch (rank)
                    {
                        case "Bronze": lblUserInfo.ForeColor = Color.SaddleBrown; break;
                        case "Silver": lblUserInfo.ForeColor = Color.DimGray; break;
                        case "Gold": lblUserInfo.ForeColor = Color.Gold; break;
                        case "Platinum": lblUserInfo.ForeColor = Color.MediumTurquoise; break;
                        case "Diamond": lblUserInfo.ForeColor = Color.DeepSkyBlue; break;
                        default: lblUserInfo.ForeColor = Color.White; break;
                    }

                    lblStatus.Text = "로그인 성공";
                    lblUserInfo.Text = $"닉네임 : {username}   승 : {wins}   패 : {losses}   랭크 : {rank}";
                }
                return;
            }


            if (line == "REGISTER_OK") lblStatus.Text = "회원가입 성공";
            else if (line == "REGISTER_FAIL") lblStatus.Text = "회원가입 실패";
            else if (line == "LOGIN_FAIL") lblStatus.Text = "로그인 실패";
            else if (line == "QUEUED") lblStatus.Text = "매칭 대기중...";
            else if (line.StartsWith("MATCH_START"))
            {
                lblStatus.Text = line;
                InitializePositions();
            }
            else if (line.StartsWith("ROLE:"))
            {
                myRole = line.Substring(5);
                lblRole.Text = "역할: " + (myRole == "KICKER" ? "키커" : "골키퍼");
                InitializePositions();
            }
            else if (line.StartsWith("TURN:"))
            {
                kickerName = line.Split('=')[1];
                lblRound.Text = "Kicker: " + kickerName;
                InitializePositions();
            }
            else if (line.StartsWith("ROUND_START"))
            {
                var parts = line.Split('|');
                int roundNum = int.Parse(parts[1].Split('=')[1]);
                lblStatus.Text = $"{roundNum} 라운드 시작! (10초 안에 선택)";

                // 🧭 1) 위치 리셋
                ResetPositions();

                // 🧭 2) UI 초기화 후
                InitializePositions();

                // 🧭 3) 타겟 활성화 (즉시)
                SetTargetsEnabledSafe(true);

                // 🧭 4) 안전장치 - 혹시 초기화 타이밍이 밀리는 경우 대비
                Task.Delay(1000).ContinueWith(_ =>
                {
                    SetTargetsEnabledSafe(true);
                });
            }
            else if (line.StartsWith("RESULT:"))
            {
                var parts = line.Split('|');
                string result = parts[0].Split(':')[1];
                string[] sc = parts[1].Split('=')[1].Split(':');
                string shooterDir = parts[2].Split('=')[1];
                string keeperDir = parts[3].Split('=')[1];

                p1 = int.Parse(sc[0]);
                p2 = int.Parse(sc[1]);
                lblScore.Text = $"Score {p1}:{p2}";

                AnimateShoot(shooterDir);
                AnimateGoalkeeper(keeperDir);
                ChangeGoalKeeperImage(keeperDir);

                lblStatus.Text = result == "goal" ? "⚽ 골!" : "🧤 세이브!";

                // 클릭 방지
                SetTargetsEnabledSafe(false);

                var delayTimer = new System.Windows.Forms.Timer { Interval = 2000 };
                delayTimer.Tick += (s, e) =>
                {
                    delayTimer.Stop();
                    ResetPositions();
                    SetTargetsEnabled(false);
                };
                delayTimer.Start();
            }
            else if (line.StartsWith("GAME_OVER"))
            {
                shootTimer?.Stop();
                keeperTimer?.Stop();
                InitializePositions();
                SetTargetsEnabled(false);

                var winner = line.Split('|').First(x => x.StartsWith("winner=")).Split('=')[1];
                var final = line.Split('|').First(x => x.StartsWith("final=")).Split('=')[1];
                MessageBox.Show($"Winner: {winner}\nFinal: {final}", "Game Over");
                lblStatus.Text = "게임 종료";

                // ✅ 게임이 끝나면 서버에 최신 전적 요청
                _ = net.Send("REQ_RECORD");
            }
            else if (line.StartsWith("RECORD "))
            {
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 5)
                {
                    string username = parts[1];
                    string wins = parts[2];
                    string losses = parts[3];
                    string rank = parts[4];

                    // 🔸 이전 랭크 저장 후 변경 확인
                    string oldRank = currentRank;
                    currentRank = rank;

                    switch (rank)
                    {
                        case "Bronze": lblUserInfo.ForeColor = Color.SaddleBrown; break;
                        case "Silver": lblUserInfo.ForeColor = Color.DimGray; break;
                        case "Gold": lblUserInfo.ForeColor = Color.Gold; break;
                        case "Platinum": lblUserInfo.ForeColor = Color.MediumTurquoise; break;
                        case "Diamond": lblUserInfo.ForeColor = Color.DeepSkyBlue; break;
                        default: lblUserInfo.ForeColor = Color.White; break;
                    }

                    lblUserInfo.Text = $"닉네임 : {username}   승 : {wins}   패 : {losses}   랭크 : {rank}";

                    // 🟡 랭크가 바뀌었으면 메시지 띄우기
                    if (!string.IsNullOrEmpty(oldRank) && oldRank != currentRank)
                    {
                        string msg = $"랭크가 {oldRank} ➝ {currentRank}(으)로 변경되었습니다!";
                        MessageBox.Show(msg, "랭크 변동", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }


        private void UpdateUserInfoLabel()
        {
            if (lblUserInfo == null) return;

            lblUserInfo.ForeColor = _rank switch
            {
                "Bronze" => Color.SaddleBrown,
                "Silver" => Color.DimGray,
                "Gold" => Color.Gold,
                "Platinum" => Color.MediumTurquoise,
                "Diamond" => Color.DeepSkyBlue,
                _ => Color.White
            };

            lblUserInfo.Text = $"닉네임 : {_username}   승 : {_wins}   패 : {_losses}   랭크 : {_rank}";
        }

        private void SetTargetsEnabled(bool enabled)
        {
            foreach (var pb in goalTarget)
                pb.Enabled = enabled;
        }

        // ========================
        // ⏳ 타겟을 일정 시간 후 다시 활성화
        // ========================
        private async Task EnableTargetsAfterDelay(int ms = 1000)
        {
            await Task.Delay(ms);
            SetTargetsEnabled(true);
            foreach (var pb in goalTarget)
                pb.BringToFront();    // ⚠️ 골키퍼나 공이 덮지 않도록 맨 앞으로
        }

        // ⚽ 공 애니메이션
        private void AnimateShoot(string direction)
        {
            if (isAnimating) return;
            isAnimating = true;

            football.Location = initialBallPosition;

            int targetX = football.Left;
            int targetY = goalBackground.Top + 180;

            switch (direction)
            {
                case "left": targetX = left.Left; targetY = left.Top; break;
                case "right": targetX = right.Left; targetY = right.Top; break;
                case "topLeft": targetX = topLeft.Left; targetY = topLeft.Top; break;
                case "top": targetX = top.Left; targetY = top.Top; break;
                case "topRight": targetX = topRight.Left; targetY = topRight.Top; break;
            }

            shootTimer?.Stop();
            shootTimer = new System.Windows.Forms.Timer();
            shootTimer.Interval = 15;
            shootTimer.Tick += async (s, e) =>
            {
                football.Left += (targetX - football.Left) / 8;
                football.Top += (targetY - football.Top) / 8;

                if (Math.Abs(football.Left - targetX) < 4 && Math.Abs(football.Top - targetY) < 4)
                {
                    shootTimer.Stop();
                    shootTimer = null;
                    isAnimating = false;       // ✅ 클릭 차단 해제
                    await EnableTargetsAfterDelay(); // ✅ 클릭 복구
                }
            };
            shootTimer.Start();
        }

        // 🧤 골키퍼 애니메이션
        private void AnimateGoalkeeper(string direction)
        {
            // 기본 위치 및 크기 초기화
            goalKeeper.Left = initialKeeperPosition.X;
            goalKeeper.Top = initialKeeperPosition.Y;

            int targetX = goalKeeper.Left;
            int targetY = goalKeeper.Top;

            switch (direction)
            {
                case "left":
                    targetX = left.Left + 10;
                    targetY = left.Top - 20;
                    break;

                case "right":
                    targetX = right.Left - 140;
                    targetY = right.Top - 25;
                    break;

                case "topLeft":
                    targetX = topLeft.Left + 10;
                    targetY = topLeft.Top + 20;
                    break;

                case "topRight":
                    targetX = topRight.Left - 110;
                    targetY = topRight.Top + 20;
                    break;

                case "top":
                    targetX = top.Left - 60;
                    targetY = top.Top + 20;
                    break;
            }

            keeperTimer?.Stop();
            keeperTimer = new System.Windows.Forms.Timer();
            keeperTimer.Interval = 15;
            keeperTimer.Tick += (s, e) =>
            {
                goalKeeper.Left += (targetX - goalKeeper.Left) / 8;
                goalKeeper.Top += (targetY - goalKeeper.Top) / 8;

                if (Math.Abs(goalKeeper.Left - targetX) < 4 && Math.Abs(goalKeeper.Top - targetY) < 4)
                {
                    keeperTimer.Stop();
                    keeperTimer = null;
                    isAnimating = false; // ✅ 골키퍼 애니메이션 끝나면 클릭 차단 해제
                }
            };
            keeperTimer.Start();
        }


        private void ChangeGoalKeeperImage(string direction)
        {
            int diveWidth = 160;
            int diveHeight = 100;

            switch (direction)
            {
                case "left":
                case "right":
                    goalKeeper.Size = new Size(180, 110);
                    break;

                case "topLeft":
                case "topRight":
                    goalKeeper.Size = new Size(160, 120);
                    break;

                case "top":
                    goalKeeper.Size = new Size(130, 180);
                    break;

                default:
                    goalKeeper.Size = initialKeeperSize;
                    break;
            }

            

            switch (direction)
            {
                case "left": goalKeeper.Image = Properties.Resources.left_save_small; break;
                case "right": goalKeeper.Image = Properties.Resources.right_save_small; break;
                case "top": goalKeeper.Image = Properties.Resources.top_save_small; break;
                case "topLeft": goalKeeper.Image = Properties.Resources.top_left_save_small; break;
                case "topRight": goalKeeper.Image = Properties.Resources.top_right_save_small; break;
                default: goalKeeper.Image = Properties.Resources.stand_small; break;
            }

            goalKeeper.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void SetTargetsEnabledSafe(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetTargetsEnabledSafe(enabled)));
                return;
            }

            foreach (var t in goalTarget)
            {
                t.Enabled = enabled;
            }
        }
        
    }
}
