using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.Data.SQLite;
using System.Linq;

class Program
{
    static List<string> specialFolders = new List<string> 
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
    static string folderLocation;
    static string tempDirectory;
    static string inputArchive;
    static bool overWrite;
    static SQLiteConnection connection;

    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ExtractFiles.exe <inputFolder> <outputFolder>");
            return;
        }

        string inputFolder = args[0];
        string destFolder = args[1];

        if (!Directory.Exists(inputFolder))
        {
            Console.WriteLine($"Error: Input folder '{inputFolder}' not found.");
            return;
        }

        try
        {
            InitDatabase();

            tempDirectory = Path.Combine(inputFolder, "temp");
            Console.WriteLine($"{tempDirectory}");

            Console.WriteLine("Getting list of archives to process...");
            var archiveFiles = GetArchiveFiles(inputFolder);

            foreach (var archiveFile in archiveFiles)
            {
                ProcessFolder(archiveFile, inputFolder, destFolder);
            }

            Console.WriteLine("Processing completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void ProcessFolder(string archiveName, string inputFolder, string outFolder)
    {
        try
        {
            Console.WriteLine($"Processing Archive: {archiveName}");
            string outputFolder = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(archiveName));
            Directory.CreateDirectory(outputFolder);

            int archiveId = AddArchive(Path.GetFileNameWithoutExtension(archiveName), connection);

            ExtractFiles(archiveName, outputFolder);
            inputArchive = Path.GetFileName(archiveName);

            SearchSpecialFolders(outputFolder);

            Console.WriteLine($"Output Folder: {outputFolder}");
            Console.WriteLine($"Extraction completed successfully for '{archiveName}'.");
            Console.WriteLine($"Special folder found at: {folderLocation}");

            List<string> contents = GetFolderContents(folderLocation);
            Console.WriteLine("Contents of the special folder:");
            foreach (var item in contents)
            {
                Console.WriteLine(item);
            }

            MoveContentsToInput(contents, archiveId, outFolder);

            Console.WriteLine($"Move completed successfully for '{archiveName}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing '{archiveName}': {ex.Message}");
        }
    }


    static int AddArchive(string archiveName, SQLiteConnection connection)
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


    static List<string> GetArchiveFiles(string folderPath)
    {
        var searchPatterns = new[] { "*.zip", "*.rar" };
        var archiveFiles = searchPatterns
            .SelectMany(pattern => Directory.GetFiles(folderPath, pattern))
            .ToList();

        return archiveFiles;
    }

    static void ExtractFiles(string inputFilePath, string outputFolderPath)
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

    static bool IsArchiveFile(string fileName)
    {
        string[] archiveExtensions = { ".zip", ".rar", ".7z" };
        string extension = Path.GetExtension(fileName).ToLower();
        return archiveExtensions.Any(ext => ext == extension);
    }

    static void SearchSpecialFolders(string currentFolderPath)
    {
        foreach (string specialFolder in specialFolders)
        {
            string[] matchingFolders = Directory.GetDirectories(currentFolderPath, specialFolder, SearchOption.AllDirectories);

            if (matchingFolders.Length > 0)
            {
                folderLocation = Path.GetDirectoryName(matchingFolders[0]);
                Console.WriteLine($"Debug: Special folder found at: {folderLocation}");
                return;
            }
        }
    }

    static List<string> GetFolderContents(string folderPath)
    {
        List<string> contents = new List<string>();

        if (Directory.Exists(folderPath))
        {
            contents.AddRange(Directory.GetFiles(folderPath));
            contents.AddRange(Directory.GetDirectories(folderPath));
        }

        return contents;
    }

    static void MoveContentsToInput(List<string> paths, int archiveId, string outputFolder)
    {
        foreach (string path in paths)
        {
            string destinationPath = Path.Combine(outputFolder, Path.GetFileName(path));

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            CopyAll(new DirectoryInfo(path), new DirectoryInfo(destinationPath), archiveId);

            Console.WriteLine($"Merged contents of Folder: {path}");
        }

        if (Directory.Exists(tempDirectory))
        {
            DeleteFolderContents(tempDirectory);
        }
        else
        {
            Console.WriteLine("Temporary folder does not exist.");
        }
    }

    public static void CopyAll(DirectoryInfo source, DirectoryInfo target, int archiveId)
    {
        if (source.FullName.ToLower() == target.FullName.ToLower())
        {
            return;
        }

        using (SQLiteConnection connection = new SQLiteConnection($"Data Source=InstalledFiles.db;Version=3;"))
        {
            connection.Open();

            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                string namedFile = Path.Combine(target.FullName, fi.Name);
                bool overWrite = File.Exists(namedFile);

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

    static void DeleteFolderContents(string folderPath)
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

            Console.WriteLine("Contents of the folder deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting folder contents: {ex.Message}");
        }
    }

    static void InitDatabase()
    {
        string databasePath = "InstalledFiles.db";

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

            using (SQLiteCommand command = new SQLiteCommand(createArchivesTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            using (SQLiteCommand command = new SQLiteCommand(createFilesTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }


    static void InsertFileRecord(SQLiteConnection connection, int archiveId, string fileName, bool overWritten)
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
            Console.WriteLine($"Error inserting/updating record into the database: {ex.Message}");
        }
    }

}
