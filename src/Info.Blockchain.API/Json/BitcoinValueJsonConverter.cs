using System;
using Info.Blockchain.API.Models;
using Newtonsoft.Json;

namespace Info.Blockchain.API.Json
{
	internal class BitcoinValueJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(BitcoinValue);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (!(reader.Value is long)) return BitcoinValue.Zero;
			var satoshis = (long)reader.Value;
			var bitcoinValue = BitcoinValue.FromSatoshis(satoshis);
			return bitcoinValue;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			string satoshis;
			satoshis = value is BitcoinValue ? ((BitcoinValue)value).Satoshis.ToString() : "0";
			writer.WriteRawValue(satoshis);
		}
	}
}
