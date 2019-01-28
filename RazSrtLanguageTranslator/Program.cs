using Newtonsoft.Json;
using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RazSrtLanguageTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("USAGE: RazSrtLanguageTranslator.exe <from-language> <to-language> <source-file.srt> <dest-file.srt>");
                return;
            }

            var from = args[0];
            var to = args[1];
            var source = args[2];
            if (!File.Exists(source))
            {
                Console.WriteLine("Source file \"{0}\" does not exist!", source);
                return;
            }

            var dest = args[3];
            var isValid = !string.IsNullOrEmpty(dest)
                && dest.IndexOfAny(Path.GetInvalidPathChars()) < 0;
            if (!isValid)
            {
                Console.WriteLine("Destination file format \"{0}\" is invalid.", dest);
                return;
            }

            GenerateSrt(from, to, source, dest);

            Console.WriteLine("SRT file {0} successfully created.", dest);
            Console.ReadLine();
        }

        private static void GenerateSrt(string from, string to, string source, string dest)
        {
            Console.WriteLine("Parsing {0}...", source);
            var items = ParseSrt(source);

            Console.WriteLine("Generating SRT file...");
            var translator = new TranslatorHelper(Properties.Settings.Default.ApiHostUrl, Properties.Settings.Default.ApiKey);
            const string format = @"hh\:mm\:ss\,fff";
            using (var outputStream = new StreamWriter(dest, false))
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var startTime = TimeSpan.FromMilliseconds(item.StartTime);
                    var endTime = TimeSpan.FromMilliseconds(item.EndTime);

                    outputStream.WriteLine((i + 1).ToString());
                    outputStream.WriteLine("{0} --> {1}", startTime.ToString(format), endTime.ToString(format));

                    string jsonResponse;
                    string text = string.Join("\r\n", item.Lines);
                    try
                    {
                        jsonResponse = translator.Translate(text, from, to);
                    }
                    catch
                    {
                        Thread.Sleep(30000);
                        translator.InitializeToken(Properties.Settings.Default.ApiKey);
                        Thread.Sleep(5000);
                        jsonResponse = translator.Translate(text, from, to);
                    }

                    var result = JsonConvert.DeserializeObject<TranslationResult[]>(jsonResponse);
                    if (result.Length != 0 && result[0].translations.Count != 0)
                        outputStream.WriteLine(result[0].translations[0].text);

                    outputStream.WriteLine();
                }
            }
        }

        private static List<SubtitleItem> ParseSrt(string source)
        {
            var parser = new SrtParser();
            List<SubtitleItem> items;
            using (var inputStream = File.OpenRead(source))
            {
                items = parser.ParseStream(inputStream, Encoding.Default);
            }

            return items;
        }
    }
}
