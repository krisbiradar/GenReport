using FastEndpoints;
using GenReport.Infrastructure.Models.HttpRequests.Shared.Storage;
using GenReport.Infrastructure.Models.HttpResponse.Shared.Storage;
using GenReport.Infrastructure.Models.Shared;
using System.Net;
using System.Net.Http.Headers;

namespace GenReport.Api.Endpoints.Shared.Storage
{
    public class UploadFile(IHttpClientFactory httpClientFactory) : Endpoint<UploadFileRequest, HttpResponse<UploadFileResponse>>
    {
        public override void Configure()
        {
            Post("/storage/upload");
            AllowFileUploads();
            // Assuming we allow anonymous or we don't. The user didn't specify, but let's keep it consistent. AddMessage isn't anonymous.
        }

        public override async Task HandleAsync(UploadFileRequest req, CancellationToken ct)
        {
            if (Files.Count == 0)
            {
                await SendAsync(new HttpResponse<UploadFileResponse>(HttpStatusCode.BadRequest, "No file uploaded.", "ERR_BAD_REQUEST", []), cancellation: ct);
                return;
            }

            var file = Files[0];

            using var content = new MultipartFormDataContent();
            
            // Create a stream content from the uploaded file
            var fileStreamContent = new StreamContent(file.OpenReadStream());
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            // Add the file to the multipart form data content
            content.Add(fileStreamContent, "file", file.FileName);

            var client = httpClientFactory.CreateClient("GoService");
            
            try
            {
                // Proxy the request to the Go service
                var response = await client.PostAsync("/storage/upload", content, ct);
                var responseBody = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(responseBody))
                {
                    await SendAsync(new HttpResponse<UploadFileResponse>(
                        HttpStatusCode.InternalServerError, 
                        $"Failed to upload file. Service returned: {response.StatusCode}", 
                        "ERR_UPLOAD_FAILED", 
                        []), cancellation: ct);
                    return;
                }

                // Assume responseBody contains the raw URL string and strip any quotes
                var url = responseBody.Trim('"', '\n', '\r', ' ');

                var uploadResponse = new UploadFileResponse
                {
                    Url = url,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Size = file.Length
                };

                await SendAsync(new HttpResponse<UploadFileResponse>(uploadResponse, "File uploaded successfully", HttpStatusCode.OK), cancellation: ct);
            }
            catch (Exception ex)
            {
                await SendAsync(new HttpResponse<UploadFileResponse>(HttpStatusCode.InternalServerError, $"Error proxying file: {ex.Message}", "ERR_INTERNAL", []), cancellation: ct);
            }
        }
    }
}
