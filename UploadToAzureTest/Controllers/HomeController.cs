using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using UploadToAzureTest.Models;

namespace UploadToAzureTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AzureBlobHelper azureBlobHelper;

        public HomeController(ILogger<HomeController> logger, AzureBlobHelper azureBlobHelper)
        {
            _logger = logger;
            this.azureBlobHelper = azureBlobHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Upload()
        {
            await azureBlobHelper.UploadFile(@"C:\test\abc\abc.rtf", "xyz");
            return Content("great");
        }

        public async Task<IActionResult> GetBlobToken(string blobName)
        {
            var sas = await azureBlobHelper.GetBlobSAS(blobName);
            return Content(sas);
        }


    }
}
