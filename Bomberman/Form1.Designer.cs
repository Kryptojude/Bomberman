namespace Bomberman
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.startScreen = new System.Windows.Forms.Panel();
            this.l3 = new System.Windows.Forms.PictureBox();
            this.l2 = new System.Windows.Forms.PictureBox();
            this.l1 = new System.Windows.Forms.PictureBox();
            this.l0 = new System.Windows.Forms.PictureBox();
            this.startButton = new System.Windows.Forms.Button();
            this.startScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.l3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.l2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.l1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.l0)).BeginInit();
            this.SuspendLayout();
            // 
            // startScreen
            // 
            this.startScreen.BackColor = System.Drawing.Color.Coral;
            this.startScreen.Controls.Add(this.l3);
            this.startScreen.Controls.Add(this.l2);
            this.startScreen.Controls.Add(this.l1);
            this.startScreen.Controls.Add(this.l0);
            this.startScreen.Controls.Add(this.startButton);
            this.startScreen.Location = new System.Drawing.Point(0, 0);
            this.startScreen.Name = "startScreen";
            this.startScreen.Size = new System.Drawing.Size(400, 400);
            this.startScreen.TabIndex = 2;
            // 
            // l3
            // 
            this.l3.Location = new System.Drawing.Point(238, 192);
            this.l3.Name = "l3";
            this.l3.Size = new System.Drawing.Size(100, 100);
            this.l3.TabIndex = 7;
            this.l3.TabStop = false;
            this.l3.Click += new System.EventHandler(this.SelectLevel);
            // 
            // l2
            // 
            this.l2.Location = new System.Drawing.Point(60, 192);
            this.l2.Name = "l2";
            this.l2.Size = new System.Drawing.Size(100, 100);
            this.l2.TabIndex = 6;
            this.l2.TabStop = false;
            this.l2.Click += new System.EventHandler(this.SelectLevel);
            // 
            // l1
            // 
            this.l1.Image = global::Bomberman.Properties.Resources.map1;
            this.l1.Location = new System.Drawing.Point(238, 51);
            this.l1.Name = "l1";
            this.l1.Size = new System.Drawing.Size(100, 100);
            this.l1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.l1.TabIndex = 5;
            this.l1.TabStop = false;
            this.l1.Click += new System.EventHandler(this.SelectLevel);
            // 
            // l0
            // 
            this.l0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.l0.Image = global::Bomberman.Properties.Resources.map0;
            this.l0.Location = new System.Drawing.Point(60, 51);
            this.l0.Name = "l0";
            this.l0.Size = new System.Drawing.Size(100, 100);
            this.l0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.l0.TabIndex = 4;
            this.l0.TabStop = false;
            this.l0.Click += new System.EventHandler(this.SelectLevel);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(162, 337);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 3;
            this.startButton.Text = "Start!";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartGame);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.startScreen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            this.startScreen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.l3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.l2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.l1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.l0)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel startScreen;
        private System.Windows.Forms.PictureBox l3;
        private System.Windows.Forms.PictureBox l2;
        private System.Windows.Forms.PictureBox l1;
        private System.Windows.Forms.PictureBox l0;
        private System.Windows.Forms.Button startButton;
    }
}

