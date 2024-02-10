using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.Data.SQLite;
using System.Linq;

public class Dz
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

    private static SQLiteConnection connection;

    public static void ProcessFolder(string archiveName, string inputFolder, string outFolder)
    {
        try
        {
            string tempDirectory = Path.Combine(inputFolder, "temp");
            Directory.CreateDirectory(tempDirectory);

            InitDatabase();

            int archiveId = AddArchive(Path.GetFileNameWithoutExtension(archiveName), connection);

            string outputFolder = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(archiveName));
            Directory.CreateDirectory(outputFolder);

            ExtractFiles(Path.Combine(inputFolder, archiveName), outputFolder);

            string folderLocation = SearchSpecialFolders(outputFolder);

            List<string> contents = GetFolderContents(folderLocation);

            MoveContentsToInput(contents, archiveId, outFolder, tempDirectory);

            DeleteFolderContents(tempDirectory);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing '{archiveName}': {ex.Message}");
        }
    }

    public static void InitDatabase()
    {
        string databasePath = "InstalledFiles.db";

        if (!File.Exists(databasePath))
        {
            CreateDatabase(databasePath);
        }

        connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
        connection.Open();
    }

    public static void CreateDatabase(string path)
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
                Location TEXT
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

    public static int AddArchive(string archiveName, SQLiteConnection connection)
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

    public static void ExtractFiles(string inputFilePath, string outputFolderPath)
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
                }
            }
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

    public static string SearchSpecialFolders(string currentFolderPath)
    {
        foreach (string specialFolder in specialFolders)
        {
            string[] matchingFolders = Directory.GetDirectories(currentFolderPath, specialFolder, SearchOption.AllDirectories);

            if (matchingFolders.Length > 0)
            {
                return Path.GetDirectoryName(matchingFolders[0]);
            }
        }

        return null;
    }

    public static List<string> GetFolderContents(string folderPath)
    {
        List<string> contents = new List<string>();

        if (Directory.Exists(folderPath))
        {
            contents.AddRange(Directory.GetFiles(folderPath));
            contents.AddRange(Directory.GetDirectories(folderPath));
        }

        return contents;
    }

    public static void MoveContentsToInput(List<string> paths, int archiveId, string outputFolder, string tempDirectory)
    {
        foreach (string path in paths)
        {
            string destinationPath = Path.Combine(outputFolder, Path.GetFileName(path));

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            CopyAll(new DirectoryInfo(path), new DirectoryInfo(destinationPath), archiveId);
        }
    }

    public static void CopyAll(DirectoryInfo source, DirectoryInfo target, int archiveId)
    {
        if (source.FullName.ToLower() == target.FullName.ToLower())
        {
            return;
        }

        foreach (FileInfo fi in source.GetFiles())
        {
            string namedFile = Path.Combine(target.FullName, fi.Name);
            bool overWrite = File.Exists(namedFile);

            fi.CopyTo(namedFile, true);

            InsertFileRecord(archiveId, namedFile, overWrite);
        }

        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir, archiveId);
        }
    }

    public static void InsertFileRecord(int archiveId, string fileName, bool overWritten)
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
                    string updateQuery = "UPDATE Files SET DateInstalled = @DateInstalled WHERE Id = @Id;";
                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@DateInstalled", DateTime.Now);
                        updateCommand.Parameters.AddWithValue("@Id", existingRecordId);

                        updateCommand.ExecuteNonQuery();
                    }
                }
                else
                {
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

    public static void DeleteFolderContents(string folderPath)
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting folder contents: {ex.Message}");
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

    
}


    
