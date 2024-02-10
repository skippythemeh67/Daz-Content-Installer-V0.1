namespace Daz_Content_Installer_V0._1
{
    partial class Form2
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
            dataGridView1 = new DataGridView();
            button1 = new Button();
            button2 = new Button();
            RuntimeBrowserDialog = new FolderBrowserDialog();
            BtnDelRuntime = new Button();
            BxRuntimeFolder = new TextBox();
            BxRuntimeShortName = new TextBox();
            BtnRuntimeBrowse = new Button();
            label1 = new Label();
            label2 = new Label();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            ChkDelete = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = SystemColors.Control;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(6, 30);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.RowTemplate.Height = 33;
            dataGridView1.Size = new Size(1226, 138);
            dataGridView1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(1102, 53);
            button1.Name = "button1";
            button1.Size = new Size(130, 34);
            button1.TabIndex = 1;
            button1.Text = "Add Runtime";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(1004, 174);
            button2.Name = "button2";
            button2.Size = new Size(228, 34);
            button2.TabIndex = 2;
            button2.Text = "Remove Runtime from list";
            button2.UseVisualStyleBackColor = true;
            // 
            // BtnDelRuntime
            // 
            BtnDelRuntime.BackColor = SystemColors.Control;
            BtnDelRuntime.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            BtnDelRuntime.ForeColor = Color.FromArgb(192, 0, 0);
            BtnDelRuntime.Location = new Point(414, 25);
            BtnDelRuntime.Name = "BtnDelRuntime";
            BtnDelRuntime.Size = new Size(172, 34);
            BtnDelRuntime.TabIndex = 3;
            BtnDelRuntime.Text = "Delete Runtime";
            BtnDelRuntime.UseVisualStyleBackColor = false;
            // 
            // BxRuntimeFolder
            // 
            BxRuntimeFolder.Location = new Point(333, 55);
            BxRuntimeFolder.Name = "BxRuntimeFolder";
            BxRuntimeFolder.Size = new Size(763, 31);
            BxRuntimeFolder.TabIndex = 5;
            BxRuntimeFolder.TextChanged += BxRuntimeFolder_TextChanged;
            // 
            // BxRuntimeShortName
            // 
            BxRuntimeShortName.Location = new Point(6, 55);
            BxRuntimeShortName.Name = "BxRuntimeShortName";
            BxRuntimeShortName.Size = new Size(321, 31);
            BxRuntimeShortName.TabIndex = 6;
            BxRuntimeShortName.TextChanged += BxRuntimeShortName_TextChanged;
            // 
            // BtnRuntimeBrowse
            // 
            BtnRuntimeBrowse.Location = new Point(333, 92);
            BtnRuntimeBrowse.Name = "BtnRuntimeBrowse";
            BtnRuntimeBrowse.Size = new Size(99, 34);
            BtnRuntimeBrowse.TabIndex = 7;
            BtnRuntimeBrowse.Text = "Browse";
            BtnRuntimeBrowse.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 27);
            label1.Name = "label1";
            label1.Size = new Size(134, 25);
            label1.TabIndex = 8;
            label1.Text = "Runtime Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(333, 27);
            label2.Name = "label2";
            label2.Size = new Size(154, 25);
            label2.TabIndex = 9;
            label2.Text = "Runtime Location:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(BxRuntimeFolder);
            groupBox1.Controls.Add(BtnRuntimeBrowse);
            groupBox1.Controls.Add(BxRuntimeShortName);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1238, 138);
            groupBox1.TabIndex = 11;
            groupBox1.TabStop = false;
            groupBox1.Text = "Add New Runtime:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(groupBox3);
            groupBox2.Controls.Add(dataGridView1);
            groupBox2.Controls.Add(button2);
            groupBox2.Location = new Point(12, 156);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(1238, 289);
            groupBox2.TabIndex = 12;
            groupBox2.TabStop = false;
            groupBox2.Text = "Manage Runtimes:";
            // 
            // groupBox3
            // 
            groupBox3.BackColor = SystemColors.Control;
            groupBox3.Controls.Add(ChkDelete);
            groupBox3.Controls.Add(BtnDelRuntime);
            groupBox3.ForeColor = Color.FromArgb(192, 0, 0);
            groupBox3.Location = new Point(640, 214);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(592, 69);
            groupBox3.TabIndex = 5;
            groupBox3.TabStop = false;
            groupBox3.Text = "Delete Runtime from Drive";
            // 
            // ChkDelete
            // 
            ChkDelete.AutoSize = true;
            ChkDelete.ForeColor = SystemColors.ControlText;
            ChkDelete.Location = new Point(6, 29);
            ChkDelete.Name = "ChkDelete";
            ChkDelete.Size = new Size(306, 29);
            ChkDelete.TabIndex = 4;
            ChkDelete.Text = "Check to enable Runtime Deletion";
            ChkDelete.UseVisualStyleBackColor = true;
            ChkDelete.CheckedChanged += ChkDelete_CheckedChanged;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1262, 456);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form2";
            Text = "Runtime Management";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private Button button1;
        private Button button2;
        private FolderBrowserDialog RuntimeBrowserDialog;
        private Button BtnDelRuntime;
        private TextBox BxRuntimeFolder;
        private TextBox BxRuntimeShortName;
        private Button BtnRuntimeBrowse;
        private Label label1;
        private Label label2;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private CheckBox ChkDelete;
        private GroupBox groupBox3;
    }
}