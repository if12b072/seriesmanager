using SeriesManager.Enum;

namespace SeriesManager.Extension
{
    public static class LanguageExtensions
    {
        public static string ToShort(this Languages val)
        {
            switch (val)
            {
                case Languages.Chinese:
                    return "zh";
                case Languages.Croatian:
                    return "hr";
                case Languages.Czech:
                    return "cs";
                case Languages.Dansk:
                    return "da";
                case Languages.Deutsch:
                    return "de";
                case Languages.English:
                    return "en";
                case Languages.Español:
                    return "es";
                case Languages.Français:
                    return "fr";
                case Languages.Greek:
                    return "el";
                case Languages.Hebrew:
                    return "he";
                case Languages.Italiano:
                    return "it";
                case Languages.Japanese:
                    return "ja";
                case Languages.Korean:
                    return "ko";
                case Languages.Magyar:
                    return "hu";
                case Languages.Nederlands:
                    return "nl";
                case Languages.Norsk:
                    return "no";
                case Languages.Polski:
                    return "pl";
                case Languages.Portuguese:
                    return "pt";
                case Languages.Russian:
                    return "ru";
                case Languages.Slovenian:
                    return "sl";
                case Languages.Suomeksi:
                    return "fi";
                case Languages.Svenska:
                    return "sv";
                case Languages.Turkish:
                    return "tr";
                default:
                    return "en";
            }
        }

        public static int ToID(this Languages val)
        {
            switch (val)
            {
                case Languages.Chinese:
                    return 27;
                case Languages.Croatian:
                    return 31;
                case Languages.Czech:
                    return 28;
                case Languages.Dansk:
                    return 10;
                case Languages.Deutsch:
                    return 14;
                case Languages.English:
                    return 7;
                case Languages.Español:
                    return 16;
                case Languages.Français:
                    return 17;
                case Languages.Greek:
                    return 20;
                case Languages.Hebrew:
                    return 24;
                case Languages.Italiano:
                    return 15;
                case Languages.Japanese:
                    return 25;
                case Languages.Korean:
                    return 32;
                case Languages.Magyar:
                    return 19;
                case Languages.Nederlands:
                    return 13;
                case Languages.Norsk:
                    return 9;
                case Languages.Polski:
                    return 18;
                case Languages.Portuguese:
                    return 26;
                case Languages.Russian:
                    return 22;
                case Languages.Slovenian:
                    return 30;
                case Languages.Suomeksi:
                    return 11;
                case Languages.Svenska:
                    return 8;
                case Languages.Turkish:
                    return 21;
                default:
                    return 7;
            }
        }

        public static Languages ToLanguage(this string val)
        {
            switch (val)
            {
                case "zh":
                    return Languages.Chinese;
                case "hr":
                    return Languages.Croatian;
                case "cs":
                    return Languages.Czech;
                case "da":
                    return Languages.Dansk;
                case "de":
                    return Languages.Deutsch;
                case "en":
                    return Languages.English;
                case "es":
                    return Languages.Español;
                case "fr":
                    return Languages.Français;
                case "el":
                    return Languages.Greek;
                case "he":
                    return Languages.Hebrew;
                case "it":
                    return Languages.Italiano;
                case "ja":
                    return Languages.Japanese;
                case "ko":
                    return Languages.Korean;
                case "hu":
                    return Languages.Magyar;
                case "nl":
                    return Languages.Nederlands;
                case "no":
                    return Languages.Norsk;
                case "pl":
                    return Languages.Polski;
                case "pt":
                    return Languages.Portuguese;
                case "ru":
                    return Languages.Russian;
                case "sl":
                    return Languages.Slovenian;
                case "fi":
                    return Languages.Suomeksi;
                case "sv":
                    return Languages.Svenska;
                case "tr":
                    return Languages.Turkish;
                default:
                    return Languages.English;
            }
        }
    }
}
