
namespace MSC
{
    partial class Setup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setup));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.stageTitle = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.downloadServerDataStage = new System.Windows.Forms.CheckBox();
            this.basicConfigStage = new System.Windows.Forms.CheckBox();
            this.goAStageBack = new System.Windows.Forms.Button();
            this.nextStage = new System.Windows.Forms.Button();
            this.quitSetup = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel1.Controls.Add(this.stageTitle);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(548, 346);
            this.splitContainer1.SplitterDistance = 54;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.Image = global::MSC.Properties.Resources.icon;
            this.pictureBox1.Location = new System.Drawing.Point(9, 6);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(43, 45);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // stageTitle
            // 
            this.stageTitle.AutoSize = true;
            this.stageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.stageTitle.Location = new System.Drawing.Point(56, 20);
            this.stageTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.stageTitle.Name = "stageTitle";
            this.stageTitle.Size = new System.Drawing.Size(116, 18);
            this.stageTitle.TabIndex = 0;
            this.stageTitle.Text = "First Time Setup";
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label3.Location = new System.Drawing.Point(0, 40);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(548, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "_________________________________________________________________________________" +
    "_______________";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(548, 243);
            this.panel1.TabIndex = 3;
            this.panel1.Visible = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Panel2.Controls.Add(this.goAStageBack);
            this.splitContainer2.Panel2.Controls.Add(this.nextStage);
            this.splitContainer2.Panel2.Controls.Add(this.quitSetup);
            this.splitContainer2.Size = new System.Drawing.Size(548, 289);
            this.splitContainer2.SplitterDistance = 239;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Right;
            this.label2.Location = new System.Drawing.Point(235, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(313, 239);
            this.label2.TabIndex = 1;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.downloadServerDataStage);
            this.groupBox1.Controls.Add(this.basicConfigStage);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(230, 239);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stages";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoCheck = false;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(4, 59);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(53, 17);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "Finish";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // downloadServerDataStage
            // 
            this.downloadServerDataStage.AutoCheck = false;
            this.downloadServerDataStage.AutoSize = true;
            this.downloadServerDataStage.Location = new System.Drawing.Point(4, 38);
            this.downloadServerDataStage.Margin = new System.Windows.Forms.Padding(2);
            this.downloadServerDataStage.Name = "downloadServerDataStage";
            this.downloadServerDataStage.Size = new System.Drawing.Size(134, 17);
            this.downloadServerDataStage.TabIndex = 2;
            this.downloadServerDataStage.Text = "Download Server Data";
            this.downloadServerDataStage.UseVisualStyleBackColor = true;
            // 
            // basicConfigStage
            // 
            this.basicConfigStage.AutoCheck = false;
            this.basicConfigStage.AutoSize = true;
            this.basicConfigStage.Location = new System.Drawing.Point(4, 17);
            this.basicConfigStage.Margin = new System.Windows.Forms.Padding(2);
            this.basicConfigStage.Name = "basicConfigStage";
            this.basicConfigStage.Size = new System.Drawing.Size(122, 17);
            this.basicConfigStage.TabIndex = 0;
            this.basicConfigStage.Text = "Server Configuration";
            this.basicConfigStage.UseVisualStyleBackColor = true;
            // 
            // goAStageBack
            // 
            this.goAStageBack.AutoSize = true;
            this.goAStageBack.Dock = System.Windows.Forms.DockStyle.Right;
            this.goAStageBack.Enabled = false;
            this.goAStageBack.Location = new System.Drawing.Point(344, 0);
            this.goAStageBack.Margin = new System.Windows.Forms.Padding(2);
            this.goAStageBack.MaximumSize = new System.Drawing.Size(68, 22);
            this.goAStageBack.MinimumSize = new System.Drawing.Size(68, 22);
            this.goAStageBack.Name = "goAStageBack";
            this.goAStageBack.Size = new System.Drawing.Size(68, 22);
            this.goAStageBack.TabIndex = 2;
            this.goAStageBack.Text = "◀️ Back";
            this.goAStageBack.UseVisualStyleBackColor = true;
            this.goAStageBack.Click += new System.EventHandler(this.goAStageBack_Click);
            // 
            // nextStage
            // 
            this.nextStage.AutoSize = true;
            this.nextStage.Dock = System.Windows.Forms.DockStyle.Right;
            this.nextStage.Location = new System.Drawing.Point(412, 0);
            this.nextStage.Margin = new System.Windows.Forms.Padding(2);
            this.nextStage.MaximumSize = new System.Drawing.Size(68, 22);
            this.nextStage.MinimumSize = new System.Drawing.Size(68, 22);
            this.nextStage.Name = "nextStage";
            this.nextStage.Size = new System.Drawing.Size(68, 22);
            this.nextStage.TabIndex = 1;
            this.nextStage.Text = "Next ▶️";
            this.nextStage.UseVisualStyleBackColor = true;
            this.nextStage.Click += new System.EventHandler(this.nextStage_Click);
            // 
            // quitSetup
            // 
            this.quitSetup.AutoSize = true;
            this.quitSetup.Dock = System.Windows.Forms.DockStyle.Right;
            this.quitSetup.Location = new System.Drawing.Point(480, 0);
            this.quitSetup.Margin = new System.Windows.Forms.Padding(2);
            this.quitSetup.MaximumSize = new System.Drawing.Size(68, 22);
            this.quitSetup.MinimumSize = new System.Drawing.Size(68, 22);
            this.quitSetup.Name = "quitSetup";
            this.quitSetup.Size = new System.Drawing.Size(68, 22);
            this.quitSetup.TabIndex = 0;
            this.quitSetup.Text = "Cancel";
            this.quitSetup.UseVisualStyleBackColor = true;
            this.quitSetup.Click += new System.EventHandler(this.quitSetup_Click);
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 346);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(564, 385);
            this.MinimumSize = new System.Drawing.Size(564, 385);
            this.Name = "Setup";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setup - Minecraft Server Client";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label stageTitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox downloadServerDataStage;
        private System.Windows.Forms.CheckBox basicConfigStage;
        private System.Windows.Forms.Button goAStageBack;
        private System.Windows.Forms.Button nextStage;
        private System.Windows.Forms.Button quitSetup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
    }
}