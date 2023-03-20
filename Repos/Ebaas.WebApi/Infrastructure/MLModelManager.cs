using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Xml;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Ebaas.WebApi.Models;
using Newtera.Common.MetaData.FileType;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Model manager that provides services for saving and delete the
    /// </summary>
    public class MLModelManager
    {
        private const string MODELS_DIR = @"\Models";
        private const string MANIFEST_FILE = "manifest.json";

        private static MLModelManager theInstance;

        /// <summary>
        /// File manager that provides services for gettting, uploading, downloading, delete the files in a local disk for data instances
        /// </summary>
        private MLModelManager()
        {
        }

        /// <summary>
        /// Gets the MLModelManager instance.
        /// </summary>
        /// <returns> The MLModelManager instance.</returns>
        static public MLModelManager Instance
        {
            get
            {
                return theInstance;
            }
        }

        static MLModelManager()
        {
            // Initializing the instance.
            {
                theInstance = new MLModelManager();
            }
        }

        /// <summary>
        /// Save a model file
        /// </summary>
        /// <returns>Added fileViewNodel objects</returns>
        public async Task<IEnumerable<FileViewModel>> AddModel(HttpRequestMessage request, string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            var files = new List<FileViewModel>();

            string modelFileDir = AddModelInfo(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);

            var provider = new ModelFileMultipartFormDataStreamProvider(modelFileDir);

            // save model files in a local disk
            await request.Content.ReadAsMultipartAsync(provider);

            string localFileName;

            foreach (var file in provider.FileData)
            {
                localFileName = file.LocalFileName;
                FileInfo info = new FileInfo(localFileName);

                FileViewModel fileView = new FileViewModel();

                FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Extension);
                fileView.ID = info.Name;
                fileView.Name = info.Name;
                fileView.Description = fileTypeInfo.Description;
                fileView.Size = info.Length / 1024;
                fileView.Created = info.CreationTime;
                fileView.Type = info.Extension;
                if (fileTypeInfo.Suffixes.Count > 0)
                {
                    // take the first suffix as default
                    fileView.Suffix = ((FileSuffix)fileTypeInfo.Suffixes[0]).Suffix;
                }

                files.Add(fileView);
            }

            return files;
        }

        /// <summary>
        /// Save a preprocess file
        /// </summary>
        /// <returns>Added fileViewNodel objects</returns>
        public async Task<IEnumerable<FileViewModel>> AddPreprocess(HttpRequestMessage request, string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            var files = new List<FileViewModel>();

            string programFileDir = CreateModelDir(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);;

            var provider = new ModelFileMultipartFormDataStreamProvider(programFileDir);

            // save model files in a local disk
            await request.Content.ReadAsMultipartAsync(provider);

            string programFile = null;

            foreach (var file in provider.FileData)
            {
                string localFileName = file.LocalFileName;
                FileInfo info = new FileInfo(localFileName);

                FileViewModel fileView = new FileViewModel();

                FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Extension);
                fileView.ID = info.Name;
                fileView.Name = info.Name;
                programFile = info.Name;
                fileView.Description = fileTypeInfo.Description;
                fileView.Size = info.Length / 1024;
                fileView.Created = info.CreationTime;
                fileView.Type = info.Extension;
                if (fileTypeInfo.Suffixes.Count > 0)
                {
                    // take the first suffix as default
                    fileView.Suffix = ((FileSuffix)fileTypeInfo.Suffixes[0]).Suffix;
                }

                files.Add(fileView);
            }

            if (programFile != null)
            {
                AddPreprocessInfo(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast, programFile);
            }

            return files;
        }

        /// <summary>
        /// Save a postprocess file
        /// </summary>
        /// <returns>Added fileViewNodel objects</returns>
        public async Task<IEnumerable<FileViewModel>> AddPostprocess(HttpRequestMessage request, string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            var files = new List<FileViewModel>();

            string programFileDir = CreateModelDir(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast); ;

            var provider = new ModelFileMultipartFormDataStreamProvider(programFileDir);

            // save model files in a local disk
            await request.Content.ReadAsMultipartAsync(provider);

            string programFile = null;

            foreach (var file in provider.FileData)
            {
                string localFileName = file.LocalFileName;
                FileInfo info = new FileInfo(localFileName);

                FileViewModel fileView = new FileViewModel();

                FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Extension);
                fileView.ID = info.Name;
                fileView.Name = info.Name;
                programFile = info.Name;
                fileView.Description = fileTypeInfo.Description;
                fileView.Size = info.Length / 1024;
                fileView.Created = info.CreationTime;
                fileView.Type = info.Extension;
                if (fileTypeInfo.Suffixes.Count > 0)
                {
                    // take the first suffix as default
                    fileView.Suffix = ((FileSuffix)fileTypeInfo.Suffixes[0]).Suffix;
                }

                files.Add(fileView);
            }

            if (programFile != null)
            {
                AddPostprocessInfo(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast, programFile);
            }

            return files;
        }

        /// <summary>
        /// Get an array of model infos that match the criteria
        /// </summary>
        /// <returns></returns>
        public async Task<List<ModelInfo>> GetModelInfos(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            List<ModelInfo> modelInfos = null;

            await Task.Factory.StartNew(() =>
            {
                modelInfos = GetMatchedModelInfos(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);
            });

            return modelInfos;
        }

        /// <summary>
        /// Delete a model
        /// </summary>
        /// <returns></returns>
        public async Task<FileActionResult> DeleteModel(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {

                    string modelFileDir = DeleteModelInfo(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);

                    // delete the file
                    if (!string.IsNullOrEmpty(modelFileDir) &&
                        Directory.Exists(modelFileDir))
                    {
                        Directory.Delete(modelFileDir, true);
                    }
                    else
                    {
                        throw new FileNotFoundException("File not found");
                    }
                });

                return new FileActionResult { Successful = true, Message = "Model deleted successfully" };
            }
            catch (FileNotFoundException)
            {
                return new FileActionResult { Successful = false, NotFound = true, Message = "Model not found" };
            }
            catch (Exception ex)
            {
                return new FileActionResult { Successful = false, Message = "error deleting model " + ex.GetBaseException().Message };
            }
        }

        /// <summary>
        /// Add model info to a manifest file and create a directory for the model file
        /// </summary>
        /// <returns></returns>
        private string AddModelInfo(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            lock (this)
            {
                // create a dir for the model files
                string fileDir = CreateModelDir(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);

                AddToManifest(ModelInfoType.Model, schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast, null);

                return fileDir;
            }
        }

        /// <summary>
        /// Add preprocess program info to a manifest file
        /// </summary>
        /// <returns></returns>
        private void AddPreprocessInfo(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast, string programName)
        {
            lock (this)
            {
                AddToManifest(ModelInfoType.Preprocess, schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast, programName);
            }
        }

        /// <summary>
        /// Add postprocess program info to a manifest file
        /// </summary>
        /// <returns></returns>
        private void AddPostprocessInfo(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast, string programName)
        {
            lock (this)
            {
                AddToManifest(ModelInfoType.Postprocess, schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast, programName);
            }
        }

        /// <summary>
        /// Delete model info to a manifest file
        /// </summary>
        /// <returns></returns>
        private string DeleteModelInfo(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            lock (this)
            {
                // create a dir for the model files
                string fileDir = CreateModelDir(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);

                DeleteFromManifest(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);

                return fileDir;
            }
        }

        private string CreateModelDir(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            SchemaInfo schemaInfo = new SchemaInfo();
            schemaInfo.Name = schemaName;

            string fileDir = NewteraNameSpace.GetAppHomeDir() + MODELS_DIR;
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            // next level is schemaName 1.0
            fileDir += @"\" + schemaInfo.NameAndVersion;
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            // next level is class name
            fileDir += @"\" + className;
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            // next level is a directory for the model file
            string modelDir = GetDirHashCode(xmlSchemaName, fieldName, frequency, maxForecast);

            // next level is dir with hashcode
            fileDir += @"\" + modelDir;
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            return fileDir;
        }

        private void AddToManifest(ModelInfoType modelInfoType, string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast, string programName)
        {
            SchemaInfo schemaInfo = new SchemaInfo();
            schemaInfo.Name = schemaName;

            string manifestFilePath = NewteraNameSpace.GetAppHomeDir() + MODELS_DIR + @"\" + schemaInfo.NameAndVersion + @"\" + className + @"\" + MANIFEST_FILE;

            List<ModelInfo> items = GetManifestInfos(manifestFilePath);

            ModelInfo found = null;

            foreach (ModelInfo info in items)
            {
                if (info.XMLSchemaName == xmlSchemaName &&
                    info.FieldName == fieldName &&
                    info.Frequency == frequency &&
                    info.MaxForecast == maxForecast)
                {
                    found = info;
                    break;
                }
            }

            if (found == null &&
                modelInfoType == ModelInfoType.Model)
            {
                ModelInfo info = new ModelInfo();
                info.ModelDirName = GetDirHashCode(xmlSchemaName, fieldName, frequency, maxForecast);
                info.XMLSchemaName = xmlSchemaName;
                info.FieldName = fieldName;
                info.Frequency = frequency;
                info.MaxForecast = maxForecast;

                items.Add(info);

                SaveManifestInfos(manifestFilePath, items);
            }
            else if (modelInfoType == ModelInfoType.Preprocess)
            {
                found.Preprocess = programName;
                SaveManifestInfos(manifestFilePath, items);
            }
            else if (modelInfoType == ModelInfoType.Postprocess)
            {
                found.Postprocess = programName;
                SaveManifestInfos(manifestFilePath, items);
            }
        }

        private void DeleteFromManifest(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            SchemaInfo schemaInfo = new SchemaInfo();
            schemaInfo.Name = schemaName;

            string manifestFilePath = NewteraNameSpace.GetAppHomeDir() + MODELS_DIR + @"\" + schemaInfo.NameAndVersion + @"\" + className + @"\" + MANIFEST_FILE;

            List<ModelInfo> items = GetManifestInfos(manifestFilePath);

            ModelInfo found = null;

            foreach (ModelInfo info in items)
            {
                if (info.XMLSchemaName == xmlSchemaName &&
                    info.FieldName == fieldName &&
                    info.Frequency == frequency &&
                    info.MaxForecast == maxForecast)
                {
                    found = info;
                    break;
                }
            }

            if (found != null)
            {
                items.Remove(found);

                SaveManifestInfos(manifestFilePath, items);
            }
        }

        public string GetModelFileDir(string schemaName, string className, string modelId)
        {
            SchemaInfo schemaInfo = new SchemaInfo();
            schemaInfo.Name = schemaName;

            return NewteraNameSpace.GetAppHomeDir() + MODELS_DIR + @"\" + schemaInfo.NameAndVersion + @"\" + className + @"\" + modelId;
        }

        public List<ModelInfo> GetMatchedModelInfos(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            lock (this)
            {
                List<ModelInfo> modelInfos = new List<ModelInfo>();
                SchemaInfo schemaInfo = new SchemaInfo();
                schemaInfo.Name = schemaName;

                string manifestFilePath = NewteraNameSpace.GetAppHomeDir() + MODELS_DIR + @"\" + schemaInfo.NameAndVersion + @"\" + className + @"\" + MANIFEST_FILE;

                List<ModelInfo> items = GetManifestInfos(manifestFilePath);

                foreach (ModelInfo info in items)
                {
                    if (info.XMLSchemaName == xmlSchemaName)
                    {
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            if (info.FieldName == fieldName)
                            {
                                if (!string.IsNullOrEmpty(frequency))
                                {
                                    if (info.Frequency == frequency)
                                    {
                                        if (!string.IsNullOrEmpty(maxForecast))
                                        {
                                            if (info.MaxForecast == maxForecast)
                                            {
                                                modelInfos.Add(info);
                                            }
                                        }
                                        else
                                        {
                                            modelInfos.Add(info);
                                        }
                                    }
                                }
                                else
                                {
                                    modelInfos.Add(info);
                                }
                            }
                        }
                        else
                        {
                            modelInfos.Add(info);
                        }
                    }
                }

                return modelInfos;
            }
        }

        public ModelInfo GetModelInfo(string schemaName, string className, string modelId)
        {
            lock (this)
            {
                ModelInfo modelInfo = null;
                SchemaInfo schemaInfo = new SchemaInfo();
                schemaInfo.Name = schemaName;

                string manifestFilePath = NewteraNameSpace.GetAppHomeDir() + MODELS_DIR + @"\" + schemaInfo.NameAndVersion + @"\" + className + @"\" + MANIFEST_FILE;

                List<ModelInfo> items = GetManifestInfos(manifestFilePath);

                foreach (ModelInfo info in items)
                {
                    if (info.ModelDirName == modelId)
                    {
                        modelInfo = info;
                        break;
                    }
                }

                return modelInfo;
            }
        }

        private string GetDirHashCode(string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            // next level is a directory for the model file
            string modelDir = xmlSchemaName + fieldName + frequency + maxForecast;

            // use hash code as dir name
            modelDir = Math.Abs(modelDir.GetHashCode()).ToString();

            return modelDir;
        }

        private List<ModelInfo> GetManifestInfos(string manifestFile)
        {
            List<ModelInfo> items;

            if (File.Exists(manifestFile))
            {
                using (StreamReader r = new StreamReader(manifestFile))
                {
                    string json = r.ReadToEnd();
                    items = JsonConvert.DeserializeObject<List<ModelInfo>>(json);
                }
            }
            else
            {
                items = new List<ModelInfo>();
            }

            return items;
        }

        private void SaveManifestInfos(string manifestFile, List<ModelInfo> items)
        {
            if (File.Exists(manifestFile))
            {
                File.Delete(manifestFile);
            }

            string json = JsonConvert.SerializeObject(items.ToArray(), Newtonsoft.Json.Formatting.Indented);

            //write string to file
            System.IO.File.WriteAllText(manifestFile, json);
        }
    }

    internal enum ModelInfoType
    {
        Model,
        Preprocess,
        Postprocess
    }

    public class ModelInfo
    {
        public string ModelDirName { get; set; }
        public string XMLSchemaName { get; set; }
        public string Frequency { get; set; }
        public string FieldName { get; set; }
        public string MaxForecast { get; set; }
        public string Preprocess { get; set; }
        public string Postprocess { get; set; }
    }
}