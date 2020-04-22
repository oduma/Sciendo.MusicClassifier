using Sciendo.ArtistEnhancer.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.ArtistEnhancer.KnowledgeBaseProvider.Generators
{
    public class KnowledgeBaseLoaded
    {
        public Dictionary<string, string> LatinAlphabetTransformations = new Dictionary<string, string>
            {
                {"á","a" },
                {"à","a" },
                {"ä", "a" },
                {"Å","a" },
                {"â","a" },
                {"ã","a" },
                {"å","a" },
                {"æ","a" },
                {"ß","s" },
                {"č","c" },
                {"ć", "c" },
                {"Ç", "c" },
                {"é", "e" },
                {"è","e" },
                {"ë", "e" },
                {"Ê", "e" },
                {"ė","e" },
                {"ę","e" },
                {"ğ", "g" },
                {"í","i" },
                {"ï","i" },
                {"Î","i" },
                {"ñ","n" },
                {"ń","n" },
                {"ó","o" },
                {"ò","o" },
                {"Ö","o" },
                {"ø","o" },
                {"ô","o" },
                {"ō","o" },
                {"ř","r" },
                {"š","s" },
                { "ș","s"},
                {"$","s" },
                {"ü","u" },
                {"ú","u" },
                {"ū","u" },
                {"ý","y" },
                {"Λ","&" },
                {"�","i" },
            };

        public Dictionary<string,string> Noise = new Dictionary<string, string>
        {
            { @"\[",", " },
            {@"\]",", " },
            {@"\|",", " },
            {@"=",", " },
            {@"\""","" },
            {@"'","" },
            {@"„(.*?)“","" },
            {@"<br\s*\/?>",", " },
            {@"\\u00fc","u" },
            {@"\u00e4","a" },
            {@"\\u201e(.*?)\\u201c","" },
            {@"(?:^|\W)akkordeon(?:$|\W)",", " },
            {@"(?:^|\W)bass(?:$|\W)",", " },
            {@"(?:^|\W)begleitgesang(?:$|\W)",", " },
            {@"(?:^|\W)conga(?:$|\W)",", " },
            {@"(?:^|\W)gastmusiker(?:$|\W)",", " },
            {@"(?:^|\W)geige(?:$|\W)",", " },
            {@"(?:^|\W)gesang(?:$|\W)",", " },
            {@"(?:^|\W)gitarre(?:$|\W)",", " },
            {@"(?:^|\W)kontrabass(?:$|\W)",", " },
            {@"(?:^|\W)schlagzeug(?:$|\W)",", " },
            {@"(?:^|\W)stimme(?:$|\W)",", " },
            {@"(?:^|\W)trompete(?:$|\W)",", " },
            {@"(?:^|\W)violine(?:$|\W)",", " },


        };

        public Dictionary<string, string[]> MembersMarkersByLanguages = new Dictionary<string, string[]>
        {
            {
                "en", new []
                {
                    @"(?<=current_members)(.*?)(?=\n)",
                    @"(?<=current_members)(.*?)(?=\n}}\n)",
                    @"(?<=past_members)(.*?)(?=}\n)",
                    @"(?<=past_members)(.*?)(?=\n)",
                    @"(?<=Members ===\n)(.*?)(?=\n\n== Discography)",
                    @"(?<=current_members)(.*?)(?=\\n)",
                    @"(?<=past_members)(.*?)(?=\\n)",
                    @"(?<=\n;former members)(.*?)(?=\n{{col-end}})",
                    @"(?<=\[\[musician\|artists]])(.*?)(?=\.)",
                    //@"(?<=band members==)(.*?)(?=\n\n==Discography)",
                }
            },
            {
                "de", new []
                {
                    @"(?<=Besetzung)(.*?)(?=\n)",
                    //@"(?<=| Gr\u00fcnder)(.*?)(?=\n)",
                    @"(?<=Mitglieder ==\n)(.*?)(?=\n\n== Diskografie)",
                    @"(?<=Mitglieder ==\n)(.*?)(?=\n\n== Geschichte)",
                    @"(?<=Bandmitglieder ==\n)(.*?)(?=\n\n== Diskografie)",
                    //@"(?<=Ehemalige )(.*?)(?=\n)"
                }
            },
            {
                "fr", new []
                {
                    "(?<=membres actuels)(.*?)(?=\\n)",
                    @"(?<=ex membres)(.*?)(?=\\n)",
                    @"(?<=Composition du groupe ==\n)(.*?)(?=\n\n\n\n\n\n)",
                    @"(?<=Membres ==\\n)(.*?)(?=\n\n\n== Discographie)",
                    @"(?<=Musiciens==\\n)(.*?)(?=\n\n)",
                    @"(?<=Membres originaux ===\n\n)(.*?)(?=\n\n)",
                    @"(?<=Membres actuels ===\n\n)(.*?)(?=\n\n)"
                }
            },
            {
                "pt", new []
                {
                    @"(?<=integrantes)(.*?)(?=\n)",
                    @"(?<=exintegrantes)(.*?)(?=\n)"
                }
            },
            {
                "es", new []
                {
                    @"(?<=Miembros)(.*?)(?=\n)",
                    @"(?<=Miembros antiguos)(.*?)(?=\n)",
                    @"(?<=miembros)(.*?)(?=\n)",
                    @"(?<=Otros_miembros)(.*?)(?=\n)",
                    @"(?<=Antiguos miembros)(.*?)(?=\n)"
                }
            },
            {
                "sv", new[]
                {
                    @"(?<= medlemmar)(.*?)(?=\n)"
                }
            }
        };
    }
}
