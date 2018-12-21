using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp5
{
    public class Producer
    {

        public Subject<Quote> PriceStream { get; set; } = new Subject<Quote>();

        public async Task StartProducing(string symbol, double startPrice)
        {
            var rnd = new Random();

            var interval = TimeSpan.FromSeconds(1);
            var r = new Random();
            double cl = 0;

            while (true)
            {
                var intervalStart = DateTime.Now;
                double h = cl == 0 ? startPrice : cl;
                double l = cl == 0 ? startPrice : cl;
                double c = cl == 0 ? startPrice : cl;
                double o = 0;


                while (DateTime.Now - intervalStart < interval)
                {
                    await Task.Delay(5);

                    c = c + (double)r.Next(-100, 100) / 100;
                    l = Math.Min(l, c);
                    h = Math.Max(h, c);

                    var q = new Quote { GlobalQuote = new GlobalQuote { The01Symbol = symbol, The02Open = o, The03High = h, The04Low = l, The05Price = c, The08PreviousClose = cl } };

                    PriceStream.OnNext(q);

                }
                cl = c;
            }

           
        }


    }

    public partial class Quote
    {
        [JsonProperty("Global Quote")]
        public GlobalQuote GlobalQuote { get; set; }
    }

    public partial class GlobalQuote
    {
        [JsonProperty("01. symbol")]
        public string The01Symbol { get; set; }

        [JsonProperty("02. open")]
        public double The02Open { get; set; }

        [JsonProperty("03. high")]
        public double The03High { get; set; }

        [JsonProperty("04. low")]
        public double The04Low { get; set; }

        [JsonProperty("05. price")]
        public double The05Price { get; set; }

        [JsonProperty("06. volume")]
        [JsonConverter(typeof(ParseStringConverter))]
        public double The06Volume { get; set; }

        [JsonProperty("07. latest trading day")]
        public DateTimeOffset The07LatestTradingDay { get; set; }

        [JsonProperty("08. previous close")]
        public double The08PreviousClose { get; set; }

        [JsonProperty("09. change")]
        public double The09Change { get; set; }

        [JsonProperty("10. change percent")]
        public double The10ChangePercent { get; set; }

        public override int GetHashCode()
        {
            return this.The01Symbol.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var quote = obj as GlobalQuote;
            if (quote == null) return false;

            return this.The01Symbol == quote.The01Symbol;
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
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
