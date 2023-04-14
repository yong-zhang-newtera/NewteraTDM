using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.BlobStorage.S3Storage
{
    public class S3ProviderOptions
    {
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public string Bucket { get; set; }
        public string ServiceUrl { get; set; }
        public string ServerSideEncryptionMethod { get; set; }
        public string ProfileName { get; set; }

        /// <summary>
        /// Gets or sets the threshold at which chunked uploads are used.
        /// </summary>
        /// <value>The size in bytes</value>
        public long ChunkedUploadThreshold { get; set; } = 100000000;
        public TimeSpan? Timeout { get; set; }
    }
}
