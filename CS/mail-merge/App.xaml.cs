using MailMerge.Model;
using SQLite;

namespace MailMerge;

public partial class App : Application {
    public App() {
        InitializeComponent();
        DBContactService.Instance = new DBContactService("contacts.db");
        MainPage = new AppShell();
    }

    public class DBContactService {
        string DbWorkingPath;
        string dbFileName;
        bool isWorkingFolderInitialized;
        public static DBContactService Instance {
            get;
            set;
        }
        public DBContactService(string dbFileName) {
            DbWorkingPath = Path.Combine(FileSystem.Current.AppDataDirectory, dbFileName);
            this.dbFileName = dbFileName;
        }
        public async Task<IEnumerable<Models>> GetItemsAsync() {
            if (!isWorkingFolderInitialized)
                await InitWorkingFolderAsync();
            var conn = CreateConnection();
            return await Task.Run(() => conn.Table<Models>().ToList());
        }
        SQLiteConnection CreateConnection() {
            return new SQLiteConnection(DbWorkingPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.ProtectionNone | SQLite.SQLiteOpenFlags.SharedCache | SQLiteOpenFlags.FullMutex);
        }

        async Task InitWorkingFolderAsync() {
            if (!File.Exists(DbWorkingPath)) {
                using (Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(dbFileName)) {
                    using (FileStream outputStream = File.OpenWrite(DbWorkingPath)) {
                        fileStream.CopyTo(outputStream);
                    }
                }
            }
            isWorkingFolderInitialized = true;
        }
    }
}