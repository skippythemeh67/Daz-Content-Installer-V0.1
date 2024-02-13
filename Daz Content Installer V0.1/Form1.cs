using SharpCompress.Archives;
using SharpCompress.Common;
using System.Data.SQLite;
using static System.Environment;

namespace Daz_Content_Installer_V0._1
{
    public partial class Main : Form
    {
        private static List<string> specialFolders = new List<string>
        {
        "data",
        "people",
        "props",
        "runtime",
        "Documentation",
        "Environments",
        "Figures",
        "Light Presets",
        "Materials",
        "plugins",
        "poses",
        "Presets",
        "Render Presets",
        "Render Settings",
        "Scenes",
        "Scripts",
        "Shader Presets",
        "Shaders",
        "Shaping",
        "Wearables"
         };
        string newRuntimeFolder;
        string newRTShortname;
        static string folderLocation;
        static string tempDirectory;
        static string inputArchive;
        static bool overWrite;
        static string inputFolder;
        static string destFolder;
        List<string> archiveFiles;
        int archiveCount;
        string runtimeLocation;
        string moveFolder;
        string selectedRuntime;
        List<string> errorList;
        private List<string> fileListForSelectedArchive = new List<string>(); // Define a class-level variable to hold the file list
        int archiveID;


        private static SQLiteConnection connection;

        public Main()
        {
            InitializeComponent();
            this.Load += MainForm_Load; // Subscribe to the Load event
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Call the DB init method for initialization
            InitDatabase();
            CheckAndSwitchTabRuntimeEmpty(connection);
            DefArchiveList();


            errorList = new List<string>();

            // Unsubscribe from the Load event to ensure it runs only once
            this.Load -= MainForm_Load;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateRTComboBox();
        }
        private void Log(string message)
        {
            if (BxConsole.InvokeRequired)
            {
                // If we're not on the UI thread, marshal the call to the UI thread
                BxConsole.BeginInvoke(new Action<string>(Log), message);
            }
            else
            {
                // If we're already on the UI thread, update the control directly
                BxConsole.AppendText(message + Environment.NewLine);
            }
        }
        private void DefArchiveList()
        {
            PopulateListBox();
            List<string> AllArchives = AllArchivesByName();
            foreach (string archiveName in AllArchives)
            {
                LstArchive.Items.Add(archiveName);
            }
        }
        private void CheckAndSwitchTabRuntimeEmpty(SQLiteConnection connection)
        {
            string tableName = "Runtimes";
            string query = $"SELECT COUNT(*) FROM {tableName}";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                if (rowCount == 0)
                {
                    // Show warning message to the user
                    MessageBox.Show("No Runtimes configured!\n\nSwitching to the Runtime Management tab on launch.\n\nPlease add at least one Runtime.\n\n(This is normal if this is the first time you have run this application.)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Switch to a specific tab (assuming yourTabControl is the name of your TabControl)
                    Tab.SelectTab(tabPage4); // Specify your desired tab page
                }
            }
        }
        private void BtnInputFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = ArchiveFolderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Get the selected folder path and update bxOutDir
                string selectedFolder = ArchiveFolderDialog.SelectedPath;
                List<string> ArchiveList = GetArchiveFiles(selectedFolder);
                BxArchiveList.Text = string.Join(NewLine, ArchiveList);
                inputFolder = selectedFolder;
                // outputFolder = selectedFolder;
                tempDirectory = Path.Combine(selectedFolder, "Temp");
                Log($"Temp Folder set to: {tempDirectory}");
                Log($"Folder scan. Input folder is {inputFolder}");
                archiveFiles = GetArchiveFiles(inputFolder);
                archiveCount = archiveFiles.Count;
                Log($"Number of Archives to process: {archiveFiles.Count}");
            }
        }
        private void BtnSelMoveFolder_Click(object sender, EventArgs e)
        {

            DialogResult result = moveBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Get the selected folder path and update bxOutDir
                moveFolder = moveBrowserDialog.SelectedPath;
                BxMoveLocation.Text = moveFolder;
                Log($"Processed Archives will be moved to: {moveFolder}");
            }
        }
        private void BtnSelZipFile_Click(object sender, EventArgs e)
        {
            openZipFileDialog.Filter = "Archive Files|*.zip;*.rar";
            openZipFileDialog.Multiselect = true;

            DialogResult result = openZipFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Get the selected file names and update zipFile
                string[] selectedFiles = openZipFileDialog.FileNames;
                archiveFiles = new List<string>(selectedFiles);
                BxArchiveList.Text = string.Join(NewLine, archiveFiles);
                tempDirectory = Path.Combine(Path.GetDirectoryName(selectedFiles[0]), "Temp");
                Log($"Temp Folder set to: {tempDirectory}");
                //var archiveFiles = GetArchiveFiles(zipFile);
                Log($"Number of Archives to process: {archiveFiles.Count}");

            }
        }
        private void BtnClearList_Click(object sender, EventArgs e)
        {
            BxArchiveList.Text = "";
            archiveFiles.Clear();
            Log("Install List Cleared!");
        }
        private void BxArchivesList_TextChanged(object sender, EventArgs e)
        {

        }
        private void BtnAddRuntime_Click(object sender, EventArgs e)
        {

        }
        private async void BtnInstallContent_Click(object sender, EventArgs e)
        {
            BtnInstallContent.Enabled = false;

            await Task.Run(() =>
            {
                int currentIteration = 0;

                foreach (var archiveFile in archiveFiles)
                {
                    ProcessFolder(archiveFile, inputFolder, destFolder);
                    // Move the file
                    if (ChkMoveArchives.Checked)
                    {
                        string destinationFilePath = Path.Combine(moveFolder, Path.GetFileName(archiveFile));

                        File.Move(archiveFile, destinationFilePath);
                        Log($"{archiveFile} moved to {destinationFilePath}");
                    }
                    // Increment the current iteration count
                    currentIteration++;

                    // Calculate the percentage of completion
                    int progressPercentage = (int)((double)currentIteration / archiveCount * 100);

                    // Report progress to the UI thread
                    UpdateProgressBar(progressPercentage);

                }
            });
            // Reset the progress bar after the task is completed
            ResetProgressBar();

            // Enable the button after the task is completed
            BtnInstallContent.Enabled = true;
            Log("Operation complete!");
        }
        // Method to update the progress bar value safely from any thread
        private void UpdateProgressBar(int value)
        {
            // Ensure that the value is within the valid range of the progress bar
            int safeValue = Math.Max(ProgBar.Minimum, Math.Min(ProgBar.Maximum, value));

            if (ProgBar.InvokeRequired)
            {
                ProgBar.Invoke((MethodInvoker)(() => ProgBar.Value = safeValue));
            }
            else
            {
                ProgBar.Value = safeValue;
            }
        }
        private void ResetProgressBar()
        {
            if (ProgBar.InvokeRequired)
            {
                ProgBar.Invoke((MethodInvoker)(() => ProgBar.Value = 0));
            }
            else
            {
                ProgBar.Value = 0;
            }
        }
        private void BtnShowFiles_Click(object sender, EventArgs e)
        {
            string selectedArchive = LstArchive.SelectedItem?.ToString();
            GetSafeFileList();
            Log($"SAFE File list for {selectedArchive}");
            foreach (string fileName in fileListForSelectedArchive)
            {
                Log($" {fileName}");
            }
            Log($"Files in List: {fileListForSelectedArchive.Count}");
            Log($"Archive ID = {archiveID}");
        }
        private void BtnUninstallALL_Click(object sender, EventArgs e)
        {
            GetAllFileList();
            DialogResult result = MessageBox.Show($"You are about to remove {fileListForSelectedArchive.Count} files!\nAre you sure?", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
            {
                DeleteFiles(fileListForSelectedArchive);
                DelAllFilesFromDB(archiveID);
                DelArchiveFromDB(archiveID);
                PopulateArchiveListbox();
                archiveID = 0;
                Log($"Archive ID = {archiveID}");
            }
            else if (result == DialogResult.Cancel)
            {
                return;
            }

        }
        private void BtnBackupDB_Click(object sender, EventArgs e)
        {

        }
        private void BtnRestoreDB_Click(object sender, EventArgs e)
        {

        }
        private void LblSelRuntime_Click(object sender, EventArgs e)
        {

        }
        private void ChkMoveArchives_Changed(object sender, EventArgs e)
        {
            if (ChkMoveArchives.Checked)
            {
                BtnSelMoveFolder.Visible = true;
                BxMoveLocation.Visible = true;
            }
            else if (!ChkMoveArchives.Checked)
            {
                BtnSelMoveFolder.Visible = false;
                BxMoveLocation.Visible = false;
            }
        }
        private void CmboSelRuntime_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                // Ensure the database connection is initialized and opened
                InitDatabase();


                // Get the selected runtime name from the combo box
                string selectedRuntime = CmboSelRuntime.SelectedItem?.ToString();

                if (selectedRuntime != null)
                {
                    // SQL query to select the Location from Runtimes table based on the selected runtime name
                    string query = "SELECT Location FROM Runtimes WHERE RuntimeName = @RuntimeName";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RuntimeName", selectedRuntime);
                        runtimeLocation = command.ExecuteScalar()?.ToString();

                        // Populate BxShowRTPath with the location if it's not null
                        if (runtimeLocation != null)
                        {
                            BxShowRTPath.Text = runtimeLocation;
                        }
                        else
                        {
                            // Clear BxShowRTPath if no location is found
                            BxShowRTPath.Text = "";
                        }
                    }
                }
                destFolder = runtimeLocation;
                Log($"Selected Runtime directory: {destFolder}");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Log($"Error populating BxShowRTPath: {ex.Message}");
            }
        }
        private void BtnRuntimes_Click(object sender, EventArgs e)
        {
            {
                // Create an instance of Form2
                // Form2 form2 = new Form2();

                // Pass the SQLite connection to Form2 constructor
                // Form2 form2 = new Form2(connection);

                // Show Form2
                // form2.ShowDialog();
                // OR, if you want Form2 to be modal (blocks interaction with Form1 until Form2 is closed):
                // form2.ShowDialog();
            }
        }
        public void PopulateRTComboBox()
        {
            try
            {
                // Ensure the database connection is initialized and opened
                InitDatabase();

                // SQL query to select RuntimeName values from Runtimes table
                string query = "SELECT RuntimeName FROM Runtimes";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Clear existing items in the ComboBox
                        CmboSelRuntime.Items.Clear();

                        // Iterate through the results and add each RuntimeName value to the ComboBox
                        while (reader.Read())
                        {
                            // Assuming RuntimeName is a string
                            string runtimeName = reader.GetString(0);
                            CmboSelRuntime.Items.Add(runtimeName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Log($"Error populating runtime ComboBox: {ex.Message}");
            }
        }
        public void ProcessFolder(string archiveName, string inputFolder, string outFolder)
        {
            try
            {
                Log($"Processing Archive: {archiveName}");
                string outputFolder = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(archiveName));
                Directory.CreateDirectory(outputFolder);

                int archiveId = AddArchive(Path.GetFileNameWithoutExtension(archiveName), connection);

                ExtractFiles(archiveName, outputFolder);
                inputArchive = Path.GetFileName(archiveName);

                SearchSpecialFolders(outputFolder);

                Log($"Output Folder: {outputFolder}");
                Log($"Extraction completed successfully for '{archiveName}'.");
                Log($"Special folder found at: {folderLocation}");

                List<string> contents = GetFolderContents(folderLocation);
                Log("Contents of the special folder:");
                foreach (var item in contents)
                {
                    Log(item);
                }

                MoveContentsToInput(contents, archiveId, outFolder);

                Log($"Move completed successfully for '{archiveName}'.");
            }
            catch (Exception ex)
            {
                Log($"Error processing '{archiveName}': {ex.Message}");
            }
        }
        public int AddArchive(string archiveName, SQLiteConnection connection)
        {
            string insertOrUpdateQuery = @"
        INSERT OR IGNORE INTO Archives (ArchiveName, DateInstalled) VALUES (@ArchiveName, @DateInstalled);
        UPDATE Archives SET DateInstalled = @DateInstalled WHERE ArchiveName = @ArchiveName;
        SELECT ArchiveId FROM Archives WHERE ArchiveName = @ArchiveName;";

            using (SQLiteCommand command = new SQLiteCommand(insertOrUpdateQuery, connection))
            {
                command.Parameters.AddWithValue("@ArchiveName", archiveName);
                command.Parameters.AddWithValue("@DateInstalled", DateTime.Now);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
        public List<string> GetArchiveFiles(string folderPath)
        {
            var searchPatterns = new[] { "*.zip", "*.rar" };
            var archiveFiles = searchPatterns
                .SelectMany(pattern => Directory.GetFiles(folderPath, pattern))
                .ToList();

            return archiveFiles;
        }
        public void ExtractFiles(string inputFilePath, string outputFolderPath)
        {
            try
            {
                using (var archive = ArchiveFactory.Open(inputFilePath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            entry.WriteToDirectory(outputFolderPath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });

                            if (IsArchiveFile(entry.Key))
                            {
                                string nestedArchivePath = Path.Combine(outputFolderPath, entry.Key);
                                string nestedOutputFolder = Path.Combine(outputFolderPath, Path.GetFileNameWithoutExtension(entry.Key));

                                Directory.CreateDirectory(nestedOutputFolder);

                                ExtractFiles(nestedArchivePath, nestedOutputFolder);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch any exceptions and add the error message to the error list
                errorList.Add(ex.Message);
            }
        }
        public bool IsArchiveFile(string fileName)
        {
            string[] archiveExtensions = { ".zip", ".rar", ".7z" };
            string extension = Path.GetExtension(fileName).ToLower();
            return archiveExtensions.Any(ext => ext == extension);
        }
        public void SearchSpecialFolders(string currentFolderPath)
        {
            foreach (string specialFolder in specialFolders)
            {
                string[] matchingFolders = Directory.GetDirectories(currentFolderPath, specialFolder, SearchOption.AllDirectories);

                if (matchingFolders.Length > 0)
                {
                    folderLocation = Path.GetDirectoryName(matchingFolders[0]);
                    Log($"Debug: Special folder found at: {folderLocation}");
                    return;
                }
            }
        }
        public List<string> GetFolderContents(string folderPath)
        {
            List<string> contents = new List<string>();

            if (Directory.Exists(folderPath))
            {
                contents.AddRange(Directory.GetFiles(folderPath));
                contents.AddRange(Directory.GetDirectories(folderPath));
            }

            return contents;
        }
        public void MoveContentsToInput(List<string> paths, int archiveId, string outputFolder)
        {
            foreach (string path in paths)
            {
                string destinationPath = Path.Combine(outputFolder, Path.GetFileName(path));

                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                CopyAll(new DirectoryInfo(path), new DirectoryInfo(destinationPath), archiveId);

                Log($"Merged contents of Folder: {path}");
            }

            if (Directory.Exists(tempDirectory))
            {
                DeleteFolderContents(tempDirectory);
            }
            else
            {
                Log("Temporary folder does not exist.");
            }
        }
        private void CopyAll(DirectoryInfo source, DirectoryInfo target, int archiveId)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source=InstallerFiles.db;Version=3;"))
            {
                connection.Open();

                foreach (FileInfo fi in source.GetFiles())
                {

                    string namedFile = Path.Combine(target.FullName, fi.Name);
                    bool overWrite = File.Exists(namedFile);
                    Log($"Copying {fi.Name} to {target.FullName}");

                    fi.CopyTo(namedFile, true);

                    InsertFileRecord(connection, archiveId, namedFile, overWrite);
                }

                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyAll(diSourceSubDir, nextTargetSubDir, archiveId);
                }
            }
        }
        public void DeleteFolderContents(string folderPath)
        {
            try
            {
                foreach (var file in Directory.GetFiles(folderPath))
                {
                    File.Delete(file);
                }

                foreach (var subdirectory in Directory.GetDirectories(folderPath))
                {
                    Directory.Delete(subdirectory, true);
                }

                Log("Contents of the folder deleted successfully.");
            }
            catch (Exception ex)
            {
                Log($"Error deleting folder contents: {ex.Message}");
            }
        }
        public void InitDatabase()
        {
            string databasePath = "InstallerFiles.db";

            if (!File.Exists(databasePath))
            {
                CreateDatabase(databasePath);
            }

            connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
            connection.Open();
        }
        public void CreateDatabase(string path)
        {
            SQLiteConnection.CreateFile(path);

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={path};Version=3;"))
            {
                connection.Open();

                string createArchivesTableQuery = @"
                CREATE TABLE IF NOT EXISTS Archives (
                    ArchiveId INTEGER PRIMARY KEY,
                    ArchiveName TEXT,
                    DateInstalled DATETIME,
                    UNIQUE(ArchiveName)
            );";

                string createFilesTableQuery = @"
                CREATE TABLE IF NOT EXISTS Files (
                    Id INTEGER PRIMARY KEY,
                    ArchiveId INTEGER,
                    FileName TEXT,
                    Overwritten INTEGER,
                    DateInstalled DATETIME,
                    FOREIGN KEY(ArchiveId) REFERENCES Archives(ArchiveId)
                );";

                string createRuntimesTableQuery = @"
                CREATE TABLE IF NOT EXISTS Runtimes (
                    Id INTEGER PRIMARY KEY,
                    RuntimeName TEXT,
                    Location TEXT,
                    UNIQUE(RuntimeName)
                );";

                using (SQLiteCommand command = new SQLiteCommand(createArchivesTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(createFilesTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(createRuntimesTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        private void InsertFileRecord(SQLiteConnection connection, int archiveId, string fileName, bool overWritten)
        {
            try
            {
                string selectQuery = "SELECT Id FROM Files WHERE ArchiveId = @ArchiveId AND FileName = @FileName;";
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@ArchiveId", archiveId);
                    selectCommand.Parameters.AddWithValue("@FileName", fileName);

                    object existingRecordId = selectCommand.ExecuteScalar();

                    if (existingRecordId != null)
                    {
                        // If a record with the same ArchiveId and FileName exists, update it
                        string updateQuery = "UPDATE Files SET DateInstalled = @DateInstalled WHERE Id = @Id;";
                        using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection))
                        {
                            // updateCommand.Parameters.AddWithValue("@Overwritten", overWritten ? 1 : 0);
                            updateCommand.Parameters.AddWithValue("@DateInstalled", DateTime.Now);
                            updateCommand.Parameters.AddWithValue("@Id", existingRecordId);

                            updateCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // If no existing record, insert a new one
                        string insertQuery = "INSERT INTO Files (ArchiveId, FileName, Overwritten, DateInstalled) VALUES (@ArchiveId, @FileName, @Overwritten, @DateInstalled);";
                        using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@ArchiveId", archiveId);
                            insertCommand.Parameters.AddWithValue("@FileName", fileName);
                            insertCommand.Parameters.AddWithValue("@Overwritten", overWritten ? 1 : 0);
                            insertCommand.Parameters.AddWithValue("@DateInstalled", DateTime.Now);

                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error inserting/updating record into the database: {ex.Message}");
            }
        }
        public static void AddRuntimeRecord(string shortname, string rtFolder)
        {
            string selectQuery = "SELECT COUNT(*) FROM Runtimes WHERE Location = @rtFolder";
            string insertQuery = "INSERT INTO Runtimes (RuntimeName, Location) VALUES (@shortname, @rtFolder)";
            string updateQuery = "UPDATE Runtimes SET RuntimeName = @shortname WHERE Location = @rtFolder";

            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
            {
                selectCommand.Parameters.AddWithValue("@rtFolder", rtFolder);
                int count = Convert.ToInt32(selectCommand.ExecuteScalar());

                if (count == 0)
                {
                    // Record doesn't exist, perform insert
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@shortname", shortname);
                        insertCommand.Parameters.AddWithValue("@rtFolder", rtFolder);
                        insertCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Record exists, perform update
                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@shortname", shortname);
                        updateCommand.Parameters.AddWithValue("@rtFolder", rtFolder);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }
        private void BxConsole_TextChanged(object sender, EventArgs e)
        {

        }
        private void BxListArchives_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void openZipDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        public void BxRuntimeFolder_TextChanged(object sender, EventArgs e)
        {
            newRuntimeFolder = BxRuntimeFolder.Text;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (newRuntimeFolder == "")
            {
                //Pop up empty runtime location message
            }
            if (newRTShortname == "")
            {
                //Pop up empty shortname message
            }
            else
            {
                AddRuntimeRecord(newRTShortname, newRuntimeFolder);
            }

            PopulateRTComboBox();
            PopulateListBox();
        }
        private void BtnRuntimeBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = RuntimeBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Get the selected folder path and update bxOutDir
                newRuntimeFolder = RuntimeBrowserDialog.SelectedPath;
                BxRuntimeFolder.Text = newRuntimeFolder;
            }
        }
        private void BxRuntimeShortName_TextChanged(object sender, EventArgs e)
        {
            newRTShortname = ((TextBox)sender).Text;
        }
        private void button3_Click(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void PopulateListBox()
        {
            try
            {
                string query = "SELECT RuntimeName, Location FROM Runtimes"; // Select 'RuntimeName' and 'Location' columns

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        ListBoxRT.Items.Clear(); // Clear existing items in the ListBoxRT

                        while (reader.Read())
                        {
                            // Assuming 'RuntimeName' is in the first column (index 0) and 'Location' is in the second column (index 1)
                            if (!reader.IsDBNull(0) && !reader.IsDBNull(1)) // Check if both values are not NULL
                            {
                                string runtimeName = reader.GetString(0);
                                string location = reader.GetString(1);
                                string formattedItem = $"{runtimeName}\t\t\t{location}";
                                ListBoxRT.Items.Add(formattedItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating ListBoxRT: {ex.Message}");
            }
        }
        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
        private void BtnRuntimeBrowse_Click_1(object sender, EventArgs e)
        {

        }
        private void BxRuntimeShortName_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void BxRuntimeFolder_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void BtnRuntimeBrowse_Click_2(object sender, EventArgs e)
        {

        }
        private void button1_Click_1(object sender, EventArgs e)
        {

        }
        private void CmboSelRuntime_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        private void BxMoveLocation_TextChanged(object sender, EventArgs e)
        {

        }
        private void BxShowRTPath_TextChanged(object sender, EventArgs e)
        {

        }
        private void button4_Click(object sender, EventArgs e)
        {
            PopulateRTComboBox();
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void ArchiveFolderDialog_HelpRequest(object sender, EventArgs e)
        {

        }
        private void ListBoxRT_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetSelectedRuntime();
            Log($"Runtime Selected: {selectedRuntime}");
        }
        private void GetSelectedRuntime()
        {
            if (ListBoxRT.SelectedItem != null)
            {
                string selectedValue = ListBoxRT.SelectedItem.ToString();
                string[] columns = selectedValue.Split('\t'); // Split the selected value by tab character
                if (columns.Length >= 2) // Ensure there are at least two columns
                {
                    string selectedColumn = columns[0]; // Select the second column (index 1)
                                                        // Use the selectedColumn as needed
                    Log("Selected Column: " + selectedColumn);
                    selectedRuntime = selectedColumn;
                }
                else
                {
                    // Handle case when selected value does not have two columns
                    Log("Selected value does not have two columns.");
                }
            }
            else
            {
                // Handle case when no item is selected
                Log("No item is selected.");
            }
        }
        private void RemoveRuntime(string selectedRuntime)
        {
            try
            {
                string query = "DELETE FROM Runtimes WHERE RuntimeName = @RuntimeName";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@RuntimeName", selectedRuntime);

                    // Execute the command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Log("Record deleted successfully.");
                    }
                    else
                    {
                        Log("No record found matching the selected runtime.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error deleting record: {ex.Message}");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            RemoveRuntime(selectedRuntime);
            PopulateListBox();
            Log($"Runtime {selectedRuntime} removed from list");
        }
        private void ChkShowLog_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkShowLog.Checked)
            {
                // Store the current location
                Point currentLocation = this.Location;

                // Resize the form to width 1037 and height 1037
                this.Size = new Size(1037, 1037);

                // Restore the current location
                this.Location = currentLocation;
            }
            else if (!ChkMoveArchives.Checked)
            {
                Point currentLocation = this.Location;

                // Resize the form to width 1037 and height 605
                this.Size = new Size(1037, 605);

                // Restore the current location
                this.Location = currentLocation;
            }
        }
        private void BxArchiveSearch_TextChanged(object sender, EventArgs e)
        {
            PopulateArchiveListbox();
        }
        private void PopulateArchiveListbox()
        {
            string searchQuery = BxArchiveSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                List<string> matchingArchives = SearchArchivesByName(searchQuery);

                // Clear existing items in the ListBox
                LstArchive.Items.Clear();

                // Populate the ListBox with the matching archives
                foreach (string archiveName in matchingArchives)
                {
                    LstArchive.Items.Add(archiveName);
                }
            }

        }
        private List<string> SearchArchivesByName(string searchTerm)
        {
            List<string> matchingArchives = new List<string>();

            try
            {
                string query = @"
            SELECT ArchiveName 
            FROM Archives 
            WHERE ArchiveName LIKE @SearchTerm";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    // Using parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string archiveName = reader.GetString(0);
                            matchingArchives.Add(archiveName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error searching archives by name: {ex.Message}");
            }

            return matchingArchives;
        }
        private List<string> AllArchivesByName()
        {
            List<string> allArchives = new List<string>();

            try
            {
                string query = @"
                SELECT ArchiveName 
                FROM Archives
                ORDER BY DateInstalled DESC;";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Retrieve the value of the field from each row and add it to the list
                            string value = reader.GetString(0); // Assuming the field is a string type
                            allArchives.Add(value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error searching archives : {ex.Message}");
            }

            return allArchives;
        }
        private void GetSafeFileList()
        {
            // Clear existing items in the list variable
            fileListForSelectedArchive.Clear();

            // Retrieve the selected archive name from the ListBox
            string selectedArchive = LstArchive.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedArchive))
            {
                fileListForSelectedArchive = GetSafeFileListForArchive(selectedArchive);
                archiveID = GetArchiveId(selectedArchive);

            }

        }
        private void GetAllFileList()
        {
            // Clear existing items in the list variable
            fileListForSelectedArchive.Clear();

            // Retrieve the selected archive name from the ListBox
            string selectedArchive = LstArchive.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedArchive))
            {
                fileListForSelectedArchive = GetFullFileListForArchive(selectedArchive);
                archiveID = GetArchiveId(selectedArchive);
            }
        }
        private List<string> GetSafeFileListForArchive(string archiveName)
        {
            List<string> fileList = new List<string>();

            try
            {
                string query = @"
            SELECT FileName 
            FROM Files 
            WHERE ArchiveId = (
                SELECT ArchiveId 
                FROM Archives 
                WHERE ArchiveName = @ArchiveName
            ) 
            AND Overwritten = 0";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    // Using parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@ArchiveName", archiveName);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fileName = reader.GetString(0);
                            fileList.Add(fileName);
                        }
                    }
                }
                ClearLog();
                fileListForSelectedArchive = fileList;
                //Log($"SAFE File list for {archiveName}:");
                //foreach (string fileName in fileListForSelectedArchive)
                //{
                //    Log($" {fileName}");
                //}
                //Log($"Files in List: {fileListForSelectedArchive.Count}");
            }
            catch (Exception ex)
            {
                Log($"Error retrieving file list for archive '{archiveName}': {ex.Message}");
            }

            return fileList;
        }
        private List<string> GetFullFileListForArchive(string archiveName)
        {
            List<string> fileList = new List<string>();

            try
            {
                string query = @"
            SELECT FileName 
            FROM Files 
            WHERE ArchiveId = (
                SELECT ArchiveId 
                FROM Archives 
                WHERE ArchiveName = @ArchiveName
            )";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    // Using parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@ArchiveName", archiveName);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fileName = reader.GetString(0);
                            fileList.Add(fileName);
                        }
                    }
                }
                fileListForSelectedArchive = fileList;
                ClearLog();
                //Log($"FULL File list for {archiveName}:");
                //foreach (string fileName in fileList)
                //{
                //    Log($" {fileName}");
                //}
                //Log($"Files in List: {fileListForSelectedArchive.Count}");
            }
            catch (Exception ex)
            {
                Log($"Error retrieving file list for archive '{archiveName}': {ex.Message}");
            }

            return fileList;
        }
        private void BtnShowAllFiles_Click(object sender, EventArgs e)
        {
            string selectedArchive = LstArchive.SelectedItem?.ToString();
            GetAllFileList();
            Log($"FULL File list for {selectedArchive}:");
            foreach (string fileName in fileListForSelectedArchive)
            {
                Log($" {fileName}");
            }
            Log($"Files in List: {fileListForSelectedArchive.Count}");
        }
        private void DeleteFiles(List<string> fileList)
        {
            foreach (string filePath in fileList)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Log($"Deleted file: {filePath}");
                    }
                    else
                    {
                        Log($"File does not exist: {filePath}");
                    }



                }
                catch (Exception ex)
                {
                    Log($"Error deleting file '{filePath}': {ex.Message}");
                }
            }

        }
        private void BtnClearFileList_Click(object sender, EventArgs e)
        {
            fileListForSelectedArchive.Clear();
            Log("File list Cleared!");
            Log($"Files in List: {fileListForSelectedArchive.Count}");
        }
        private void ClearLog()
        {
            BxConsole.Text = string.Empty;
        }
        private void BtnSafeUninstall_Click(object sender, EventArgs e)
        {
            GetSafeFileList();
            DialogResult result = MessageBox.Show($"You are about to remove {fileListForSelectedArchive.Count} files!\nAre you sure?", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
            {
                DeleteFiles(fileListForSelectedArchive);
                DelSafeFilesFromDB(archiveID);
                DelArchiveFromDB(archiveID);
                PopulateArchiveListbox();
                archiveID = 0;
                Log($"Archive ID = {archiveID}");

            }
            else if (result == DialogResult.Cancel)
            {
                return;
            }

        }
        private int GetArchiveId(string archiveName)
        {
            int archiveId = -1; // Default value if no record is found or an error occurs

            try
            {
                string query = "SELECT ArchiveId FROM Archives WHERE ArchiveName = @ArchiveName;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ArchiveName", archiveName);

                    // connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        archiveId = Convert.ToInt32(result);
                    }

                    // connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log($"Error retrieving ArchiveId: {ex.Message}");
            }

            return archiveId;
        }
        private void DelSafeFilesFromDB(int archiveID)
        {
            try
            {
                string query = "DELETE FROM Files WHERE ArchiveID = @ArchiveID AND Overwritten = 0";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@ArchiveID", archiveID);

                    // Execute the command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Log("Record deleted successfully.");
                    }
                    else
                    {
                        Log("No record found matching the selected runtime.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error deleting record: {ex.Message}");
            }
        }
        private void DelAllFilesFromDB(int archiveID)
        {
            try
            {
                string query = "DELETE FROM Files WHERE ArchiveID = @ArchiveID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@ArchiveID", archiveID);

                    // Execute the command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Log("Record deleted successfully.");
                    }
                    else
                    {
                        Log("No record found matching the selected runtime.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error deleting record: {ex.Message}");
            }
        }
        private void DelArchiveFromDB(int archiveID)
        {
            try
            {
                string query = "DELETE FROM Archives WHERE ArchiveID = @ArchiveID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@ArchiveID", archiveID);

                    // Execute the command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Log("Record deleted successfully.");
                    }
                    else
                    {
                        Log("No record found matching the selected runtime.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error deleting record: {ex.Message}");
            }
        }
        private void LstArchive_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            LstArchive.Items.Clear();
            DefArchiveList();
        }
    }

}

