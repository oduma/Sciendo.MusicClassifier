using Sciendo.ArtistEnhancer.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.ArtistEnhancer.KnowledgeBaseProvider.Generators
{
    public class KnowledgeBaseLoaded
    {
        //public KnowledgeBaseItem<Dictionary<string, string>>[] KnowledgeBaseItems = new KnowledgeBaseItem<Dictionary<string, string>>[]
        //{
        //    //Exclude any specific markdown characters
        //    new KnowledgeBaseItem<Dictionary<string, string>>
        //    {
        //        Priority = 100,
        //        RuleSet = new Dictionary<string, string>
        //        {
        //            {@"\=","" },
        //            {@"\[","" },
        //            {@"\]", "" },
        //            {"}","" },
        //            {"{","" },
        //            {@"\*","" },
        //            {"†","" },
        //            {@"\:","" },
        //            {"''","" },
        //            {"citation needed","" }
        //        }
        //    },
        //    //Exclude any comments and refs
        //    new KnowledgeBaseItem<Dictionary<string, string>>
        //    {
        //        Priority = 150,
        //        RuleSet = new Dictionary<string, string>
        //        {
        //            {@"<!--.*?-->","" },
        //            {@"<ref.*?\/ref>","" },
        //            {@"<ref.*?\/>", "" },
        //            {@"<div.*?>","" },
        //            {@"<\/div>","" },
        //            {@"\*","" },
        //            {@"<center>","" },
        //            {@"<\/center>","" },
        //        }
        //    },
        //    //Transform everything in latin lower case
        //    new KnowledgeBaseItem<Dictionary<string, string>>
        //    {
        //        Priority = 200,
        //        RuleSet = new Dictionary<string, string>
        //        {
        //            {"á","a" },
        //            {"à","a" },
        //            {"ä", "a" },
        //            {"Å","a" },
        //            {"â","a" },
        //            {"ã","a" },
        //            {"å","a" },
        //            {"æ","a" },
        //            {"ß","s" },
        //            {"č","c" },
        //            {"ć", "c" },
        //            {"Ç", "c" },
        //            {"é", "e" },
        //            {"è","e" },
        //            {"ë", "e" },
        //            {"Ê", "e" },
        //            {"ė","e" },
        //            {"ę","e" },
        //            {"ğ", "g" },
        //            {"í","i" },
        //            {"ï","i" },
        //            {"Î","i" },
        //            {"ñ","n" },
        //            {"ń","n" },
        //            {"ó","o" },
        //            {"ò","o" },
        //            {"Ö","o" },
        //            {"ø","o" },
        //            {"ô","o" },
        //            {"ō","o" },
        //            {"ř","r" },
        //            {"š","s" },
        //            {"ș","s"},
        //            {"$","s" },
        //            {"ü","u" },
        //            {"ú","u" },
        //            {"ū","u" },
        //            {"ý","y" },
        //            {"Λ","&" },
        //            {"�","i" },
        //        }
        //    },
        //    //Transform Encoded Html to text
        //    new KnowledgeBaseItem<Dictionary<string, string>>
        //    {
        //        Priority = 250,
        //        RuleSet = new Dictionary<string, string>
        //        {
        //            {"&nbsp", " " },
        //            {@"<\/u>", " " },
        //            {@"<sup><\/sup>", " " },
        //            { "&ndash", " "},
        //            {"citation;", " " }
        //        }
        //    },
        //    //Exclude non artists words
        //    new KnowledgeBaseItem<Dictionary<string, string>>
        //    {
        //        Priority = 300,
        //        RuleSet = new Dictionary<string, string>
        //        {
        //            {"100px", "" },
        //            {"300px", "" },
        //            {"#band","" },
        //            {"#former","" },
        //            {"#lineups","" },
        //            {"#personnel","" },
        //            {"#members","" },
        //            {"acumental","" },
        //            {"allmusic","" },
        //            {"article","" },
        //            {"band","" },
        //            {"below","" },
        //            {"cite","" },
        //            {"classic","" },
        //            {"deceased","" },
        //            {"director","" },
        //            {"disputed","" },
        //            {"film", "" },
        //            {"former","" },
        //            {"lineup","" },
        //            {"list","" },
        //            {"live","" },
        //            {"members","" },
        //            {"music","" },
        //            {"musician", "" },
        //            {"none","" },
        //            {"other","" },
        //            {"permanent","" },
        //            {"personnel","" },
        //            {"plainlist","" },
        //            {"rock","" },
        //            {"rotating","" },
        //            {"section","" },
        //            {"see","" },
        //            {"selecter","" },
        //            {"sweet","" },
        //            {"ublist","" },
        //            {"unbulleted","" },
        //            {"uncredited", ""},
        //            {"version","" },
        //            {"web","" },
        //            {"websiteimbd","" },
        //            {"websiterate","" },
        //            {"your","" },

        //        }
        //    },

        //    //exclude any artists that have non-alpha numeric characters in their names
        //    new KnowledgeBaseItem<Dictionary<string, string>>
        //    {
        //        Priority = 500,
        //        RuleSet = new Dictionary<string, string>
        //        {
        //            {"^((?![a-z]).)*$", "" },
        //        }
        //    }

        //};

        public Dictionary<string,string> Noise = new Dictionary<string, string>
        {
            { @"\[",", " },
            {@"\]",", " },
            {@"\|",", " },
            {@"=",", " },
            {@"\""","" },
            {@"'","" },
            {@"<br\s*\/?>",", " },
            {@"\\u00fc","u" },
            {@"(?:^|\W)akkordeon(?:$|\W)",", " },
            {@"(?:^|\W)geige(?:$|\W)",", " },
            {@"(?:^|\W)gesang(?:$|\W)",", " },
            {@"(?:^|\W)gitarre(?:$|\W)",", " },
            {@"(?:^|\W)kontrabass(?:$|\W)",", " },
            {@"(?:^|\W)stimme(?:$|\W)",", " },
            {@"(?:^|\W)trompete(?:$|\W)",", " },


        };

        public Dictionary<string, string[]> MembersMarkersByLanguages = new Dictionary<string, string[]>
        {
            {
                "en", new []
                {
                    @"(?<=current_members)(.*?)(?=\n)",
                    @"(?<=past_members)(.*?)(?=\n)",
                    @"(?<=Members ===\n)(.*?)(?=\n\n== Discography)",
                    @"(?<=current_members)(.*?)(?=\n)",
                    @"(?<=past_members)(.*?)(?=\n)",
                    @"(?<=Members ===\n)(.*?)(?=\n\n== Discography)"
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
                    @"(?<=Ehemalige)(.*?)(?=\n)"
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
