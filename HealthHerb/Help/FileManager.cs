using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Help
{
    public class FileManager
    {
        private readonly IWebHostEnvironment env;
        private string folder;

        public FileManager(IWebHostEnvironment env)
        {
            this.env = env;
            folder = "uploads";
        }

        public string Upload(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);

            if (!IsSupported(extension))
            {
                return null;
            }

            string fileName = GenerateName(extension);
            string uploadPath = GetDestination(fileName);

            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return fileName;
        }

        public List<string> Upload(List<IFormFile> files)
        {
            List<string> model = new List<string>();
            for (int i = 0; i < files.Count; i++)
            {
                string extension = Path.GetExtension(files[i].FileName);

                if (!IsSupported(extension))
                {
                    return null;
                }

                string fileName = GenerateName(extension);
                string uploadPath = GetDestination(fileName);

                using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                {
                    files[i].CopyTo(fileStream);
                }

                model.Add(fileName);
            }
            return model;
        }

        public bool Delete(string file)
        {
            try
            {
                string fullPath = GetDestination(file);
                File.Delete(fullPath);
            }
            catch (IOException exception)
            {
                throw exception;
            }

            return true;
        }

        private bool IsSupported(string extension)
        {
            return FileExtentions.SupportedExtensions().Any(m => m.Equals(extension));
        }

        private string GenerateName(string extension)
        {
            return DateTime.Now.Ticks.ToString() + extension;
        }

        private string GetDestination(string file)
        {
            return Path.Combine(env.WebRootPath, folder, file);
        }
    }
}
