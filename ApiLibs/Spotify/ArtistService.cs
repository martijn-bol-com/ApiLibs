﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.General;

namespace ApiLibs.Spotify
{
    public class ArtistService : SubService
    {
        public ArtistService(Service service) : base(service)
        {
        }

        public async Task<Artist> GetArtist(string id)
        {
            return await MakeRequest<Artist>("artists/" + id);
        }

        public async Task<List<Artist>> GetArtists(List<string> ids, RegionInfo info, List<AlbumType> types = null, int? limit = null, int? offset = null)
        {
            return (await MakeRequest<RelatedArtistResult>("artists?ids=" + ids.Aggregate((i,j) => i + "," + j), parameters: new List<Param>
            {
                new Param("country", info.TwoLetterISORegionName),
                new OParam("types", types?.Select(i => i.ToString().ToLower()).Aggregate((i, j) => i + "," + j)),
                new OParam("limit", limit),
                new OParam("offset", offset)
            })).artists;
        }

        public async Task<List<Album>> GetAlbumFromArtist(string artistId)
        {
            return (await MakeRequest<AlbumResultsResponse>("artists/" + artistId + "/albums")).items;
        }

        public async Task<List<Track>> GetTopTracks(string artistId)
        {
            return (await MakeRequest<TrackResultsResponse>("artists/" + artistId + "/top-tracks")).items;
        }

        public async Task<List<Artist>> GetRelatedArtists(string id)
        {
            return (await MakeRequest<RelatedArtistResult>("artists/" + id + "/related-artists")).artists;
        }

        private class RelatedArtistResult
        {
            public List<Artist> artists;
        }
    }

    public enum UserType
    {
        User, Artist
    }

    public enum AlbumType
    {
        Album, Single, Appears_On, Compilation
    }
}
