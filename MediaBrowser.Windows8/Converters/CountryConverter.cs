using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class CountryConverter : IValueConverter
    {
        private static List<Language> _allCultureInfos;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                if (_allCultureInfos == null) _allCultureInfos = GetCultures();
                var countryCode = (string) value;
                var countryCulture = _allCultureInfos.SingleOrDefault(x => x.ThreeLetterName.ToLower().Equals(countryCode.ToLower()));
                if (countryCulture != null)
                    return countryCulture.NativeName;
            }
            return null;
        }

        public List<Language> GetCultures()
        {
            var cultures = new List<Language>
                               {
                                   new Language {Name = "af", NativeName = "Afrikaans", ThreeLetterName = "afr", EnglishName = "Afrikaans"},
                                   new Language {Name = "sq", NativeName = "Shqip", ThreeLetterName = "sqi", EnglishName = "Albanian"},
                                   new Language {Name = "gsw", NativeName = "Elsässisch", ThreeLetterName = "gsw", EnglishName = "Alsatian"},
                                   new Language {Name = "am", NativeName = "አማርኛ", ThreeLetterName = "amh", EnglishName = "Amharic"},
                                   new Language {Name = "ar", NativeName = "العربية", ThreeLetterName = "ara", EnglishName = "Arabic"},
                                   new Language {Name = "hy", NativeName = "Հայերեն", ThreeLetterName = "hye", EnglishName = "Armenian"},
                                   new Language {Name = "as", NativeName = "অসমীয়া", ThreeLetterName = "asm", EnglishName = "Assamese"},
                                   new Language {Name = "az", NativeName = "Azərbaycan­ılı", ThreeLetterName = "aze", EnglishName = "Azerbaijani"},
                                   new Language {Name = "az-Cyrl", NativeName = "Азәрбајҹан дили", ThreeLetterName = "aze", EnglishName = "Azerbaijani (Cyrillic)"},
                                   new Language {Name = "az-Latn", NativeName = "Azərbaycan dili (Azərbaycan)", ThreeLetterName = "aze", EnglishName = "Azerbaijani (Latin)"},
                                   new Language {Name = "bn", NativeName = "বাংলা", ThreeLetterName = "bng", EnglishName = "Bangla"},
                                   new Language {Name = "ba", NativeName = "Башҡорт", ThreeLetterName = "bak", EnglishName = "Bashkir"},
                                   new Language {Name = "eu", NativeName = "euskara", ThreeLetterName = "eus", EnglishName = "Basque"},
                                   new Language {Name = "be", NativeName = "Беларуская", ThreeLetterName = "bel", EnglishName = "Belarusian"},
                                   new Language {Name = "bs", NativeName = "bosanski", ThreeLetterName = "bsb", EnglishName = "Bosnian"},
                                   new Language {Name = "bs-Cyrl", NativeName = "босански", ThreeLetterName = "bsc", EnglishName = "Bosnian (Cyrillic)"},
                                   new Language {Name = "bs-Latn", NativeName = "bosanski", ThreeLetterName = "bsb", EnglishName = "Bosnian (Latin)"},
                                   new Language {Name = "br", NativeName = "brezhoneg", ThreeLetterName = "bre", EnglishName = "Breton"},
                                   new Language {Name = "bg", NativeName = "български", ThreeLetterName = "bul", EnglishName = "Bulgarian"},
                                   new Language {Name = "ca", NativeName = "Català", ThreeLetterName = "cat", EnglishName = "Catalan"},
                                   new Language {Name = "tzm-Latn", NativeName = "Tamazight", ThreeLetterName = "tzm", EnglishName = "Central Atlas Tamazight (Latin)"},
                                   new Language {Name = "tzm-Tfng", NativeName = "ⵜⴰⵎⴰⵣⵉⵖⵜ", ThreeLetterName = "tzm", EnglishName = "Central Atlas Tamazight (Tifinagh)"},
                                   new Language {Name = "ku", NativeName = "کوردیی ناوەڕاست", ThreeLetterName = "kur", EnglishName = "Central Kurdish"},
                                   new Language {Name = "ku-Arab", NativeName = "کوردیی ناوەڕاست", ThreeLetterName = "kur", EnglishName = "Central Kurdish"},
                                   new Language {Name = "chr", NativeName = "ᏣᎳᎩ", ThreeLetterName = "chr", EnglishName = "Cherokee"},
                                   new Language {Name = "chr-Cher", NativeName = "ᏣᎳᎩ", ThreeLetterName = "chr", EnglishName = "Cherokee"},
                                   new Language {Name = "zh", NativeName = "中文", ThreeLetterName = "zho", EnglishName = "Chinese"},
                                   new Language {Name = "zh-Hans", NativeName = "中文(简体)", ThreeLetterName = "zho", EnglishName = "Chinese (Simplified)"},
                                   new Language {Name = "zh-CHS", NativeName = "中文(简体) 旧版", ThreeLetterName = "zho", EnglishName = "Chinese (Simplified) Legacy"},
                                   new Language {Name = "zh-Hant", NativeName = "中文(繁體)", ThreeLetterName = "zho", EnglishName = "Chinese (Traditional)"},
                                   new Language {Name = "zh-CHT", NativeName = "中文(繁體) 舊版", ThreeLetterName = "zho", EnglishName = "Chinese (Traditional) Legacy"},
                                   new Language {Name = "co", NativeName = "Corsu", ThreeLetterName = "cos", EnglishName = "Corsican"},
                                   new Language {Name = "hr", NativeName = "hrvatski", ThreeLetterName = "hrv", EnglishName = "Croatian"},
                                   new Language {Name = "cs", NativeName = "čeština", ThreeLetterName = "ces", EnglishName = "Czech"},
                                   new Language {Name = "da", NativeName = "dansk", ThreeLetterName = "dan", EnglishName = "Danish"},
                                   new Language {Name = "prs", NativeName = "درى", ThreeLetterName = "prs", EnglishName = "Dari"},
                                   new Language {Name = "dv", NativeName = "ދިވެހިބަސް", ThreeLetterName = "div", EnglishName = "Divehi"},
                                   new Language {Name = "nl", NativeName = "Nederlands", ThreeLetterName = "nld", EnglishName = "Dutch"},
                                   new Language {Name = "en", NativeName = "English", ThreeLetterName = "eng", EnglishName = "English"},
                                   new Language {Name = "et", NativeName = "eesti", ThreeLetterName = "est", EnglishName = "Estonian"},
                                   new Language {Name = "fo", NativeName = "føroyskt", ThreeLetterName = "fao", EnglishName = "Faroese"},
                                   new Language {Name = "fil", NativeName = "Filipino", ThreeLetterName = "fil", EnglishName = "Filipino"},
                                   new Language {Name = "fi", NativeName = "suomi", ThreeLetterName = "fin", EnglishName = "Finnish"},
                                   new Language {Name = "fr", NativeName = "français", ThreeLetterName = "fra", EnglishName = "French"},
                                   new Language {Name = "fy", NativeName = "Frysk", ThreeLetterName = "fry", EnglishName = "Frisian"},
                                   new Language {Name = "ff", NativeName = "Fulah", ThreeLetterName = "ful", EnglishName = "Fulah"},
                                   new Language {Name = "ff-Latn", NativeName = "Fulah", ThreeLetterName = "ful", EnglishName = "Fulah"},
                                   new Language {Name = "gl", NativeName = "galego", ThreeLetterName = "glg", EnglishName = "Galician"},
                                   new Language {Name = "ka", NativeName = "ქართული", ThreeLetterName = "kat", EnglishName = "Georgian"},
                                   new Language {Name = "de", NativeName = "Deutsch", ThreeLetterName = "deu", EnglishName = "German"},
                                   new Language {Name = "el", NativeName = "Ελληνικά", ThreeLetterName = "ell", EnglishName = "Greek"},
                                   new Language {Name = "kl", NativeName = "kalaallisut", ThreeLetterName = "kal", EnglishName = "Greenlandic"},
                                   new Language {Name = "gu", NativeName = "ગુજરાતી", ThreeLetterName = "guj", EnglishName = "Gujarati"},
                                   new Language {Name = "ha", NativeName = "Hausa", ThreeLetterName = "hau", EnglishName = "Hausa"},
                                   new Language {Name = "ha-Latn", NativeName = "Hausa", ThreeLetterName = "hau", EnglishName = "Hausa (Latin)"},
                                   new Language {Name = "haw", NativeName = "Hawaiʻi", ThreeLetterName = "haw", EnglishName = "Hawaiian"},
                                   new Language {Name = "he", NativeName = "עברית", ThreeLetterName = "heb", EnglishName = "Hebrew"},
                                   new Language {Name = "hi", NativeName = "हिंदी", ThreeLetterName = "hin", EnglishName = "Hindi"},
                                   new Language {Name = "hu", NativeName = "magyar", ThreeLetterName = "hun", EnglishName = "Hungarian"},
                                   new Language {Name = "is", NativeName = "íslenska", ThreeLetterName = "isl", EnglishName = "Icelandic"},
                                   new Language {Name = "ig", NativeName = "Igbo", ThreeLetterName = "ibo", EnglishName = "Igbo"},
                                   new Language {Name = "id", NativeName = "Bahasa Indonesia", ThreeLetterName = "ind", EnglishName = "Indonesian"},
                                   new Language {Name = "iu", NativeName = "Inuktitut", ThreeLetterName = "iku", EnglishName = "Inuktitut"},
                                   new Language {Name = "iu-Latn", NativeName = "Inuktitut", ThreeLetterName = "iku", EnglishName = "Inuktitut (Latin)"},
                                   new Language {Name = "iu-Cans", NativeName = "ᐃᓄᒃᑎᑐᑦ", ThreeLetterName = "iku", EnglishName = "Inuktitut (Syllabics)"},
                                   new Language {Name = "", NativeName = "Invariant Language (Invariant Country)", ThreeLetterName = "ivl", EnglishName = "Invariant Language (Invariant Country)"},
                                   new Language {Name = "ga", NativeName = "Gaeilge", ThreeLetterName = "gle", EnglishName = "Irish"},
                                   new Language {Name = "xh", NativeName = "isiXhosa", ThreeLetterName = "xho", EnglishName = "isiXhosa"},
                                   new Language {Name = "zu", NativeName = "isiZulu", ThreeLetterName = "zul", EnglishName = "isiZulu"},
                                   new Language {Name = "it", NativeName = "italiano", ThreeLetterName = "ita", EnglishName = "Italian"},
                                   new Language {Name = "ja", NativeName = "日本語", ThreeLetterName = "jpn", EnglishName = "Japanese"},
                                   new Language {Name = "kn", NativeName = "ಕನ್ನಡ", ThreeLetterName = "kan", EnglishName = "Kannada"},
                                   new Language {Name = "kk", NativeName = "Қазақ", ThreeLetterName = "kaz", EnglishName = "Kazakh"},
                                   new Language {Name = "km", NativeName = "ភាសាខ្មែរ", ThreeLetterName = "khm", EnglishName = "Khmer"},
                                   new Language {Name = "qut", NativeName = "K'iche'", ThreeLetterName = "qut", EnglishName = "K'iche'"},
                                   new Language {Name = "rw", NativeName = "Kinyarwanda", ThreeLetterName = "kin", EnglishName = "Kinyarwanda"},
                                   new Language {Name = "sw", NativeName = "Kiswahili", ThreeLetterName = "swa", EnglishName = "Kiswahili"},
                                   new Language {Name = "kok", NativeName = "कोंकणी", ThreeLetterName = "kok", EnglishName = "Konkani"},
                                   new Language {Name = "ko", NativeName = "한국어", ThreeLetterName = "kor", EnglishName = "Korean"},
                                   new Language {Name = "ky", NativeName = "Кыргыз", ThreeLetterName = "kir", EnglishName = "Kyrgyz"},
                                   new Language {Name = "lo", NativeName = "ພາສາລາວ", ThreeLetterName = "lao", EnglishName = "Lao"},
                                   new Language {Name = "lv", NativeName = "latviešu", ThreeLetterName = "lav", EnglishName = "Latvian"},
                                   new Language {Name = "lt", NativeName = "lietuvių", ThreeLetterName = "lit", EnglishName = "Lithuanian"},
                                   new Language {Name = "dsb", NativeName = "dolnoserbšćina", ThreeLetterName = "dsb", EnglishName = "Lower Sorbian"},
                                   new Language {Name = "lb", NativeName = "Lëtzebuergesch", ThreeLetterName = "ltz", EnglishName = "Luxembourgish"},
                                   new Language {Name = "mk", NativeName = "македонски јазик", ThreeLetterName = "mkd", EnglishName = "Macedonian (Former Yugoslav Republic of Macedonia)"},
                                   new Language {Name = "ms", NativeName = "Bahasa Melayu", ThreeLetterName = "msa", EnglishName = "Malay"},
                                   new Language {Name = "ml", NativeName = "മലയാളം", ThreeLetterName = "mym", EnglishName = "Malayalam"},
                                   new Language {Name = "mt", NativeName = "Malti", ThreeLetterName = "mlt", EnglishName = "Maltese"},
                                   new Language {Name = "mi", NativeName = "Reo Māori", ThreeLetterName = "mri", EnglishName = "Maori"},
                                   new Language {Name = "arn", NativeName = "Mapudungun", ThreeLetterName = "arn", EnglishName = "Mapudungun"},
                                   new Language {Name = "mr", NativeName = "मराठी", ThreeLetterName = "mar", EnglishName = "Marathi"},
                                   new Language {Name = "moh", NativeName = "Kanien'kéha", ThreeLetterName = "moh", EnglishName = "Mohawk"},
                                   new Language {Name = "mn", NativeName = "Монгол хэл", ThreeLetterName = "mon", EnglishName = "Mongolian"},
                                   new Language {Name = "mn-Cyrl", NativeName = "Монгол хэл", ThreeLetterName = "mon", EnglishName = "Mongolian (Cyrillic)"},
                                   new Language {Name = "mn-Mong", NativeName = "ᠮᠤᠨᠭᠭᠤᠯ ᠬᠡᠯᠡ", ThreeLetterName = "mon", EnglishName = "Mongolian (Traditional Mongolian)"},
                                   new Language {Name = "ne", NativeName = "नेपाली", ThreeLetterName = "nep", EnglishName = "Nepali"},
                                   new Language {Name = "no", NativeName = "norsk", ThreeLetterName = "nob", EnglishName = "Norwegian"},
                                   new Language {Name = "nb", NativeName = "norsk (bokmål)", ThreeLetterName = "nob", EnglishName = "Norwegian (Bokmål)"},
                                   new Language {Name = "nn", NativeName = "norsk (nynorsk)", ThreeLetterName = "nno", EnglishName = "Norwegian (Nynorsk)"},
                                   new Language {Name = "oc", NativeName = "Occitan", ThreeLetterName = "oci", EnglishName = "Occitan"},
                                   new Language {Name = "or", NativeName = "ଓଡ଼ିଆ", ThreeLetterName = "ori", EnglishName = "Odia"},
                                   new Language {Name = "ps", NativeName = "پښتو", ThreeLetterName = "pus", EnglishName = "Pashto"},
                                   new Language {Name = "fa", NativeName = "فارسى", ThreeLetterName = "fas", EnglishName = "Persian"},
                                   new Language {Name = "pl", NativeName = "polski", ThreeLetterName = "pol", EnglishName = "Polish"},
                                   new Language {Name = "pt", NativeName = "Português", ThreeLetterName = "por", EnglishName = "Portuguese"},
                                   new Language {Name = "pa", NativeName = "ਪੰਜਾਬੀ", ThreeLetterName = "pan", EnglishName = "Punjabi"},
                                   new Language {Name = "pa-Arab", NativeName = "پنجابی", ThreeLetterName = "pan", EnglishName = "Punjabi"},
                                   new Language {Name = "quz", NativeName = "runasimi", ThreeLetterName = "qub", EnglishName = "Quechua"},
                                   new Language {Name = "ro", NativeName = "română", ThreeLetterName = "ron", EnglishName = "Romanian"},
                                   new Language {Name = "rm", NativeName = "Rumantsch", ThreeLetterName = "roh", EnglishName = "Romansh"},
                                   new Language {Name = "ru", NativeName = "русский", ThreeLetterName = "rus", EnglishName = "Russian"},
                                   new Language {Name = "sah", NativeName = "Саха", ThreeLetterName = "sah", EnglishName = "Sakha"},
                                   new Language {Name = "smn", NativeName = "sämikielâ", ThreeLetterName = "smn", EnglishName = "Sami (Inari)"},
                                   new Language {Name = "smj", NativeName = "julevusámegiella", ThreeLetterName = "smj", EnglishName = "Sami (Lule)"},
                                   new Language {Name = "se", NativeName = "davvisámegiella", ThreeLetterName = "sme", EnglishName = "Sami (Northern)"},
                                   new Language {Name = "sms", NativeName = "sää´mǩiõll", ThreeLetterName = "sms", EnglishName = "Sami (Skolt)"},
                                   new Language {Name = "sma", NativeName = "åarjelsaemiengïele", ThreeLetterName = "sma", EnglishName = "Sami (Southern)"},
                                   new Language {Name = "sa", NativeName = "संस्कृत", ThreeLetterName = "san", EnglishName = "Sanskrit"},
                                   new Language {Name = "gd", NativeName = "Gàidhlig", ThreeLetterName = "gla", EnglishName = "Scottish Gaelic"},
                                   new Language {Name = "sr", NativeName = "srpski", ThreeLetterName = "srp", EnglishName = "Serbian"},
                                   new Language {Name = "sr-Cyrl", NativeName = "српски", ThreeLetterName = "srp", EnglishName = "Serbian (Cyrillic)"},
                                   new Language {Name = "sr-Latn", NativeName = "srpski", ThreeLetterName = "srp", EnglishName = "Serbian (Latin)"},
                                   new Language {Name = "nso", NativeName = "Sesotho sa Leboa", ThreeLetterName = "nso", EnglishName = "Sesotho sa Leboa"},
                                   new Language {Name = "tn", NativeName = "Setswana", ThreeLetterName = "tsn", EnglishName = "Setswana"},
                                   new Language {Name = "sd", NativeName = "سنڌي", ThreeLetterName = "sin", EnglishName = "Sindhi"},
                                   new Language {Name = "sd-Arab", NativeName = "سنڌي", ThreeLetterName = "sin", EnglishName = "Sindhi"},
                                   new Language {Name = "si", NativeName = "සිංහල", ThreeLetterName = "sin", EnglishName = "Sinhala"},
                                   new Language {Name = "sk", NativeName = "slovenčina", ThreeLetterName = "slk", EnglishName = "Slovak"},
                                   new Language {Name = "sl", NativeName = "slovenski", ThreeLetterName = "slv", EnglishName = "Slovenian"},
                                   new Language {Name = "es", NativeName = "español", ThreeLetterName = "spa", EnglishName = "Spanish"},
                                   new Language {Name = "sv", NativeName = "svenska", ThreeLetterName = "swe", EnglishName = "Swedish"},
                                   new Language {Name = "syr", NativeName = "ܣܘܪܝܝܐ", ThreeLetterName = "syr", EnglishName = "Syriac"},
                                   new Language {Name = "tg", NativeName = "Тоҷикӣ", ThreeLetterName = "tgk", EnglishName = "Tajik"},
                                   new Language {Name = "tg-Cyrl", NativeName = "Тоҷикӣ", ThreeLetterName = "tgk", EnglishName = "Tajik (Cyrillic)"},
                                   new Language {Name = "tzm", NativeName = "Tamazight", ThreeLetterName = "tzm", EnglishName = "Tamazight"},
                                   new Language {Name = "ta", NativeName = "தமிழ்", ThreeLetterName = "tam", EnglishName = "Tamil"},
                                   new Language {Name = "tt", NativeName = "Татар", ThreeLetterName = "tat", EnglishName = "Tatar"},
                                   new Language {Name = "te", NativeName = "తెలుగు", ThreeLetterName = "tel", EnglishName = "Telugu"},
                                   new Language {Name = "th", NativeName = "ไทย", ThreeLetterName = "tha", EnglishName = "Thai"},
                                   new Language {Name = "bo", NativeName = "བོད་ཡིག", ThreeLetterName = "bod", EnglishName = "Tibetan"},
                                   new Language {Name = "ti", NativeName = "ትግርኛ", ThreeLetterName = "tir", EnglishName = "Tigrinya"},
                                   new Language {Name = "tr", NativeName = "Türkçe", ThreeLetterName = "tur", EnglishName = "Turkish"},
                                   new Language {Name = "tk", NativeName = "Türkmen dili", ThreeLetterName = "tuk", EnglishName = "Turkmen"},
                                   new Language {Name = "uk", NativeName = "українська", ThreeLetterName = "ukr", EnglishName = "Ukrainian"},
                                   new Language {Name = "hsb", NativeName = "hornjoserbšćina", ThreeLetterName = "hsb", EnglishName = "Upper Sorbian"},
                                   new Language {Name = "ur", NativeName = "اُردو", ThreeLetterName = "urd", EnglishName = "Urdu"},
                                   new Language {Name = "ug", NativeName = "ئۇيغۇرچە", ThreeLetterName = "uig", EnglishName = "Uyghur"},
                                   new Language {Name = "uz", NativeName = "O'zbekcha", ThreeLetterName = "uzb", EnglishName = "Uzbek"},
                                   new Language {Name = "uz-Cyrl", NativeName = "Ўзбекча", ThreeLetterName = "uzb", EnglishName = "Uzbek (Cyrillic)"},
                                   new Language {Name = "uz-Latn", NativeName = "O'zbekcha", ThreeLetterName = "uzb", EnglishName = "Uzbek (Latin)"},
                                   new Language {Name = "vi", NativeName = "Tiếng Việt", ThreeLetterName = "vie", EnglishName = "Vietnamese"},
                                   new Language {Name = "cy", NativeName = "Cymraeg", ThreeLetterName = "cym", EnglishName = "Welsh"},
                                   new Language {Name = "wo", NativeName = "Wolof", ThreeLetterName = "wol", EnglishName = "Wolof"},
                                   new Language {Name = "ii", NativeName = "ꆈꌠꁱꂷ", ThreeLetterName = "iii", EnglishName = "Yi"},
                                   new Language {Name = "yo", NativeName = "Yoruba", ThreeLetterName = "yor", EnglishName = "Yoruba"},
                               };



            return cultures;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }

    [DebuggerDisplay("EnglishName: {EnglishName}")]
    public class Language
    {
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string NativeName { get; set; }
        public string ThreeLetterName { get; set; }
    }
}
