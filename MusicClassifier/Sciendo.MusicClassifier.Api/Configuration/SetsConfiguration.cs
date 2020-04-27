using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Configuration
{
    public class SetsConfiguration
    {
        public string SetsLocation { get; set; }

        public Dictionary<SetFileType, string> SetFiles { get; set; }
    }

    public enum SetFileType
    {
        None,
        Artist,
        Album,
        Track,
        AlbumTrack,
        ArtistArtist,
        ArtistTrack,
        ComposerTrack,
        FeaturedArtistTrack,
        Tags
    }
}
