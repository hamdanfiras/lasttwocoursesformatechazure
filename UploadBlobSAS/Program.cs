using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace UploadBlobSAS
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sas = "https://tmostorage.blob.core.windows.net/yallabenayalla/hello?sv=2018-03-28&sr=b&sig=GES0i6zLpbsEX%2FPJBgfiSzkoivTHn7AIRVYoNhNwS9A%3D&se=2019-12-09T19%3A29%3A09Z&sp=rcw";
            var cloudBlockBlob = new CloudBlockBlob(new Uri(sas));
            await cloudBlockBlob.UploadFromFileAsync(@"c:\test\abc\hello.docx");
        }
    }
}
