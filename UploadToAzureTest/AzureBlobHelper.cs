using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadToAzureTest
{
    public class AzureBlobHelper
    {
        CloudBlobClient _client;
        CloudBlobContainer _container;

 


        public async Task UploadFile(string localFile, string blobName)
        {
            CloudBlobContainer container = await GetContainer();
            var blob = container.GetBlockBlobReference(blobName);
            await blob.UploadFromFileAsync(localFile);
        }

        private async Task<CloudBlobContainer> GetContainer()
        {
            if (_container != null)
            {
                return this._container;
            }

            var client = GetBlobClient();

            _container = client.GetContainerReference("yallabenayalla");
            await _container.CreateIfNotExistsAsync();
            await _container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off });
            return _container;
        }

        private CloudBlobClient GetBlobClient()
        {
            if (_client != null)
            {
                return _client;
            }

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tmostorage;AccountKey=qX2BmuMOboG193SKLZJ9PO0hNsPopdkfezXjI/sdLPwgIwdPZnVUqTWxtG483GC1T59wV22s4HJlDgxL9NluHg==;EndpointSuffix=core.windows.net");
            _client = cloudStorageAccount.CreateCloudBlobClient();
            return _client;
        }

        public async Task<string> GetBlobSAS(string blobName)
        {
  
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Read;
            //sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

            var container = await GetContainer();
            var blob = container.GetBlockBlobReference(blobName);
            return $"{blob.Uri.ToString()}{blob.GetSharedAccessSignature(sasConstraints)}";
        }
        
    }
}
