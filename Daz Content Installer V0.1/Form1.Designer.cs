namespace Daz_Content_Installer_V0._1
{
    partial class Main
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
            button1 = new Button();
            button2 = new Button();
            comboBox1 = new ComboBox();
            button3 = new Button();
            button4 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(64, 80);
            button1.Name = "button1";
            button1.Size = new Size(301, 46);
            button1.TabIndex = 0;
            button1.Text = "Select Input Folder";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(86, 164);
            button2.Name = "button2";
            button2.Size = new Size(279, 46);
            button2.TabIndex = 1;
            button2.Text = "Select Output Folder";
            button2.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(88, 261);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(322, 40);
            comboBox1.TabIndex = 2;
            // 
            // button3
            // 
            button3.Location = new Point(442, 255);
            button3.Name = "button3";
            button3.Size = new Size(175, 46);
            button3.TabIndex = 3;
            button3.Text = "Add Runtime";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(97, 372);
            button4.Name = "button4";
            button4.Size = new Size(268, 46);
            button4.TabIndex = 4;
            button4.Text = "Install Content";
            button4.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1150, 784);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(comboBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Main";
            Text = "Daz Studio Content Installer";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private ComboBox comboBox1;
        private Button button3;
        private Button button4;
    }
}
