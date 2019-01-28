using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazSrtLanguageTranslator
{
    [Serializable]
    public class TranslationResult
    {
        public DetectedLanguage detectedLanguage { get; set; }

        public IList<Translation> translations;
    }

    [Serializable]
    public class DetectedLanguage
    {
        public string language { get; set; }
        public string score { get; set; }
    }

    [Serializable]
    public class Translation
    {
        public string text { get; set; }
        public string transliteration { get; set; }
        public string to { get; set; }
    }
}
