using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit;
using MailMerge.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using static MailMerge.App;
using DevExpress.XtraRichEdit.Export;
using DevExpress.Maui.Core.Internal;

namespace MailMerge.ViewModel {
    public class MainViewModel : BindableBase {
        ObservableCollection<Models> contacts;
        ObservableCollection<EmailTemplate> emailTemplates;
        Models mailToCustomer;
        bool isTemplatesMenuVisible;
        string currentUserName = "Alex";
        public MainViewModel() {
            LoadDataAsync();
            EmailTemplates = new ObservableCollection<EmailTemplate>() {
                new EmailTemplate("Party Invitation","Party Invitation.docx"),
                new EmailTemplate("Financing Package ","Financing Package .docx"),
                new EmailTemplate("Northwind University","Northwind University.docx"),
            };
            ShowTemplatesMenuCommand = new Command<Models>(ShowTemplatesMenu);
            SendTemplateEmailCommand = new Command<EmailTemplate>(SendTemplateEmail);
            CopyTemplatesAppData();
        }

        async void SendTemplateEmail(EmailTemplate template) {
            RichEditDocumentServer resultDocumentProcessor = await MailMergeAsync(template);
            IsTemplatesMenuVisible = false;
            await SendOrShareResultAsync(template, resultDocumentProcessor);
        }
        async Task<RichEditDocumentServer> MailMergeAsync(EmailTemplate emailTemplate) {
            string workingFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, emailTemplate.DocumentSourcePath);
            RichEditDocumentServer mergeRichProcessor = new RichEditDocumentServer();
            await mergeRichProcessor.LoadDocumentAsync(workingFilePath);

            mergeRichProcessor.Document.Fields[0].Locked = true;
            mergeRichProcessor.Options.MailMerge.DataSource = new List<object> { new { RecipientName = mailToCustomer.FirstName, SenderName = currentUserName} };
            MailMergeOptions myMergeOptions = mergeRichProcessor.Document.CreateMailMergeOptions();

            RichEditDocumentServer resultDocumentProcessor = new RichEditDocumentServer();
            resultDocumentProcessor.CreateNewDocument();
            myMergeOptions.MergeMode = MergeMode.NewSection;
            mergeRichProcessor.MailMerge(resultDocumentProcessor.Document);
            return resultDocumentProcessor;
        }
        void ShowTemplatesMenu(Models customer) {
            mailToCustomer = customer;
            IsTemplatesMenuVisible = true;
        }
        async Task SendOrShareResultAsync(EmailTemplate emailTemplate, RichEditDocumentServer richProcessor) {
            if (Email.Default.IsComposeSupported) {
                richProcessor.BeforeExport += OnBeforeExport;
                string htmlEmailContent = richProcessor.Document.GetHtmlText(richProcessor.Document.Range, null);
                richProcessor.BeforeExport -= OnBeforeExport;
                var message = new EmailMessage {
                    Subject = emailTemplate.Name,
                    Body = htmlEmailContent,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = new List<string>() { mailToCustomer.Email }
                };
                try {
                    await Email.Default.ComposeAsync(message);
                }
                catch (Exception e){
                    await App.Current.MainPage.DisplayAlert("Error", "Make sure your mail client is configured", "OK");
                }
            }
            else {
                string fileToSharePath = Path.Combine(FileSystem.Current.AppDataDirectory, $"{emailTemplate.Name} Result.docx");
                await App.Current.MainPage.DisplayAlert("Can't send the email", "Email composing is not supported on this device. You can send the document with merged text to another device", "OK");
                await richProcessor.SaveDocumentAsync(fileToSharePath, DocumentFormat.OpenXml);
                await Share.Default.RequestAsync(new ShareFileRequest {
                    Title = "Share the file",
                    File = new ShareFile(fileToSharePath)
                });
            }
        }
        private void OnBeforeExport(object sender, BeforeExportEventArgs e) {
            HtmlDocumentExporterOptions options = e.Options as HtmlDocumentExporterOptions;
            if (options != null) {
                options.Encoding = Encoding.UTF8;
            }
        }
        async void CopyTemplatesAppData() {
            foreach (var template in EmailTemplates) {
                await CopyWorkingFilesToAppData(template.DocumentSourcePath);
            }
        }
        public async Task CopyWorkingFilesToAppData(string fileName) {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
            using FileStream outputStream = File.OpenWrite(targetFile);
            fileStream.CopyTo(outputStream);
        }
        public async void LoadDataAsync() {
            var retrievedItems = await DBContactService.Instance.GetItemsAsync();
            Contacts = new ObservableCollection<Models>(retrievedItems);
        }
        public ICommand SendTemplateEmailCommand {
            get;
            set;
        }
        public ICommand ShowTemplatesMenuCommand {
            get;
            set;
        }
        public ObservableCollection<Models> Contacts {
            get { return contacts; }
            set {
                contacts = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<EmailTemplate> EmailTemplates {
            get { return emailTemplates; }
            set {
                emailTemplates = value;
                RaisePropertyChanged();
            }
        }
        public bool IsTemplatesMenuVisible {
            get { return isTemplatesMenuVisible; }
            set {
                isTemplatesMenuVisible = value;
                RaisePropertyChanged();
            }
        }
    }
}
