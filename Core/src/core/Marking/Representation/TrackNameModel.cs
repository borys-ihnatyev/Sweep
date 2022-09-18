﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sweep.Core.Marking.Representation
{
    public partial class TrackNameModel
    {
        public const string OriginalMix = "original mix";
        public const string Remix = "remix";

        public const string Unknown = "Unknown";

        private readonly ISet<string> artists;
        private readonly ISet<string> remixArtists;
        private HashTagModel hashTagModel;
        private string mixType;
        private string title;

        public TrackNameModel()
        {
            hashTagModel = new HashTagModel();
            artists = new SortedSet<string>(StringComparer.InvariantCultureIgnoreCase);
            remixArtists = new SortedSet<string>(StringComparer.InvariantCultureIgnoreCase);
            title = Unknown;
        }

        public bool IsRemix
        {
            get { return (remixArtists.Count > 0) && !string.IsNullOrWhiteSpace(MixType); }
        }


        public string MixType
        {
            get { return mixType; }
            set
            {
                value = EntityTrim(value);
                if (string.IsNullOrWhiteSpace(value))
                    value = IsRemix ? Remix : string.Empty;
                mixType = value;
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                value = EntityTrim(value);

                if (string.IsNullOrWhiteSpace(value))
                    value = Unknown;
                value = value.ToUpperFirstChar();
                title = value;
            }
        }

        public string FullTitle
        {
            get
            {
                if (IsRemix)
                    return string.Format("{0} ({1} {2})", Title, RemixArtistsString, MixType);

                if (remixArtists.Count > 0)
                    return string.Format("{0} ({1})", Title, RemixArtistsString);

                return !string.IsNullOrWhiteSpace(MixType)
                    ? string.Format("{0} ({1})", Title, MixType)
                    : Title;
            }
        }

        public HashTagModel HashTagModel
        {
            get { return new HashTagModel(hashTagModel); }

            set
            {
                if (value == null)
                    value = new HashTagModel();

                if (value == hashTagModel) return;

                hashTagModel = value;
                UpdateMainKey();
            }
        }


        public Key MainKey { get; private set; }

        public string FullName
        {
            get
            {
                var fullName = string.Format("{0} - {1}", ArtistsString, FullTitle);

                if (!hashTagModel.IsEmpty())
                    fullName = string.Format("{0} {1}", fullName, hashTagModel);

                return fullName;
            }
        }

        public string ArtistsString
        {
            get
            {
                return artists.Count > 0
                    ? string.Join(", ", artists).ToTitleCase()
                    : Unknown;
            }
        }

        public string RemixArtistsString
        {
            get { return string.Join(", ", remixArtists).ToTitleCase(); }
        }


        private void UpdateMainKey()
        {
            var keys = (from hashTag in hashTagModel
                where hashTag is KeyHashTag
                select ((KeyHashTag) hashTag).Key).ToArray();

            MainKey = keys.Any() ? keys.First() : null;
        }


        public bool AddArtist(string artist)
        {
            return artists.Add(EntityTrim(artist));
        }

        public int AddArtists(IEnumerable<string> newArtists)
        {
            return newArtists.Select(EntityTrim).Count(artist => artists.Add(artist));
        }

        public bool RemoveArtist(string artist)
        {
            return artists.Remove(EntityTrim(artist));
        }

        public void ClearArtists()
        {
            remixArtists.Clear();
        }


        public bool AddRemixArtist(string artist)
        {
            return remixArtists.Add(EntityTrim(artist));
        }

        public int AddRemixArtists(IEnumerable<string> newArtists)
        {
            return newArtists.Select(EntityTrim).Count(artist => remixArtists.Add(artist));
        }

        public void ClearRemixArtists()
        {
            remixArtists.Clear();
        }

        public bool RemoveRemixArtist(string artist)
        {
            return remixArtists.Remove(EntityTrim(artist));
        }

        public override string ToString()
        {
            return FullName;
        }

        private static string EntityTrim(string value)
        {
            return value == null ? string.Empty : value.Trim(' ', '[', ']', '(', ')');
        }
    }
}
