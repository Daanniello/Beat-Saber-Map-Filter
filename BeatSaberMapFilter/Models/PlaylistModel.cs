using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMapFilter.Models
{

    public partial class PlaylistModel
    {
        [JsonProperty("playlistTitle")]
        public string PlaylistTitle { get; set; }

        [JsonProperty("playlistAuthor")]
        public string PlaylistAuthor { get; set; }

        [JsonProperty("playlistDescription")]
        public string PlaylistDescription { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("songs")]
        public List<Song> Songs { get; set; }
    }

    public partial class Song
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("difficulties")]
        public List<Difficulty> Difficulties { get; set; }
    }

    public partial class Difficulty
    {
        [JsonProperty("characteristic")]
        public Characteristic Characteristic { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public enum Characteristic { Standard };

    internal static class PlaylistModelConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                CharacteristicConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class CharacteristicConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Characteristic) || t == typeof(Characteristic?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Standard")
            {
                return Characteristic.Standard;
            }
            throw new Exception("Cannot unmarshal type Characteristic");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Characteristic)untypedValue;
            if (value == Characteristic.Standard)
            {
                serializer.Serialize(writer, "Standard");
                return;
            }
            throw new Exception("Cannot marshal type Characteristic");
        }

        public static readonly CharacteristicConverter Singleton = new CharacteristicConverter();
    }
}


