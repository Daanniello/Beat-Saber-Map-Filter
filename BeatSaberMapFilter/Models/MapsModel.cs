using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;


namespace BeatSaberMapFilter.Models
{
    public partial class Map
    {
        [JsonProperty("diffs")]
        public DiffElement[] Diffs { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("mapper")]
        public string Mapper { get; set; }

        [JsonProperty("song")]
        public string Song { get; set; }

        [JsonProperty("bpm")]
        public long Bpm { get; set; }

        [JsonProperty("downloadCount")]
        public long DownloadCount { get; set; }

        [JsonProperty("upVotes")]
        public long UpVotes { get; set; }

        [JsonProperty("downVotes")]
        public long DownVotes { get; set; }

        [JsonProperty("heat")]
        public double Heat { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        //[JsonProperty("automapper")]
        //public AutomapperUnion Automapper { get; set; }

        [JsonProperty("uploaddate")]
        public DateTimeOffset? Uploaddate { get; set; }
    }

    public partial class DiffElement
    {
        public string KeyCode { get; set; }

        [JsonProperty("diff")]
        public string Diff { get; set; }


        [JsonProperty("njs")]
        public long Njs { get; set; }

        public long Nps { get; set; }

        public long BPM { get; set; }

        [JsonProperty("scores")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Scores { get; set; }


        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("len")]
        public long Length { get; set; }

        [JsonProperty("njt")]
        public long Njt { get; set; }

        [JsonProperty("bmb")]
        public long Bombs { get; set; }

        [JsonProperty("nts")]
        public long Notes { get; set; }

        [JsonProperty("obs")]
        public long Obstacles { get; set; }

        [JsonProperty("pp")]
        public string Pp { get; set; }

        [JsonProperty("star")]
        public string Star { get; set; }

        public long DownloadCount { get; set; }
        public long Upvotes { get; set; }
        public long Downvotes { get; set; }
        public double Rating { get; set; }
        public double Heat { get; set; }
        public DateTimeOffset? UploadDate { get; set; }

        public bool IsRanked = false;
    }

    public enum AutomapperEnum { Beatsage, Deepsaber, Lolighter };

    public enum DiffEnum { Easy, Normal, Hard, Expert, ExpertPlus };

    public partial struct AutomapperUnion
    {
        public AutomapperEnum? Enum;
        public long? Integer;

        public static implicit operator AutomapperUnion(AutomapperEnum Enum) => new AutomapperUnion { Enum = Enum };
        public static implicit operator AutomapperUnion(long Integer) => new AutomapperUnion { Integer = Integer };
        public bool IsNull => Integer == null && Enum == null;
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                AutomapperUnionConverter.Singleton,
                AutomapperEnumConverter.Singleton,
                DiffEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class AutomapperUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AutomapperUnion) || t == typeof(AutomapperUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return new AutomapperUnion { };
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    switch (stringValue)
                    {
                        case "beatsage":
                            return new AutomapperUnion { Enum = AutomapperEnum.Beatsage };
                        case "deepsaber":
                            return new AutomapperUnion { Enum = AutomapperEnum.Deepsaber };
                        case "lolighter":
                            return new AutomapperUnion { Enum = AutomapperEnum.Lolighter };
                    }
                    long l;
                    if (Int64.TryParse(stringValue, out l))
                    {
                        return new AutomapperUnion { Integer = l };
                    }
                    break;
            }
            throw new Exception("Cannot unmarshal type AutomapperUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (AutomapperUnion)untypedValue;
            if (value.IsNull)
            {
                serializer.Serialize(writer, null);
                return;
            }
            if (value.Enum != null)
            {
                switch (value.Enum)
                {
                    case AutomapperEnum.Beatsage:
                        serializer.Serialize(writer, "beatsage");
                        return;
                    case AutomapperEnum.Deepsaber:
                        serializer.Serialize(writer, "deepsaber");
                        return;
                    case AutomapperEnum.Lolighter:
                        serializer.Serialize(writer, "lolighter");
                        return;
                }
            }
            if (value.Integer != null)
            {
                serializer.Serialize(writer, value.Integer.Value.ToString());
                return;
            }
            throw new Exception("Cannot marshal type AutomapperUnion");
        }

        public static readonly AutomapperUnionConverter Singleton = new AutomapperUnionConverter();
    }

    internal class AutomapperEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AutomapperEnum) || t == typeof(AutomapperEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "beatsage":
                    return AutomapperEnum.Beatsage;
                case "deepsaber":
                    return AutomapperEnum.Deepsaber;
                case "lolighter":
                    return AutomapperEnum.Lolighter;
            }
            throw new Exception("Cannot unmarshal type AutomapperEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (AutomapperEnum)untypedValue;
            switch (value)
            {
                case AutomapperEnum.Beatsage:
                    serializer.Serialize(writer, "beatsage");
                    return;
                case AutomapperEnum.Deepsaber:
                    serializer.Serialize(writer, "deepsaber");
                    return;
                case AutomapperEnum.Lolighter:
                    serializer.Serialize(writer, "lolighter");
                    return;
            }
            throw new Exception("Cannot marshal type AutomapperEnum");
        }

        public static readonly AutomapperEnumConverter Singleton = new AutomapperEnumConverter();
    }

    internal class DiffEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DiffEnum) || t == typeof(DiffEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Easy":
                    return DiffEnum.Easy;
                case "Expert":
                    return DiffEnum.Expert;
                case "Expert+":
                    return DiffEnum.ExpertPlus;
                case "Hard":
                    return DiffEnum.Hard;
                case "Normal":
                    return DiffEnum.Normal;
            }
            throw new Exception("Cannot unmarshal type DiffEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DiffEnum)untypedValue;
            switch (value)
            {
                case DiffEnum.Easy:
                    serializer.Serialize(writer, "Easy");
                    return;
                case DiffEnum.Expert:
                    serializer.Serialize(writer, "Expert");
                    return;
                case DiffEnum.ExpertPlus:
                    serializer.Serialize(writer, "Expert+");
                    return;
                case DiffEnum.Hard:
                    serializer.Serialize(writer, "Hard");
                    return;
                case DiffEnum.Normal:
                    serializer.Serialize(writer, "Normal");
                    return;
            }
            throw new Exception("Cannot marshal type DiffEnum");
        }

        public static readonly DiffEnumConverter Singleton = new DiffEnumConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}



