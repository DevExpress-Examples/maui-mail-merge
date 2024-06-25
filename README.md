<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/621277459/23.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1157166)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# DevExpress .NET MAUI Controls - Send Template-base Messages Using Mail Merge Operations

This .NET MAUI example uses the DevExpress Office File API to generate email messages using mail merge operations. Data placeholders within the email template obtain data values from a database. The features outlined in this example require a license for the [Office File API](https://www.devexpress.com/products/net/office-file-api/). Our Office File API (Basic) is included in the following DevExpress Subscriptions: [Universal](https://www.devexpress.com/subscriptions/universal.xml), [DXperience](https://www.devexpress.com/subscriptions/dxperience.xml), and [Office File API](https://www.devexpress.com/products/net/office-file-api/).  

<img src="https://user-images.githubusercontent.com/12169834/228828852-28a3feb6-e91c-4bd1-8945-2a2f80ae9e18.png" width="30%"/>

## Requirements

* Register the DevExpress NuGet Gallery in Visual Studio to restore the NuGet packages used in this solution. See the following topic for additional information: [Get Started with DevExpress Mobile UI for .NET MAUI](https://docs.devexpress.com/MAUI/403249/get-started).

	You can also refer to the following YouTube video for more information on how to get started with the DevExpress [Setting up a .NET MAUI Project](https://www.youtube.com/watch?v=juJvl5UicIQ).

* To run this example, you need to own/purchase a DevExpress [Universal](https://www.devexpress.com/subscriptions/universal.xml), [DXperience](https://www.devexpress.com/subscriptions/dxperience.xml), or [Office File API](https://www.devexpress.com/products/net/office-file-api/) Subscription.
* The email client on your device must support HTML formatting. We tested this project with Outlook for Android and iOS.


## Implementation Details


### Implement Mail Merge

Use the [RichEditDocumentServer](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.RichEditDocumentServer)  class to load an email template and define the data source (recipient information):

1. Copy files from the application bundle to the _AppData_ folder to access these files from code:
  
	  ```csharp
	  public async Task CopyWorkingFilesToAppData(string fileName) {
	      using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
	      string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
	      using FileStream outputStream = File.OpenWrite(targetFile);
	      fileStream.CopyTo(outputStream);
	  }
	  ```
  
1. Call the [RichEditDocumentServerExtensions.LoadDocumentAsync](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.RichEditDocumentServerExtensions.LoadDocumentAsync.overloads) method to load the email template:
  
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
  
1. Call the [RichEditDocumentServer.MailMerge](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.RichEditDocumentServer.MailMerge(DevExpress.XtraRichEdit.API.Native.Document)?p=netstandard) method to merge data and send results to the specified [Document](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.API.Native.Document?p=netstandard) instance.
  
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

Save mail merge results as a document. Call the [SubDocument.GetHtmlText](https://docs.devexpress.com/OfficeFileAPI/DevExpress.XtraRichEdit.API.Native.SubDocument.GetHtmlText(DevExpress.XtraRichEdit.API.Native.DocumentRange-DevExpress.Office.Services.IUriProvider)) method to convert content to HTML.

### Send Emails

Call the [Email.ComposeAsync](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.applicationmodel.communication.email.composeasync?view=net-maui-7.0) method to send emails. You should specify required attributes within the following files:

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

 Refer to the following Microsoft help topic for more information: [Email](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/email?view=net-maui-7.0&tabs=ios).


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

- [Word Processing Document API](https://docs.devexpress.com/OfficeFileAPI/17488/word-processing-document-api)

## More Examples

* [Stocks App](https://github.com/DevExpress-Examples/maui-stocks-mini)
* [Demo Application](https://github.com/DevExpress-Examples/maui-demo-app)
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=maui-mail-merge&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=maui-mail-merge&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
