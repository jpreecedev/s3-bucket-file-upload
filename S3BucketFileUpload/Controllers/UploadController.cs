using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace S3BucketFileUpload.Controllers
{

    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public UploadController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Post()
        {
            var files = Request.Form.Files;
            var size = files.Sum(f => f.Length);

            var clientId = Guid.NewGuid().ToString();
            
            foreach (var file in files)
            {
                using (var client = new AmazonS3Client(RegionEndpoint.USEast1))
                {
                    using (var newMemoryStream = new MemoryStream())
                    {
                        file.CopyTo(newMemoryStream);

                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = newMemoryStream,
                            Key = $"{clientId}/{Path.GetRandomFileName()}",
                            BucketName = "ydkaws.com",
                            CannedACL = S3CannedACL.PublicRead,
                        };
                        
                        uploadRequest.Metadata.Add("FileName", file.FileName);

                        var fileTransferUtility = new TransferUtility(client);
                        await fileTransferUtility.UploadAsync(uploadRequest);
                    }
                }
            }

            return Ok(new {count = files.Count, size});
        }
    }
}