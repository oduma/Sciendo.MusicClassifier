using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider.Generators
{
    public class ExcludesLoaded
    {

        public string PlaceholderAlbumArtists = "various artists";


        //the words: by, and, &, with, con, radio, original, part, main, x usefull for featured artists  
        public string[] WordsSeparatorsGlobal = new[]
        {
                "arr.",
                "ch",
                "demo",
                "edit",
                "extended",
                "featuring",
                "instrumental",
                "feat",
                "feat.",
                "ft.",
                "mix",
                "mixed",
                "orch.",
                "presents",
                "pres.",
                "prod.",
                "remix",
                "remixed",
                "theme",
                "version",
                "vocal",
                "vocals",
                "vs",
                "vs.",
                "✖",
            };

        public string[] ArtistsForSplitting = new[]
        {
                "2pac",
                "alvin pleasant delaney carter",
                "anna of the north",
                "antonio carles marques pinto",
                "aram mp3",
                "b l a c k i e",
                "fade",
                "jennifer evans van der harten",
                "john 5",
                @"luther ""guitar junior"" johnson",
                "maria isabel garcia asensio",
                "michiel van der kuy",
                "Possessed by Paul James",
                "the reverend peyton",
                "Tyler, The Creator",
            };

        public string[] BandsForSplitting = new[]
        {
                "+-",
                "AC/DC",
                "AC; DC",
                "angus & julia stone",
                "agnelli & nelson",
                "Artist Vs Poet",
                "Benny Turner and Real Blues",
                "between the buried and me",
                "bill and belle reed",
                "bizkit with butta on the keytar",
                "The Black & White Years",
                "Blood, Sweat & Tears",
                "Bohren & Der Club Of Gore",
                "Crosby, Stills, Nash & Young",
                "Drop Dead, Gorgeous",
                "Earth, Wind & Fire",
                "Elana James and Her Hot, Hot Trio",
                "Emerson, Lake & Palmer",
                "Good Night & Good Morning",
                "The Good, The Bad and The Queen",
                "I'm a Lion, I'm a Wolf",
                "Kawai, Mal Duo",
                "Kitty, Daisy & Lewis",
                "The Last Artful, Dodgr",
                "Now, Now",
                "Peter, Bjorn and John",
                "Project Jenny, Project Jan",
                "Shy, Low",
                "Stoni Taylor & Miles Of Stones",
                "Tacks, the Boy Disaster",
                "The Thing With Five Eyes",
                "To Live And Die In LA",
                "You, Me, And Everyone We Know",
            };

        public string WellFormattedNumbers = @"(?<!\S)(\d*\.?\d+|\d{1,3}(,\d{3})*(\.\d+)?)(?!\S)";
    }
}
