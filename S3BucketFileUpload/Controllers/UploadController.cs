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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var keyName = "2943bc3a-5001-4636-ada6-a7a5b9b02532/tijtssq5.yhv";

            // Build the request with the bucket name and the keyName (name of the file)
            var request = new GetObjectRequest
            {
                BucketName = "ydkaws.com",
                Key = keyName
            };

            using (var client = new AmazonS3Client(RegionEndpoint.USEast1))
            {
                using (var response = await client.GetObjectAsync(request))
                {
                    var title = response.Metadata["x-amz-meta-title"];
                    var filename = response.Metadata["x-amz-meta-filename"];
                    var contentType = response.Headers["Content-Type"];

                    Console.WriteLine($"Object meta, Title: {title}");
                    Console.WriteLine($"Content type, Title: {contentType}");

                    var stream = new MemoryStream();
                    response.ResponseStream.CopyTo(stream);
                    stream.Position = 0;
                        return File(stream, contentType, filename);
                }
            }


            return Ok();
        }

        [HttpPost]
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

                        uploadRequest.Metadata.Add("x-amz-meta-filename", file.FileName);

                        var fileTransferUtility = new TransferUtility(client);
                        await fileTransferUtility.UploadAsync(uploadRequest);
                    }
                }
            }

            return Ok(new {count = files.Count, size});
        }
    }
}