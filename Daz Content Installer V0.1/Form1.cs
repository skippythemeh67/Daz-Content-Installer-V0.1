using SharpCompress.Archives;
using SharpCompress.Common;
using System.Data.SQLite;
using static System.Environment;


namespace Daz_Content_Installer_V0._1
{
    public partial class Main : Form
    {
        private static readonly List<string> specialFolders = new()
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
        private string? newRuntimeFolder;
        private string? newRTShortname;
        private static string? folderLocation;
        private static string? tempDirectory;
        private static string? inputArchive;
        private static readonly bool overWrite;
        private static string? inputFolder;
        private static string? destFolder;
        private List<string>? archiveFiles;
        private int archiveCount;
        private string? runtimeLocation;
        private string? moveFolder;
        private string? selectedRuntime;
        private List<string>? errorList;
        private List<string> fileListForSelectedArchive = new(); // Define a class-level variable to hold the file list
        private int archiveID;
        private bool specialFolderFound;
        private bool archiveError;
        List<string>? matchingFolders;


        private static SQLiteConnection? connection;

        public Main()
        {
            InitializeComponent();
            Load += MainForm_Load; // Subscribe to the Load event
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Call the DB init method for initialization
            InitDatabase();
            CheckAndSwitchTabRuntimeEmpty(connection);
            DefArchiveList();


            errorList = new List<string>();
            archiveCount = 0;

            // Unsubscribe from the Load event to ensure it runs only once
            Load -= MainForm_Load;
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
                _ = BxConsole.BeginInvoke(new Action<string>(Log), message);
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
                _ = LstArchive.Items.Add(archiveName);
            }
        }
        private void CheckAndSwitchTabRuntimeEmpty(SQLiteConnection connection)
        {
            string tableName = "Runtimes";
            string query = $"SELECT COUNT(*) FROM {tableName}";

            using SQLiteCommand command = new(query, connection);
            int rowCount = Convert.ToInt32(command.ExecuteScalar());

            if (rowCount == 0)
            {
                // Show warning message to the user
                _ = MessageBox.Show("No Runtimes configured!\n\nSwitching to the Runtime Management tab on launch.\n\nPlease add at least one Runtime.\n\n(This is normal if this is the first time you have run this application.)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Switch to a specific tab (assuming yourTabControl is the name of your TabControl)
                Tab.SelectTab(tabPage4); // Specify your desired tab page
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


            OpenZipFileDialog.Filter = "Archive Files|*.zip;*.rar";
            OpenZipFileDialog.Multiselect = true;

            DialogResult result = OpenZipFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Get the selected file names and update zipFile

                string[] selectedFiles = OpenZipFileDialog.FileNames;
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

        private void BtnInstallContent_Click(object sender, EventArgs e)
        {
            if (archiveFiles == null)
            {
                MessageBox.Show("No Archives Selected!\n\nSelect at least one archive to install to continue.)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(BxShowRTPath.Text))
            {
                MessageBox.Show("No Runtime Selected!\n\nSelect a runtime to continue.)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else InstallArchive();
        }
        private async void InstallArchive()
        {
            errorList.Clear();
            BtnInstallContent.Enabled = false;

            await Task.Run(() =>
            {
                int currentIteration = 0;

                foreach (string archiveFile in archiveFiles)
                {
                    ProcessFolder(archiveFile, inputFolder, destFolder);
                    // Move the file
                    if (ChkMoveArchives.Checked)
                    {
                        if (!archiveError)
                        {
                            if (specialFolderFound)
                            {
                                string destinationFilePath = Path.Combine(moveFolder, Path.GetFileName(archiveFile));

                                File.Move(archiveFile, destinationFilePath);
                                Log($"{archiveFile} moved to {destinationFilePath}");
                            }
                        }
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
            Log(" ");
            int errorCount = errorList.Count;
            Log($"Errors:\t{errorCount}");
            foreach (var error in errorList)
            {
                Log($"  {error}\n");
            }

        }
        // Method to update the progress bar value safely from any thread
        private void UpdateProgressBar(int value)
        {
            // Ensure that the value is within the valid range of the progress bar
            int safeValue = Math.Max(ProgBar.Minimum, Math.Min(ProgBar.Maximum, value));

            if (ProgBar.InvokeRequired)
            {
                _ = ProgBar.Invoke((MethodInvoker)(() => ProgBar.Value = safeValue));
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
                _ = ProgBar.Invoke((MethodInvoker)(() => ProgBar.Value = 0));
            }
            else
            {
                ProgBar.Value = 0;
            }
        }
        private void BtnShowFiles_Click(object sender, EventArgs e)
        {
            string? selectedArchive = LstArchive.SelectedItem?.ToString();
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
                string? selectedRuntime = CmboSelRuntime.SelectedItem?.ToString();

                if (selectedRuntime != null)
                {
                    // SQL query to select the Location from Runtimes table based on the selected runtime name
                    string query = "SELECT Location FROM Runtimes WHERE RuntimeName = @RuntimeName";

                    using SQLiteCommand command = new(query, connection);
                    _ = command.Parameters.AddWithValue("@RuntimeName", selectedRuntime);
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

                using SQLiteCommand command = new(query, connection);
                using SQLiteDataReader reader = command.ExecuteReader();
                // Clear existing items in the ComboBox
                CmboSelRuntime.Items.Clear();

                // Iterate through the results and add each RuntimeName value to the ComboBox
                while (reader.Read())
                {
                    // Assuming RuntimeName is a string
                    string runtimeName = reader.GetString(0);
                    _ = CmboSelRuntime.Items.Add(runtimeName);
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
                _ = Directory.CreateDirectory(outputFolder);

                int archiveId = AddArchive(Path.GetFileNameWithoutExtension(archiveName), connection);

                ExtractFiles(archiveName, outputFolder);
                inputArchive = Path.GetFileName(archiveName);

                SearchSpecialFolders(outputFolder);
                if (!specialFolderFound)
                {
                    errorList.Add("No Daz content folders found: " + archiveName);
                }

                else
                {
                    Log($"Output Folder: {outputFolder}");
                    Log($"Extraction completed successfully for '{archiveName}'.");
                    Log($"Special folder found at: {folderLocation}");

                    List<string> contents = GetFolderContents(matchingFolders);
                    Log("Contents of the Daz Folders:");
                    foreach (string item in contents)
                    {
                        Log(item);
                    }

                    MoveContentsToInput(contents, archiveId, outFolder);

                    Log($"Move completed successfully for '{archiveName}'.");
                    archiveError = false;
                }
            }
            catch (Exception ex)
            {
                Log($"Error processing '{archiveName}': {ex.Message}");
                archiveError = true;
            }
        }
        static int AddArchive(string archiveName, SQLiteConnection connection)
        {
            string insertOrUpdateQuery = @"
        INSERT OR IGNORE INTO Archives (ArchiveName, DateInstalled) VALUES (@ArchiveName, @DateInstalled);
        UPDATE Archives SET DateInstalled = @DateInstalled WHERE ArchiveName = @ArchiveName;
        SELECT ArchiveId FROM Archives WHERE ArchiveName = @ArchiveName;";

            using SQLiteCommand command = new(insertOrUpdateQuery, connection);
            _ = command.Parameters.AddWithValue("@ArchiveName", archiveName);
            _ = command.Parameters.AddWithValue("@DateInstalled", DateTime.Now);

            return Convert.ToInt32(command.ExecuteScalar());
        }
        static List<string> GetArchiveFiles(string folderPath)
        {
            string[] searchPatterns = new[] { "*.zip", "*.rar" };
            List<string> archiveFiles = searchPatterns
                .SelectMany(pattern => Directory.GetFiles(folderPath, pattern))
                .ToList();

            return archiveFiles;
        }
        public void ExtractFiles(string inputFilePath, string outputFolderPath)
        {
            try
            {
                using IArchive archive = ArchiveFactory.Open(inputFilePath);
                foreach (IArchiveEntry entry in archive.Entries)
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

                            _ = Directory.CreateDirectory(nestedOutputFolder);

                            ExtractFiles(nestedArchivePath, nestedOutputFolder);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch any exceptions and add the error message to the error list
                errorList.Add(inputFilePath + " " + ex.Message);
                //errorList.Add(ex.Message);
            }
        }
        static bool IsArchiveFile(string fileName)
        {
            string[] archiveExtensions = { ".zip", ".rar", ".7z" };
            string extension = Path.GetExtension(fileName).ToLower();
            return archiveExtensions.Any(ext => ext == extension);
        }
        public void SearchSpecialFolders(string currentFolderPath)
        {
            foreach (string specialFolder in specialFolders)
            {
                matchingFolders = new List<string>(Directory.GetDirectories(currentFolderPath, specialFolder, SearchOption.AllDirectories));

                if (matchingFolders.Count > 0)
                {
                    folderLocation = Path.GetDirectoryName(matchingFolders[0]);
                    Log($"Daz folder found at: {folderLocation}");
                    specialFolderFound = true;
                    return;
                }
                else specialFolderFound = false;
                
            }

        }
        static List<string> GetFolderContents(List<string> folderPath)
        {
            List<string> contents = new();

            foreach (string folder in folderPath)
                if (Directory.Exists(folder))
                {
                contents.AddRange(Directory.GetFiles(Path.GetDirectoryName(folder)));
                contents.AddRange(Directory.GetDirectories(Path.GetDirectoryName(folder)));
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
                    _ = Directory.CreateDirectory(destinationPath);
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

            using SQLiteConnection connection = new($"Data Source=InstallerFiles.db;Version=3;");
            connection.Open();

            foreach (FileInfo fi in source.GetFiles())
            {

                string namedFile = Path.Combine(target.FullName, fi.Name);
                bool overWrite = File.Exists(namedFile);
                Log($"Copying {fi.Name} to {target.FullName}");

                _ = fi.CopyTo(namedFile, true);

                InsertFileRecord(connection, archiveId, namedFile, overWrite);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, archiveId);
            }
        }
        public void DeleteFolderContents(string folderPath)
        {
            try
            {
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    File.Delete(file);
                }

                foreach (string subdirectory in Directory.GetDirectories(folderPath))
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
        static void InitDatabase()
        {
            string databasePath = "InstallerFiles.db";

            if (!File.Exists(databasePath))
            {
                CreateDatabase(databasePath);
            }

            connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
            connection.Open();
        }
        static void CreateDatabase(string path)
        {
            SQLiteConnection.CreateFile(path);

            using SQLiteConnection connection = new($"Data Source={path};Version=3;");
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

            using (SQLiteCommand command = new(createArchivesTableQuery, connection))
            {
                _ = command.ExecuteNonQuery();
            }

            using (SQLiteCommand command = new(createFilesTableQuery, connection))
            {
                _ = command.ExecuteNonQuery();
            }

            using (SQLiteCommand command = new(createRuntimesTableQuery, connection))
            {
                _ = command.ExecuteNonQuery();
            }
        }
        private void InsertFileRecord(SQLiteConnection connection, int archiveId, string fileName, bool overWritten)
        {
            try
            {
                string selectQuery = "SELECT Id FROM Files WHERE ArchiveId = @ArchiveId AND FileName = @FileName;";
                using SQLiteCommand selectCommand = new(selectQuery, connection);
                _ = selectCommand.Parameters.AddWithValue("@ArchiveId", archiveId);
                _ = selectCommand.Parameters.AddWithValue("@FileName", fileName);

                object existingRecordId = selectCommand.ExecuteScalar();

                if (existingRecordId != null)
                {
                    // If a record with the same ArchiveId and FileName exists, update it
                    string updateQuery = "UPDATE Files SET DateInstalled = @DateInstalled WHERE Id = @Id;";
                    using SQLiteCommand updateCommand = new(updateQuery, connection);
                    // updateCommand.Parameters.AddWithValue("@Overwritten", overWritten ? 1 : 0);
                    _ = updateCommand.Parameters.AddWithValue("@DateInstalled", DateTime.Now);
                    _ = updateCommand.Parameters.AddWithValue("@Id", existingRecordId);

                    _ = updateCommand.ExecuteNonQuery();
                }
                else
                {
                    // If no existing record, insert a new one
                    string insertQuery = "INSERT INTO Files (ArchiveId, FileName, Overwritten, DateInstalled) VALUES (@ArchiveId, @FileName, @Overwritten, @DateInstalled);";
                    using SQLiteCommand insertCommand = new(insertQuery, connection);
                    _ = insertCommand.Parameters.AddWithValue("@ArchiveId", archiveId);
                    _ = insertCommand.Parameters.AddWithValue("@FileName", fileName);
                    _ = insertCommand.Parameters.AddWithValue("@Overwritten", overWritten ? 1 : 0);
                    _ = insertCommand.Parameters.AddWithValue("@DateInstalled", DateTime.Now);

                    _ = insertCommand.ExecuteNonQuery();
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

            using SQLiteCommand selectCommand = new(selectQuery, connection);
            _ = selectCommand.Parameters.AddWithValue("@rtFolder", rtFolder);
            int count = Convert.ToInt32(selectCommand.ExecuteScalar());

            if (count == 0)
            {
                // Record doesn't exist, perform insert
                using SQLiteCommand insertCommand = new(insertQuery, connection);
                _ = insertCommand.Parameters.AddWithValue("@shortname", shortname);
                _ = insertCommand.Parameters.AddWithValue("@rtFolder", rtFolder);
                _ = insertCommand.ExecuteNonQuery();
            }
            else
            {
                // Record exists, perform update
                using SQLiteCommand updateCommand = new(updateQuery, connection);
                _ = updateCommand.Parameters.AddWithValue("@shortname", shortname);
                _ = updateCommand.Parameters.AddWithValue("@rtFolder", rtFolder);
                _ = updateCommand.ExecuteNonQuery();
            }
        }
        private void BxConsole_TextChanged(object sender, EventArgs e)
        {

        }
        private void BxListArchives_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void OpenZipDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        public void BxRuntimeFolder_TextChanged(object sender, EventArgs e)
        {
            newRuntimeFolder = BxRuntimeFolder.Text;
        }
        private void BtnAddRuntime_Click(object sender, EventArgs e)
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
        private void PopulateListBox()
        {
            try
            {
                string query = "SELECT RuntimeName, Location FROM Runtimes"; // Select 'RuntimeName' and 'Location' columns

                using SQLiteCommand command = new(query, connection);
                using SQLiteDataReader reader = command.ExecuteReader();
                ListBoxRT.Items.Clear(); // Clear existing items in the ListBoxRT

                while (reader.Read())
                {
                    // Assuming 'RuntimeName' is in the first column (index 0) and 'Location' is in the second column (index 1)
                    if (!reader.IsDBNull(0) && !reader.IsDBNull(1)) // Check if both values are not NULL
                    {
                        string runtimeName = reader.GetString(0);
                        string location = reader.GetString(1);
                        string formattedItem = $"{runtimeName}\t\t\t{location}";
                        _ = ListBoxRT.Items.Add(formattedItem);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error populating ListBoxRT: {ex.Message}");
            }
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
        private void CmboSelRuntime_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        private void BxMoveLocation_TextChanged(object sender, EventArgs e)
        {

        }
        private void BxShowRTPath_TextChanged(object sender, EventArgs e)
        {

        }
        private void TabPage1_Click(object sender, EventArgs e)
        {

        }
        private void ProgressBar1_Click(object sender, EventArgs e)
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

                using SQLiteCommand command = new(query, connection);
                // Add parameters to the command
                _ = command.Parameters.AddWithValue("@RuntimeName", selectedRuntime);

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
            catch (Exception ex)
            {
                Log($"Error deleting record: {ex.Message}");
            }
        }
        private void BtnRemoveRT_Click(object sender, EventArgs e)
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
                Point currentLocation = Location;

                // Resize the form to width 1037 and height 1037
                Size = new Size(1037, 1037);

                // Restore the current location
                Location = currentLocation;
            }
            else if (!ChkMoveArchives.Checked)
            {
                Point currentLocation = Location;

                // Resize the form to width 1037 and height 605
                Size = new Size(1037, 605);

                // Restore the current location
                Location = currentLocation;
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
                    _ = LstArchive.Items.Add(archiveName);
                }
            }

        }
        private List<string> SearchArchivesByName(string searchTerm)
        {
            List<string> matchingArchives = new();

            try
            {
                string query = @"
            SELECT ArchiveName 
            FROM Archives 
            WHERE ArchiveName LIKE @SearchTerm";

                using SQLiteCommand command = new(query, connection);
                // Using parameters to prevent SQL injection
                _ = command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                using SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string archiveName = reader.GetString(0);
                    matchingArchives.Add(archiveName);
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
            List<string> allArchives = new();

            try
            {
                string query = @"
                SELECT ArchiveName 
                FROM Archives
                ORDER BY DateInstalled DESC;";
                using SQLiteCommand command = new(query, connection);
                using SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Retrieve the value of the field from each row and add it to the list
                    string value = reader.GetString(0); // Assuming the field is a string type
                    allArchives.Add(value);
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
            string? selectedArchive = LstArchive.SelectedItem?.ToString();

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
            string? selectedArchive = LstArchive.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedArchive))
            {
                fileListForSelectedArchive = GetFullFileListForArchive(selectedArchive);
                archiveID = GetArchiveId(selectedArchive);
            }
        }
        private List<string> GetSafeFileListForArchive(string archiveName)
        {
            List<string> fileList = new();

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

                using (SQLiteCommand command = new(query, connection))
                {
                    // Using parameters to prevent SQL injection
                    _ = command.Parameters.AddWithValue("@ArchiveName", archiveName);

                    using SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string fileName = reader.GetString(0);
                        fileList.Add(fileName);
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
            List<string> fileList = new();

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

                using (SQLiteCommand command = new(query, connection))
                {
                    // Using parameters to prevent SQL injection
                    _ = command.Parameters.AddWithValue("@ArchiveName", archiveName);

                    using SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string fileName = reader.GetString(0);
                        fileList.Add(fileName);
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
            string? selectedArchive = LstArchive.SelectedItem?.ToString();
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

                using SQLiteCommand command = new(query, connection);
                _ = command.Parameters.AddWithValue("@ArchiveName", archiveName);

                // connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    archiveId = Convert.ToInt32(result);
                }

                // connection.Close();
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

                using SQLiteCommand command = new(query, connection);
                // Add parameters to the command
                _ = command.Parameters.AddWithValue("@ArchiveID", archiveID);

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

                using SQLiteCommand command = new(query, connection);
                // Add parameters to the command
                _ = command.Parameters.AddWithValue("@ArchiveID", archiveID);

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

                using SQLiteCommand command = new(query, connection);
                // Add parameters to the command
                _ = command.Parameters.AddWithValue("@ArchiveID", archiveID);

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