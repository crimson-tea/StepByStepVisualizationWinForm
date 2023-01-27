namespace AnimationWinForm
{
    partial class UserControl3
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.NextButton = new System.Windows.Forms.Button();
            this.PrevButton = new System.Windows.Forms.Button();
            this.AutoButton = new System.Windows.Forms.Button();
            this.SwitchSieveButton = new System.Windows.Forms.Button();
            this.StepLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SieveTypeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(84, 212);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 23);
            this.NextButton.TabIndex = 0;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // PrevButton
            // 
            this.PrevButton.Location = new System.Drawing.Point(3, 212);
            this.PrevButton.Name = "PrevButton";
            this.PrevButton.Size = new System.Drawing.Size(75, 23);
            this.PrevButton.TabIndex = 1;
            this.PrevButton.Text = "Prev";
            this.PrevButton.UseVisualStyleBackColor = true;
            this.PrevButton.Click += new System.EventHandler(this.PrevButton_Click);
            // 
            // AutoButton
            // 
            this.AutoButton.Location = new System.Drawing.Point(165, 212);
            this.AutoButton.Name = "AutoButton";
            this.AutoButton.Size = new System.Drawing.Size(75, 23);
            this.AutoButton.TabIndex = 2;
            this.AutoButton.Text = "Auto";
            this.AutoButton.UseVisualStyleBackColor = true;
            this.AutoButton.Click += new System.EventHandler(this.AutoButton_Click);
            // 
            // SwitchSieveButton
            // 
            this.SwitchSieveButton.Location = new System.Drawing.Point(276, 41);
            this.SwitchSieveButton.Name = "SwitchSieveButton";
            this.SwitchSieveButton.Size = new System.Drawing.Size(75, 75);
            this.SwitchSieveButton.TabIndex = 3;
            this.SwitchSieveButton.Text = "Switch Sieve";
            this.SwitchSieveButton.UseVisualStyleBackColor = true;
            this.SwitchSieveButton.Click += new System.EventHandler(this.SwitchSieveButton_Click);
            // 
            // StepLabel
            // 
            this.StepLabel.AutoSize = true;
            this.StepLabel.Location = new System.Drawing.Point(316, 119);
            this.StepLabel.Name = "StepLabel";
            this.StepLabel.Size = new System.Drawing.Size(0, 15);
            this.StepLabel.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(277, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Step:";
            // 
            // SieveTypeLabel
            // 
            this.SieveTypeLabel.AutoSize = true;
            this.SieveTypeLabel.Location = new System.Drawing.Point(277, 23);
            this.SieveTypeLabel.Name = "SieveTypeLabel";
            this.SieveTypeLabel.Size = new System.Drawing.Size(74, 15);
            this.SieveTypeLabel.TabIndex = 6;
            this.SieveTypeLabel.Text = "Eratosthenes";
            // 
            // UserControl3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SieveTypeLabel);
            this.Controls.Add(this.StepLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SwitchSieveButton);
            this.Controls.Add(this.AutoButton);
            this.Controls.Add(this.PrevButton);
            this.Controls.Add(this.NextButton);
            this.Name = "UserControl3";
            this.Size = new System.Drawing.Size(366, 238);
            this.Load += new System.EventHandler(this.UserControl2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button NextButton;
        private Button PrevButton;
        private Button AutoButton;
        private Button SwitchSieveButton;
        private Label StepLabel;
        private Label label1;
        private Label SieveTypeLabel;
    }
}
