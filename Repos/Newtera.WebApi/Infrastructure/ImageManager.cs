using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.WebApi.Models;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.FileType;

namespace Newtera.WebApi.Infrastructure
{
    /// <summary>
    /// Image manager that provides services for gettting, adding, or delete the static images in a local disk of the site
    /// </summary>
    public class ImageManager
    {
        /// <summary>
        /// File manager that provides services for gettting, uploading, downloading, delete the files in a local disk for data instances
        /// </summary>
        public ImageManager()
        {
        }

        /// <summary>
        /// Get a list of ImageInfo objects representing the images in a given directory
        /// </summary>
        /// <param name="imageDir"></param>
        /// <returns>A list of ImageInfo objects</returns>
        public async Task<IEnumerable<ImageModel>> GetImages(string imageDir)
        {
            List<ImageModel> images = new List<ImageModel>();

            await Task.Factory.StartNew(() =>
            {
                images = GetImageModels(imageDir);
            });
                                         
            return images;
        }

        /// <summary>
        /// Add files  to a data instance
        /// </summary>
        /// <param name="request"></param>
        /// <param name="imageDir"></param>
        /// <returns>Added fileViewNodel objects</returns>
        public async Task<IEnumerable<FileViewModel>> AddImages(HttpRequestMessage request, string imageDir)
        {
            var files = new List<FileViewModel>();

            string baseDir = GetAbsoluteImageDir(imageDir);

            if (!string.IsNullOrEmpty(baseDir) && Directory.Exists(baseDir))
            {
                var provider = new FileMultipartFormDataStreamProvider(baseDir);

                // create file infos in db and save files in a local disk
                await request.Content.ReadAsMultipartAsync(provider);

                string localFileName;

                foreach (var file in provider.FileData)
                {
                    localFileName = file.LocalFileName;

                    FileInfo info = new FileInfo(localFileName);

                    FileViewModel fileView = new FileViewModel();

                    FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Extension);
                    fileView.Name = info.Name;
                    fileView.Type = info.Extension;

                    files.Add(fileView);
                }
            }
            else
            {
                throw new Exception("Directory " + baseDir + " doens't exist");
            }

            return files;
        }

        /// <summary>
        /// Add files  to a data instance
        /// </summary>
        /// <param name="request"></param>
        /// <param name="imageDir"></param>
        /// <returns>Added fileViewNodel objects</returns>
        public async Task<string> DeleteImage(string imageDir, string imageName)
        {
            string result = "ok";

            await Task.Factory.StartNew(() =>
            {
                DeleteImageFile(imageDir, imageName);
            });

            return result;
        }

        /// <summary>
        /// Return a list of ImageModel
        /// </summary>
        /// <param name="imageDir"></param>
        /// <returns>List of ImageModel objects</returns>
        public List<ImageModel> GetImageModels(string imageDir)
        {
            List<ImageModel> imageModels = new List<ImageModel>();

            if (!string.IsNullOrEmpty(imageDir))
            {
                string baseDir = GetAbsoluteImageDir(imageDir);

                if (!string.IsNullOrEmpty(baseDir) && Directory.Exists(baseDir))
                {
                    ImageModel imageModel;

                    DirectoryInfo dirInfo = new DirectoryInfo(baseDir);

                    FileInfo[] fileInfos = dirInfo.GetFiles();

                    foreach (FileInfo info in fileInfos)
                    {
                        imageModel = new ImageModel();

                        FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Extension);
                        imageModel.Name = info.Name;
                        imageModel.Type = info.Extension;

                        imageModels.Add(imageModel);
                    }
                }
            }

            return imageModels;
        }

        /// <summary>
        /// Delete an image file
        /// </summary>
        /// <param name="imageDir"></param>
        /// <param name="imageName"></param>
        /// <returns>Added fileViewNodel objects</returns>
        private void DeleteImageFile(string imageDir, string imageName)
        {
            string baseDir = GetAbsoluteImageDir(imageDir);

            if (!string.IsNullOrEmpty(baseDir) && Directory.Exists(baseDir))
            {
                string imageFilePath = baseDir + imageName;

                if (File.Exists(imageFilePath))
                {
                    File.Delete(imageFilePath);
                }
            }
        }
        private string GetAbsoluteImageDir(string imageDir)
        {
            string staticFilesDir = NewteraNameSpace.GetStaticFilesDir();

            if (imageDir.StartsWith(@"\"))
            {
                return staticFilesDir + imageDir.Substring(1) + @"\";
            }
            else
            {
                return staticFilesDir + imageDir + @"\";
            }
        }
    }

    /// <summary>
    /// Image Model
    /// </summary>
    public class ImageModel
    {
        public string Name;
        public string Type;
    }
}