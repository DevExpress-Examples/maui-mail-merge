# Mail Merge

This example uses Mail Merge to send emails. Data placeholders in an email template obtain their values from database records. Mail Merge functionality is included in our [Office File API](https://www.devexpress.com/products/net/office-file-api/) subscription.

## Requirements

* Register the DevExpress NuGet Gallery in Visual Studio to restore the NuGet packages used in this solution. See the following topic for more information: [Get Started with DevExpress Mobile UI for .NET MAUI](https://docs.devexpress.com/MAUI/403249/get-started).

   You can also refer to the following YouTube video for more information on how to get started with the DevExpress .NET MAUI Controls: [Setting up a .NET MAUI Project](https://www.youtube.com/watch?v=juJvl5UicIQ).

* To run this example, you need a DevExpress [Universal](https://www.devexpress.com/subscriptions/universal.xml), [DXperience](https://www.devexpress.com/subscriptions/dxperience.xml), or [Office File API](https://www.devexpress.com/products/net/office-file-api/) subscription.  

* The email client on your device should support HTML formatting. We tested the project with Outlook for Android and iOS.

## Implementation Details


### Implement Mail Merge

Use the [RichEditDocumentServer](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.RichEditDocumentServer) class to load an email template and define a data source with recipients:

  1. Copy files from the application bundle to the _AppData_ folder to access these files from code:
  
  ```csharp
  public async Task CopyWorkingFilesToAppData(string fileName) {
      using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
      string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
      using FileStream outputStream = File.OpenWrite(targetFile);
      fileStream.CopyTo(outputStream);
  }
  ```
  
  1. Call the [RichEditDocumentServerExtensions.LoadDocumentAsync](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.RichEditDocumentServerExtensions.LoadDocumentAsync.overloads?p=netstandard) method to load an email template:
  
  ```csharp
  async Task<RichEditDocumentServer> MailMergeAsync(EmailTemplate emailTemplate) {
      string workingFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, emailTemplate.DocumentSourcePath);
      RichEditDocumentServer mergeRichProcessor = new RichEditDocumentServer();
      await mergeRichProcessor.LoadDocumentAsync(workingFilePath);
      // ...
  }
  ```
  
  1. Assign the recipient data source to the [RichEditMailMergeOptions.DataSource](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.RichEditMailMergeOptions.DataSource?p=netstandard) property.
  
  ```csharp
  async Task<RichEditDocumentServer> MailMergeAsync(EmailTemplate emailTemplate) {
      // ...
      mergeRichProcessor.Document.Fields[0].Locked = true;
      mergeRichProcessor.Options.MailMerge.DataSource = new List<object> { new { RecipientName = mailToCustomer.FirstName, SenderName = currentUserName} };
      // ...
  }
  ```
  
  1. Call the [RichEditDocumentServer.MailMerge](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.RichEditDocumentServer.MailMerge(DevExpress.XtraRichEdit.API.Native.Document)?p=netstandard) method to merge data and send the result to the specified [Document](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.API.Native.Document?p=netstandard) instance.
  
  ```csharp
  async Task<RichEditDocumentServer> MailMergeAsync(EmailTemplate emailTemplate) {
      // ...
      MailMergeOptions myMergeOptions = mergeRichProcessor.Document.CreateMailMergeOptions();
      RichEditDocumentServer resultDocumentProcessor = new RichEditDocumentServer();
      resultDocumentProcessor.CreateNewDocument();
      myMergeOptions.MergeMode = MergeMode.NewSection;
      mergeRichProcessor.MailMerge(resultDocumentProcessor.Document);
      return resultDocumentProcessor;
  }
  ```
### Save Document as HTML

Save the mail merge result as a document. Call the [SubDocument.GetHtmlText](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.API.Native.SubDocument.GetHtmlText(DevExpress.XtraRichEdit.API.Native.DocumentRange-DevExpress.Office.Services.IUriProvider)?p=netstandard) method to convert the content to HTML.
### Send Emails

Call the [Email.ComposeAsync](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.applicationmodel.communication.email.composeasync?view=net-maui-7.0) method to send emails. You should specify required attributes in the following files: 

   _Android/AndroidManifest.xml_:
   
  ```xml
  <queries>
      <intent>
          <action android:name="android.intent.action.SENDTO" />
          <data android:scheme="mailto" />
      </intent>
	</queries>
  ```
  
  _iOS/Info.plist_:
  
  ```xml
  <key>LSApplicationQueriesSchemes</key>
	<array>
		  <string>mailto</string>
	</array>
  ```

  Refer to the following topic on learn.microsoft.com for more information: [Email](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/email?view=net-maui-7.0&tabs=ios).
  
* 

## Files to Review

<!-- default file list -->
* [MainPage.xaml](./CS/MainPage.xaml)
* [Models.cs](./CS/Model/Models.cs)
* [MainViewModel.cs](./CS/ViewModel/MainViewModel.cs)
* [Info.plist](./CS/Platforms/iOS/Info.plist)
* [AndroidManifest.xml](./CS/Platforms/Android/AndroidManifest.xml)
* [App.xaml](./CS/App.xaml)
<!-- default file list end -->

## Documentation

- [Spreadsheet Document API](https://docs.devexpress.com/OfficeFileAPI/14912/spreadsheet-document-api?p=netstandard)

## More Examples

* [Stocks App](https://github.com/DevExpress-Examples/maui-stocks-mini)
* [Demo Application](https://github.com/DevExpress-Examples/maui-demo-app)
