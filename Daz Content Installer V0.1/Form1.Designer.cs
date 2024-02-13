
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            BtnInputFolder = new Button();
            CmboSelRuntime = new ComboBox();
            BtnInstallContent = new Button();
            Tab = new TabControl();
            tabPage1 = new TabPage();
            BtnClearList = new Button();
            ChkShowLog = new CheckBox();
            label4 = new Label();
            BxShowRTPath = new TextBox();
            ProgBar = new ProgressBar();
            label3 = new Label();
            BxArchiveList = new TextBox();
            BxMoveLocation = new TextBox();
            BtnSelMoveFolder = new Button();
            ChkMoveArchives = new CheckBox();
            BtnSelZipFile = new Button();
            LblSelRuntime = new Label();
            tabPage2 = new TabPage();
            BtnShowAll = new Button();
            BtnClearFileList = new Button();
            BtnShowAllFiles = new Button();
            LstArchive = new ListBox();
            BtnUninstallALL = new Button();
            BtnShowFiles = new Button();
            BtnSafeUninstall = new Button();
            BxArchiveSearch = new TextBox();
            LblArchiveSearch = new Label();
            BxWarning = new TextBox();
            tabPage4 = new TabPage();
            label6 = new Label();
            groupBox1 = new GroupBox();
            ListBoxRT = new ListBox();
            button2 = new Button();
            label1 = new Label();
            label2 = new Label();
            button1 = new Button();
            BxRuntimeFolder = new TextBox();
            BtnRuntimeBrowse = new Button();
            BxRuntimeShortName = new TextBox();
            tabPage3 = new TabPage();
            textBox2 = new TextBox();
            label7 = new Label();
            BtnBackupDB = new Button();
            BtnRestoreDB = new Button();
            BxConsole = new TextBox();
            saveDBDialog = new SaveFileDialog();
            openZipFileDialog = new OpenFileDialog();
            ArchiveFolderDialog = new FolderBrowserDialog();
            RuntimeBrowserDialog = new FolderBrowserDialog();
            moveBrowserDialog = new FolderBrowserDialog();
            fileSystemWatcher1 = new FileSystemWatcher();
            Tab.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).BeginInit();
            SuspendLayout();
            // 
            // BtnInputFolder
            // 
            BtnInputFolder.Location = new Point(5, 5);
            BtnInputFolder.Margin = new Padding(2);
            BtnInputFolder.Name = "BtnInputFolder";
            BtnInputFolder.Size = new Size(232, 36);
            BtnInputFolder.TabIndex = 0;
            BtnInputFolder.Text = "Select Input Folder";
            BtnInputFolder.UseVisualStyleBackColor = true;
            BtnInputFolder.Click += BtnInputFolder_Click;
            // 
            // CmboSelRuntime
            // 
            CmboSelRuntime.FormattingEnabled = true;
            CmboSelRuntime.Location = new Point(6, 305);
            CmboSelRuntime.Margin = new Padding(2);
            CmboSelRuntime.Name = "CmboSelRuntime";
            CmboSelRuntime.Size = new Size(303, 33);
            CmboSelRuntime.TabIndex = 2;
            CmboSelRuntime.SelectedIndexChanged += CmboSelRuntime_SelectedIndexChanged;
            CmboSelRuntime.Click += CmboSelRuntime_SelectedIndexChanged_1;
            // 
            // BtnInstallContent
            // 
            BtnInstallContent.Location = new Point(7, 416);
            BtnInstallContent.Margin = new Padding(2);
            BtnInstallContent.Name = "BtnInstallContent";
            BtnInstallContent.Size = new Size(206, 36);
            BtnInstallContent.TabIndex = 4;
            BtnInstallContent.Text = "Install Content";
            BtnInstallContent.UseVisualStyleBackColor = true;
            BtnInstallContent.Click += BtnInstallContent_Click;
            // 
            // Tab
            // 
            Tab.Controls.Add(tabPage1);
            Tab.Controls.Add(tabPage2);
            Tab.Controls.Add(tabPage4);
            Tab.Controls.Add(tabPage3);
            Tab.Location = new Point(10, 12);
            Tab.Name = "Tab";
            Tab.SelectedIndex = 0;
            Tab.Size = new Size(1003, 535);
            Tab.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(BtnClearList);
            tabPage1.Controls.Add(ChkShowLog);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(BxShowRTPath);
            tabPage1.Controls.Add(ProgBar);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(BxArchiveList);
            tabPage1.Controls.Add(BxMoveLocation);
            tabPage1.Controls.Add(BtnSelMoveFolder);
            tabPage1.Controls.Add(ChkMoveArchives);
            tabPage1.Controls.Add(BtnSelZipFile);
            tabPage1.Controls.Add(LblSelRuntime);
            tabPage1.Controls.Add(BtnInputFolder);
            tabPage1.Controls.Add(BtnInstallContent);
            tabPage1.Controls.Add(CmboSelRuntime);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(995, 497);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Install";
            tabPage1.UseVisualStyleBackColor = true;
            tabPage1.Click += tabPage1_Click;
            // 
            // BtnClearList
            // 
            BtnClearList.Location = new Point(877, 6);
            BtnClearList.Name = "BtnClearList";
            BtnClearList.Size = new Size(112, 34);
            BtnClearList.TabIndex = 17;
            BtnClearList.Text = "Clear List";
            BtnClearList.UseVisualStyleBackColor = true;
            BtnClearList.Click += BtnClearList_Click;
            // 
            // ChkShowLog
            // 
            ChkShowLog.AutoSize = true;
            ChkShowLog.Checked = true;
            ChkShowLog.CheckState = CheckState.Checked;
            ChkShowLog.Location = new Point(838, 423);
            ChkShowLog.Name = "ChkShowLog";
            ChkShowLog.Size = new Size(151, 29);
            ChkShowLog.TabIndex = 16;
            ChkShowLog.Text = "Show Console";
            ChkShowLog.UseVisualStyleBackColor = true;
            ChkShowLog.CheckedChanged += ChkShowLog_CheckedChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(314, 277);
            label4.Name = "label4";
            label4.Size = new Size(121, 25);
            label4.TabIndex = 15;
            label4.Text = "Runtime Path:";
            // 
            // BxShowRTPath
            // 
            BxShowRTPath.Location = new Point(314, 305);
            BxShowRTPath.Name = "BxShowRTPath";
            BxShowRTPath.ReadOnly = true;
            BxShowRTPath.Size = new Size(675, 31);
            BxShowRTPath.TabIndex = 14;
            BxShowRTPath.TextChanged += BxShowRTPath_TextChanged;
            // 
            // ProgBar
            // 
            ProgBar.Location = new Point(7, 457);
            ProgBar.Name = "ProgBar";
            ProgBar.Size = new Size(983, 34);
            ProgBar.TabIndex = 8;
            ProgBar.Click += progressBar1_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 43);
            label3.Name = "label3";
            label3.Size = new Size(199, 25);
            label3.TabIndex = 13;
            label3.Text = "Archives to be installed:";
            // 
            // BxArchiveList
            // 
            BxArchiveList.BackColor = SystemColors.Control;
            BxArchiveList.Location = new Point(6, 71);
            BxArchiveList.Multiline = true;
            BxArchiveList.Name = "BxArchiveList";
            BxArchiveList.ReadOnly = true;
            BxArchiveList.ScrollBars = ScrollBars.Both;
            BxArchiveList.Size = new Size(983, 204);
            BxArchiveList.TabIndex = 12;
            BxArchiveList.WordWrap = false;
            BxArchiveList.TextChanged += BxArchivesList_TextChanged;
            // 
            // BxMoveLocation
            // 
            BxMoveLocation.Location = new Point(158, 379);
            BxMoveLocation.Name = "BxMoveLocation";
            BxMoveLocation.Size = new Size(832, 31);
            BxMoveLocation.TabIndex = 10;
            BxMoveLocation.Visible = false;
            BxMoveLocation.TextChanged += BxMoveLocation_TextChanged;
            // 
            // BtnSelMoveFolder
            // 
            BtnSelMoveFolder.Location = new Point(7, 377);
            BtnSelMoveFolder.Name = "BtnSelMoveFolder";
            BtnSelMoveFolder.Size = new Size(145, 34);
            BtnSelMoveFolder.TabIndex = 9;
            BtnSelMoveFolder.Text = "Move Location:";
            BtnSelMoveFolder.UseVisualStyleBackColor = true;
            BtnSelMoveFolder.Visible = false;
            BtnSelMoveFolder.Click += BtnSelMoveFolder_Click;
            // 
            // ChkMoveArchives
            // 
            ChkMoveArchives.AutoSize = true;
            ChkMoveArchives.Location = new Point(6, 343);
            ChkMoveArchives.Name = "ChkMoveArchives";
            ChkMoveArchives.Size = new Size(537, 29);
            ChkMoveArchives.TabIndex = 8;
            ChkMoveArchives.Text = "(Optional) Move successfully installed archives to new location?";
            ChkMoveArchives.UseVisualStyleBackColor = true;
            ChkMoveArchives.CheckedChanged += ChkMoveArchives_Changed;
            // 
            // BtnSelZipFile
            // 
            BtnSelZipFile.Location = new Point(252, 6);
            BtnSelZipFile.Name = "BtnSelZipFile";
            BtnSelZipFile.Size = new Size(203, 34);
            BtnSelZipFile.TabIndex = 7;
            BtnSelZipFile.Text = "Select Individual File(s)";
            BtnSelZipFile.UseVisualStyleBackColor = true;
            BtnSelZipFile.Click += BtnSelZipFile_Click;
            // 
            // LblSelRuntime
            // 
            LblSelRuntime.AutoSize = true;
            LblSelRuntime.Location = new Point(6, 278);
            LblSelRuntime.Name = "LblSelRuntime";
            LblSelRuntime.Size = new Size(133, 25);
            LblSelRuntime.TabIndex = 6;
            LblSelRuntime.Text = "Select Runtime:";
            LblSelRuntime.Click += LblSelRuntime_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(BtnShowAll);
            tabPage2.Controls.Add(BtnClearFileList);
            tabPage2.Controls.Add(BtnShowAllFiles);
            tabPage2.Controls.Add(LstArchive);
            tabPage2.Controls.Add(BtnUninstallALL);
            tabPage2.Controls.Add(BtnShowFiles);
            tabPage2.Controls.Add(BtnSafeUninstall);
            tabPage2.Controls.Add(BxArchiveSearch);
            tabPage2.Controls.Add(LblArchiveSearch);
            tabPage2.Controls.Add(BxWarning);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(995, 497);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Uninstall";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // BtnShowAll
            // 
            BtnShowAll.Location = new Point(815, 55);
            BtnShowAll.Name = "BtnShowAll";
            BtnShowAll.Size = new Size(171, 34);
            BtnShowAll.TabIndex = 12;
            BtnShowAll.Text = "Show All Archives";
            BtnShowAll.UseVisualStyleBackColor = true;
            BtnShowAll.Click += BtnShowAll_Click;
            // 
            // BtnClearFileList
            // 
            BtnClearFileList.Location = new Point(19, 453);
            BtnClearFileList.Name = "BtnClearFileList";
            BtnClearFileList.Size = new Size(967, 34);
            BtnClearFileList.TabIndex = 11;
            BtnClearFileList.Text = "Clear List of Files";
            BtnClearFileList.UseVisualStyleBackColor = true;
            BtnClearFileList.Click += BtnClearFileList_Click;
            // 
            // BtnShowAllFiles
            // 
            BtnShowAllFiles.Location = new Point(747, 364);
            BtnShowAllFiles.Name = "BtnShowAllFiles";
            BtnShowAllFiles.Size = new Size(239, 34);
            BtnShowAllFiles.TabIndex = 10;
            BtnShowAllFiles.Text = "Show ALL Files";
            BtnShowAllFiles.UseVisualStyleBackColor = true;
            BtnShowAllFiles.Click += BtnShowAllFiles_Click;
            // 
            // LstArchive
            // 
            LstArchive.FormattingEnabled = true;
            LstArchive.ItemHeight = 25;
            LstArchive.Location = new Point(19, 95);
            LstArchive.Name = "LstArchive";
            LstArchive.Size = new Size(970, 254);
            LstArchive.TabIndex = 9;
            LstArchive.SelectedIndexChanged += LstArchive_SelectedIndexChanged;
            // 
            // BtnUninstallALL
            // 
            BtnUninstallALL.Location = new Point(747, 411);
            BtnUninstallALL.Name = "BtnUninstallALL";
            BtnUninstallALL.Size = new Size(242, 34);
            BtnUninstallALL.TabIndex = 5;
            BtnUninstallALL.Text = "Uninstall ALL Files";
            BtnUninstallALL.UseVisualStyleBackColor = true;
            BtnUninstallALL.Click += BtnUninstallALL_Click;
            // 
            // BtnShowFiles
            // 
            BtnShowFiles.Location = new Point(19, 366);
            BtnShowFiles.Name = "BtnShowFiles";
            BtnShowFiles.Size = new Size(150, 34);
            BtnShowFiles.TabIndex = 4;
            BtnShowFiles.Text = "Show Safe Files";
            BtnShowFiles.UseVisualStyleBackColor = true;
            BtnShowFiles.Click += BtnShowFiles_Click;
            // 
            // BtnSafeUninstall
            // 
            BtnSafeUninstall.Location = new Point(19, 413);
            BtnSafeUninstall.Name = "BtnSafeUninstall";
            BtnSafeUninstall.Size = new Size(150, 34);
            BtnSafeUninstall.TabIndex = 3;
            BtnSafeUninstall.Text = "Safe Uninstall";
            BtnSafeUninstall.UseVisualStyleBackColor = true;
            BtnSafeUninstall.Click += BtnSafeUninstall_Click;
            // 
            // BxArchiveSearch
            // 
            BxArchiveSearch.Location = new Point(19, 49);
            BxArchiveSearch.Name = "BxArchiveSearch";
            BxArchiveSearch.Size = new Size(400, 31);
            BxArchiveSearch.TabIndex = 1;
            BxArchiveSearch.TextChanged += BxArchiveSearch_TextChanged;
            // 
            // LblArchiveSearch
            // 
            LblArchiveSearch.AutoSize = true;
            LblArchiveSearch.Location = new Point(19, 21);
            LblArchiveSearch.Name = "LblArchiveSearch";
            LblArchiveSearch.Size = new Size(200, 25);
            LblArchiveSearch.TabIndex = 0;
            LblArchiveSearch.Text = "Archive name to search:";
            // 
            // BxWarning
            // 
            BxWarning.BackColor = Color.FromArgb(255, 255, 192);
            BxWarning.Location = new Point(175, 366);
            BxWarning.Multiline = true;
            BxWarning.Name = "BxWarning";
            BxWarning.ReadOnly = true;
            BxWarning.Size = new Size(566, 79);
            BxWarning.TabIndex = 6;
            BxWarning.Text = "WARNING: 'Unistall ALL Files' will remove all files, including any files that overwrote existing ones when installed. This may break other content or even Daz Studio. Use with caution!  ";
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(label6);
            tabPage4.Controls.Add(groupBox1);
            tabPage4.Controls.Add(ListBoxRT);
            tabPage4.Controls.Add(button2);
            tabPage4.Controls.Add(label1);
            tabPage4.Controls.Add(label2);
            tabPage4.Controls.Add(button1);
            tabPage4.Controls.Add(BxRuntimeFolder);
            tabPage4.Controls.Add(BtnRuntimeBrowse);
            tabPage4.Controls.Add(BxRuntimeShortName);
            tabPage4.Location = new Point(4, 34);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(995, 497);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Manage Runtimes";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(133, 432);
            label6.Name = "label6";
            label6.Size = new Size(375, 25);
            label6.TabIndex = 20;
            label6.Text = "(Removes from list only. Does not affect files.)";
            // 
            // groupBox1
            // 
            groupBox1.Location = new Point(559, 465);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(8, 8);
            groupBox1.TabIndex = 19;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // ListBoxRT
            // 
            ListBoxRT.FormattingEnabled = true;
            ListBoxRT.ItemHeight = 25;
            ListBoxRT.Location = new Point(6, 142);
            ListBoxRT.Name = "ListBoxRT";
            ListBoxRT.Size = new Size(980, 279);
            ListBoxRT.TabIndex = 18;
            ListBoxRT.SelectedIndexChanged += ListBoxRT_SelectedIndexChanged;
            // 
            // button2
            // 
            button2.Location = new Point(6, 427);
            button2.Name = "button2";
            button2.Size = new Size(121, 34);
            button2.TabIndex = 17;
            button2.Text = "Remove Runtime from list";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 27);
            label1.Name = "label1";
            label1.Size = new Size(134, 25);
            label1.TabIndex = 14;
            label1.Text = "Runtime Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 67);
            label2.Name = "label2";
            label2.Size = new Size(154, 25);
            label2.TabIndex = 15;
            label2.Text = "Runtime Location:";
            // 
            // button1
            // 
            button1.Location = new Point(851, 62);
            button1.Name = "button1";
            button1.Size = new Size(130, 34);
            button1.TabIndex = 10;
            button1.Text = "Add Runtime";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // BxRuntimeFolder
            // 
            BxRuntimeFolder.Location = new Point(271, 64);
            BxRuntimeFolder.Name = "BxRuntimeFolder";
            BxRuntimeFolder.Size = new Size(574, 31);
            BxRuntimeFolder.TabIndex = 11;
            BxRuntimeFolder.TextChanged += BxRuntimeFolder_TextChanged;
            // 
            // BtnRuntimeBrowse
            // 
            BtnRuntimeBrowse.Location = new Point(166, 62);
            BtnRuntimeBrowse.Name = "BtnRuntimeBrowse";
            BtnRuntimeBrowse.Size = new Size(99, 34);
            BtnRuntimeBrowse.TabIndex = 13;
            BtnRuntimeBrowse.Text = "Browse";
            BtnRuntimeBrowse.UseVisualStyleBackColor = true;
            BtnRuntimeBrowse.Click += BtnRuntimeBrowse_Click;
            // 
            // BxRuntimeShortName
            // 
            BxRuntimeShortName.Location = new Point(271, 24);
            BxRuntimeShortName.Name = "BxRuntimeShortName";
            BxRuntimeShortName.Size = new Size(315, 31);
            BxRuntimeShortName.TabIndex = 12;
            BxRuntimeShortName.TextChanged += BxRuntimeShortName_TextChanged;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(textBox2);
            tabPage3.Controls.Add(label7);
            tabPage3.Controls.Add(BtnBackupDB);
            tabPage3.Controls.Add(BtnRestoreDB);
            tabPage3.Location = new Point(4, 34);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(995, 497);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Database Backup";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(255, 255, 192);
            textBox2.Location = new Point(17, 160);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(972, 37);
            textBox2.TabIndex = 3;
            textBox2.Text = "For manual back-up copy the file 'InstallerFiles.db' from the application folder to another location.\r\n";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
            label7.Location = new Point(17, 17);
            label7.Name = "label7";
            label7.Size = new Size(189, 25);
            label7.TabIndex = 2;
            label7.Text = "(Not implemented yet)";
            // 
            // BtnBackupDB
            // 
            BtnBackupDB.Enabled = false;
            BtnBackupDB.Location = new Point(17, 61);
            BtnBackupDB.Name = "BtnBackupDB";
            BtnBackupDB.Size = new Size(164, 34);
            BtnBackupDB.TabIndex = 0;
            BtnBackupDB.Text = "Backup Database";
            BtnBackupDB.UseVisualStyleBackColor = true;
            BtnBackupDB.Click += BtnBackupDB_Click;
            // 
            // BtnRestoreDB
            // 
            BtnRestoreDB.Enabled = false;
            BtnRestoreDB.Location = new Point(17, 101);
            BtnRestoreDB.Name = "BtnRestoreDB";
            BtnRestoreDB.Size = new Size(265, 34);
            BtnRestoreDB.TabIndex = 1;
            BtnRestoreDB.Text = "Restore Database from Backup";
            BtnRestoreDB.UseVisualStyleBackColor = true;
            BtnRestoreDB.Click += BtnRestoreDB_Click;
            // 
            // BxConsole
            // 
            BxConsole.Location = new Point(10, 549);
            BxConsole.Multiline = true;
            BxConsole.Name = "BxConsole";
            BxConsole.ScrollBars = ScrollBars.Vertical;
            BxConsole.Size = new Size(999, 424);
            BxConsole.TabIndex = 7;
            BxConsole.TextChanged += BxConsole_TextChanged;
            // 
            // openZipFileDialog
            // 
            openZipFileDialog.FileName = "openZipFileDialog";
            openZipFileDialog.FileOk += openZipDialog_FileOk;
            // 
            // ArchiveFolderDialog
            // 
            ArchiveFolderDialog.HelpRequest += ArchiveFolderDialog_HelpRequest;
            // 
            // fileSystemWatcher1
            // 
            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.SynchronizingObject = this;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1015, 979);
            Controls.Add(BxConsole);
            Controls.Add(Tab);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            Name = "Main";
            Text = "Daz Studio Content Installer";
            Load += Form1_Load;
            Tab.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }



        #endregion

        private Button BtnInputFolder;
        private ComboBox CmboSelRuntime;
        private Button BtnInstallContent;
        private TabControl Tab;
        private TabPage tabPage1;
        private TextBox BxConsole;
        private Label LblSelRuntime;
        private TabPage tabPage2;
        private Button BtnSafeUninstall;
        private TextBox BxArchiveSearch;
        private Label LblArchiveSearch;
        private Button BtnUninstallALL;
        private Button BtnShowFiles;
        private TextBox BxWarning;
        private TabPage tabPage3;
        private Button BtnRestoreDB;
        private Button BtnBackupDB;
        private SaveFileDialog saveDBDialog;
        private OpenFileDialog openZipFileDialog;
        private FolderBrowserDialog ArchiveFolderDialog;
        private ProgressBar ProgBar;
        private Button BtnSelZipFile;
        private TextBox BxMoveLocation;
        private Button BtnSelMoveFolder;
        private CheckBox ChkMoveArchives;
        private TextBox BxArchiveList;
        private TabPage tabPage4;
        private Label label1;
        private Label label2;
        private Button button1;
        private TextBox BxRuntimeFolder;
        private Button BtnRuntimeBrowse;
        private TextBox BxRuntimeShortName;
        private Button button2;
        private FolderBrowserDialog RuntimeBrowserDialog;
        private Label label3;
        private CheckBox ChkShowLog;
        private Label label4;
        private TextBox BxShowRTPath;
        private Button BtnClearList;
        private FolderBrowserDialog moveBrowserDialog;
        private ListBox ListBoxRT;
        private FileSystemWatcher fileSystemWatcher1;
        private Label label6;
        private GroupBox groupBox1;
        private Label label7;
        private TextBox textBox2;
        private ListBox LstArchive;
        private Button BtnShowAllFiles;
        private Button BtnClearFileList;
        private Button BtnShowAll;
    }
}
