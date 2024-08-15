using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

namespace MauiOFAMerge {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
        }
        private async void OnLoaded(object sender, EventArgs e) {
            await InitFilesAsync("Party Invitation.docx");
        }
        async Task InitFilesAsync(string fileName) {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
            using FileStream outputStream = File.OpenWrite(targetFile);
            fileStream.CopyTo(outputStream);
        }
    }

    public partial class MergeInfoViewModel : ObservableObject {
        [ObservableProperty]
        string publicName = "Alice";

        [RelayCommand]
        async Task MergeAsync() {
            RichEditDocumentServer mergeProcessor = await RichProcFromFileAsync("Party Invitation.docx");
            AssignSource(mergeProcessor);
            mergeProcessor = MergeToNewDocument(mergeProcessor);
            string docPath = await SaveToFile(mergeProcessor, "ResultingDoc.docx");
            await ShareDocAsync(docPath);
        }
        async Task<RichEditDocumentServer> RichProcFromFileAsync(string fileName) {
            string workingFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
            RichEditDocumentServer mergeRichProcessor = new RichEditDocumentServer();
            await mergeRichProcessor.LoadDocumentAsync(workingFilePath);
            return mergeRichProcessor;
        }
        void AssignSource(RichEditDocumentServer mergeProcessor) {
            mergeProcessor.Document.Fields[0].Locked = true;
            mergeProcessor.Options.MailMerge.DataSource = new List<object> { new {
                RecipientName = this.PublicName,
                SenderCompany = "DX_Company" } };
        }
        RichEditDocumentServer MergeToNewDocument(RichEditDocumentServer existingDoc) {
            RichEditDocumentServer resultDocumentProcessor = new RichEditDocumentServer();
            resultDocumentProcessor.CreateNewDocument();
            MailMergeOptions myMergeOptions = existingDoc.Document.CreateMailMergeOptions();
            myMergeOptions.MergeMode = MergeMode.NewSection;
            existingDoc.MailMerge(resultDocumentProcessor.Document);
            return resultDocumentProcessor;
        }
        async Task<string> SaveToFile(RichEditDocumentServer mergeProcessor, string fileName) {
            string fileToSavePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
            await mergeProcessor.SaveDocumentAsync(fileToSavePath, DocumentFormat.OpenXml);
            return fileToSavePath;
        }
        async Task ShareDocAsync(string docPath) {
            await Share.Default.RequestAsync(new ShareFileRequest {
                Title = "Share the file",
                File = new ShareFile(docPath)
            });
        }
    }
}