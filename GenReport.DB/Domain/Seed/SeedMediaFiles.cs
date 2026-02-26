using GenReport.Domain.Entities.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenReport.DB.Domain.Seed
{
    public partial class ApplicationDBContextSeeder
    {
        public static List<string> MimeTypes = new() { "image/jpeg","image/png","image/gif","image/svg+xml","image/webp","image/bmp","image/tiff","image/x-icon","image/vnd.microsoft.icon","image/x-png","image/avif","image/heic","image/heif","image/x-canon-cr2","image/x-adobe-dng","image/x-nikon-nef","image/x-sony-arw","image/x-panasonic-rw2","image/x-fujifilm-raf","image/x-olympus-orf","video/mp4","video/x-msvideo","video/x-matroska","video/webm","video/quicktime","video/x-flv","video/x-ms-wmv","video/3gpp","video/3gpp2","video/ogg","video/mpeg","video/x-m4v","video/x-ms-asf","video/x-ms-vob","video/x-f4v","video/x-mxf","video/h264","audio/mpeg","audio/wav","audio/x-wav","audio/mp4","audio/aac","audio/flac","audio/ogg","audio/opus","audio/x-ms-wma","audio/webm","audio/x-aiff","audio/x-matroska","audio/x-m4a","audio/midi","audio/x-midi","audio/x-mp3","audio/x-flac","audio/x-wav","audio/x-ms-wma","audio/x-aac","audio/x-m4a","audio/x-ogx","audio/x-vorbis","audio/x-xm","audio/x-mod","audio/x-s3m","audio/x-it","audio/x-amzxml","audio/x-caf","audio/x-ape","audio/x-musepack","audio/x-tta","audio/x-opus+ogg","audio/x-adpcm","audio/x-snd","application/pdf","application/msword","application/vnd.openxmlformats-officedocument.wordprocessingml.document","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","application/vnd.ms-excel","application/vnd.ms-powerpoint","application/vnd.openxmlformats-officedocument.presentationml.presentation","application/rtf","application/vnd.oasis.opendocument.text","application/vnd.oasis.opendocument.spreadsheet","application/vnd.oasis.opendocument.presentation","application/vnd.apple.pages","application/vnd.google-apps.document","application/vnd.google-apps.spreadsheet","application/vnd.google-apps.presentation","application/x-iwork-keynote-sffkey","application/x-iwork-pages-sffpages","application/x-iwork-numbers-sffnumbers","application/epub+zip","application/x-mobipocket-ebook","application/x-fictionbook+xml","application/vnd.amazon.ebook","application/onenote","application/atom+xml","application/xhtml+xml","application/x-shockwave-flash","application/x-tex","application/x-latex" };
        public async Task SeedMediaFiles()
        {
            
            var files = Enumerable.Range(0, 20).Select(x => new MediaFile(Faker.Internet.SecureUrl(),$"{Faker.Name.FullName()}-profilepic.png",MimeTypes[1],Faker.RandomNumber.Next(100000L)));
            await applicationDbContext.MediaFiles.AddRangeAsync(files);
           await applicationDbContext.SaveChangesAsync();
        }
    }
}
