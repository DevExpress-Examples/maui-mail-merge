using Microsoft.Maui.Storage;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MailMerge.Model {
    [Table("Customers")]
    public class Models  {
        byte[] photo;

        [PrimaryKey, AutoIncrement, NotNull, Column("ID")]
        public int? ID {
            get;
            set;
        }

        [Required(ErrorMessage = "First Name cannot be empty")]
        public string FirstName {
            get;
            set;
        }
        [Required(ErrorMessage = "Last Name cannot be empty")]
        public string LastName {
            get;
            set;
        }
        public string Company {
            get;
            set;
        }
        public string Address {
            get;
            set;
        }
        public string City {
            get;
            set;
        }
        public string State {
            get;
            set;
        }
        public int ZipCode {
            get;
            set;
        }
        public string HomePhone {
            get;
            set;
        }

        public string Email {
            get;
            set;
        }

        public byte[] Photo {
            get {
                return photo;
            }
            set {
                photo = value;
                CachePhotoLocally();
            }
        }

        private void CachePhotoLocally() {
            string targetFile = Path.Combine(FileSystem.Current.CacheDirectory, $"{ID}.png");
            CachedImagePath = targetFile;
            if (File.Exists(targetFile))
                return;
            using var writer = new BinaryWriter(File.OpenWrite(targetFile));
            writer.Write(Photo);
        }

        [Ignore]
        public string CachedImagePath {
            get;
            set;
        }
    }

    public class EmailTemplate {
        public EmailTemplate(string name, string documentPath) {
            Name = name;
            DocumentSourcePath = documentPath;
        }
        public string Name {
            get;
            set;
        }
        public string DocumentSourcePath {
            get;
            set;
        }
    }
    public class BindableBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
