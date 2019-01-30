# RazSrtLanguageTranslator
This is a simple command line utility that translates an SRT file from 1 language to another, using Microsoft Azure's Cognitive Services - Text Translator API.

Simple syntax is:
```
RazSrtLanguageTranslator.exe <from-language> <to-language> <source-file.srt> <dest-file.srt>
```

# Getting Started
To get started, simply enter your Text Translator API key in the App.config.

The pushed code uses the Asia Pacific API URL by default. For other regions, please see: https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-reference#base-urls

# Technologies Used
This simple utility was created using .Net Framework 4.6.2.
There are 2 Nuget Package dependencies:
- Newtonsoft.Json: https://www.nuget.org/packages/Newtonsoft.Json/
- SubtitlesParser: https://www.nuget.org/packages/SubtitlesParser/
