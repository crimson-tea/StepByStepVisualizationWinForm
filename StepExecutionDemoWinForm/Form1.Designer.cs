namespace AnimationWinForm
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
            this.SwitchButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SwitchButton
            // 
            this.SwitchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SwitchButton.Location = new System.Drawing.Point(306, 221);
            this.SwitchButton.Name = "SwitchButton";
            this.SwitchButton.Size = new System.Drawing.Size(75, 23);
            this.SwitchButton.TabIndex = 0;
            this.SwitchButton.Text = "Switch2";
            this.SwitchButton.UseVisualStyleBackColor = true;
            this.SwitchButton.Click += new System.EventHandler(this.SwitchButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 256);
            this.Controls.Add(this.SwitchButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion

        private Button SwitchButton;
    }
}