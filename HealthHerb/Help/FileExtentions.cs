using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Help
{
    public static class FileExtentions
    {
        public static List<string> SupportedExtensions()
        {
            return new List<string>
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".PNG"
            };
        }
    }
}
