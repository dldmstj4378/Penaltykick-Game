namespace Penaltykick_Game
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            lblScore = new Label();
            lblMissed = new Label();
            left = new PictureBox();
            right = new PictureBox();
            topLeft = new PictureBox();
            top = new PictureBox();
            topRight = new PictureBox();
            goalKeeper = new PictureBox();
            football = new PictureBox();
            KeeperTimer = new System.Windows.Forms.Timer(components);
            BallTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)left).BeginInit();
            ((System.ComponentModel.ISupportInitialize)right).BeginInit();
            ((System.ComponentModel.ISupportInitialize)topLeft).BeginInit();
            ((System.ComponentModel.ISupportInitialize)top).BeginInit();
            ((System.ComponentModel.ISupportInitialize)topRight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)goalKeeper).BeginInit();
            ((System.ComponentModel.ISupportInitialize)football).BeginInit();
            SuspendLayout();
            // 
            // lblScore
            // 
            lblScore.AutoSize = true;
            lblScore.BackColor = Color.Transparent;
            lblScore.Font = new Font("맑은 고딕", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblScore.ForeColor = Color.White;
            lblScore.Location = new Point(21, 26);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(127, 40);
            lblScore.TabIndex = 0;
            lblScore.Text = "Score: 0";
            // 
            // lblMissed
            // 
            lblMissed.AutoSize = true;
            lblMissed.BackColor = Color.Transparent;
            lblMissed.Font = new Font("맑은 고딕", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblMissed.ForeColor = Color.White;
            lblMissed.Location = new Point(740, 26);
            lblMissed.Name = "lblMissed";
            lblMissed.Size = new Size(148, 40);
            lblMissed.TabIndex = 1;
            lblMissed.Text = "Missed: 0";
            // 
            // left
            // 
            left.BackColor = Color.Yellow;
            left.Image = Properties.Resources.target;
            left.Location = new Point(199, 238);
            left.Name = "left";
            left.Size = new Size(40, 40);
            left.SizeMode = PictureBoxSizeMode.StretchImage;
            left.TabIndex = 2;
            left.TabStop = false;
            left.Tag = "left";
            left.Click += SetGoalTargetEvent;
            // 
            // right
            // 
            right.BackColor = Color.Yellow;
            right.Image = Properties.Resources.target;
            right.Location = new Point(674, 238);
            right.Name = "right";
            right.Size = new Size(40, 40);
            right.SizeMode = PictureBoxSizeMode.StretchImage;
            right.TabIndex = 3;
            right.TabStop = false;
            right.Tag = "right";
            right.Click += SetGoalTargetEvent;
            // 
            // topLeft
            // 
            topLeft.BackColor = Color.Yellow;
            topLeft.Image = Properties.Resources.target;
            topLeft.Location = new Point(199, 81);
            topLeft.Name = "topLeft";
            topLeft.Size = new Size(40, 40);
            topLeft.SizeMode = PictureBoxSizeMode.StretchImage;
            topLeft.TabIndex = 4;
            topLeft.TabStop = false;
            topLeft.Tag = "topLeft";
            topLeft.Click += SetGoalTargetEvent;
            // 
            // top
            // 
            top.BackColor = Color.Yellow;
            top.Image = Properties.Resources.target;
            top.Location = new Point(436, 81);
            top.Name = "top";
            top.Size = new Size(40, 40);
            top.SizeMode = PictureBoxSizeMode.StretchImage;
            top.TabIndex = 5;
            top.TabStop = false;
            top.Tag = "top";
            top.Click += SetGoalTargetEvent;
            // 
            // topRight
            // 
            topRight.BackColor = Color.Yellow;
            topRight.Image = Properties.Resources.target;
            topRight.Location = new Point(674, 81);
            topRight.Name = "topRight";
            topRight.Size = new Size(40, 40);
            topRight.SizeMode = PictureBoxSizeMode.StretchImage;
            topRight.TabIndex = 6;
            topRight.TabStop = false;
            topRight.Tag = "topRight";
            topRight.Click += SetGoalTargetEvent;
            // 
            // goalKeeper
            // 
            goalKeeper.BackColor = Color.Transparent;
            goalKeeper.Image = Properties.Resources.stand_small;
            goalKeeper.Location = new Point(418, 169);
            goalKeeper.Name = "goalKeeper";
            goalKeeper.Size = new Size(82, 126);
            goalKeeper.SizeMode = PictureBoxSizeMode.AutoSize;
            goalKeeper.TabIndex = 7;
            goalKeeper.TabStop = false;
            // 
            // football
            // 
            football.BackColor = Color.Transparent;
            football.Image = Properties.Resources.football;
            football.Location = new Point(430, 500);
            football.Name = "football";
            football.Size = new Size(50, 51);
            football.SizeMode = PictureBoxSizeMode.AutoSize;
            football.TabIndex = 8;
            football.TabStop = false;
            // 
            // KeeperTimer
            // 
            KeeperTimer.Interval = 20;
            KeeperTimer.Tick += KeeperTimerEvent;
            // 
            // BallTimer
            // 
            BallTimer.Interval = 20;
            BallTimer.Tick += BallTimerEvent;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.background;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(899, 678);
            Controls.Add(football);
            Controls.Add(goalKeeper);
            Controls.Add(topRight);
            Controls.Add(top);
            Controls.Add(topLeft);
            Controls.Add(right);
            Controls.Add(left);
            Controls.Add(lblMissed);
            Controls.Add(lblScore);
            DoubleBuffered = true;
            Name = "Form1";
            Text = "Penaltykick Game";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)left).EndInit();
            ((System.ComponentModel.ISupportInitialize)right).EndInit();
            ((System.ComponentModel.ISupportInitialize)topLeft).EndInit();
            ((System.ComponentModel.ISupportInitialize)top).EndInit();
            ((System.ComponentModel.ISupportInitialize)topRight).EndInit();
            ((System.ComponentModel.ISupportInitialize)goalKeeper).EndInit();
            ((System.ComponentModel.ISupportInitialize)football).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblScore;
        private Label lblMissed;
        private PictureBox left;
        private PictureBox right;
        private PictureBox topLeft;
        private PictureBox top;
        private PictureBox topRight;
        private PictureBox goalKeeper;
        private PictureBox football;
        private System.Windows.Forms.Timer KeeperTimer;
        private System.Windows.Forms.Timer BallTimer;
    }
}
