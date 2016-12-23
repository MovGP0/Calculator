using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;
using Serilog.Events;

namespace Calculator.CharacterSelection
{
    public static class CodepageHelper
    {
        private static ILogger Log => Serilog.Log.ForContext(typeof(CodepageHelper));

        public static IEnumerable<Encoding> Codepages
        {
            get
            {
                var codepages = (from encodingInfo in Encoding.GetEncodings()
                                 let encoding = encodingInfo.GetEncoding()
                                 where encodingInfo.Name != encoding.BodyName
                                       || encodingInfo.Name != encoding.HeaderName
                                       || encodingInfo.Name != encoding.WebName
                                 select encoding).ToArray();

                LogFoundEncodings(codepages);

                return codepages;
            }
        }

        private static void LogFoundEncodings(IEnumerable<Encoding> codepages)
        {
            if (!Log.IsEnabled(LogEventLevel.Information)) return;

            var message = codepages.Aggregate(
                new StringBuilder().AppendLine("CodePage           EncodingName                 WebName"),
                (builder, encoding) =>
                    builder.AppendFormat("{0,-15} {1,-25} {2,-25}", encoding.CodePage, encoding.EncodingName,
                        encoding.WebName),
                builder => builder.ToString());

            Log.Information(message);
        }

        public static IEnumerable<char> GetCharactersInCodepage(Encoding codepage)
        {
            var inbytes = new byte[256];
            foreach (var code in Enumerable.Range(0, inbytes.Length - 1))
                inbytes[code] = Convert.ToByte(code);

            var input = Encoding.Default.GetString(inbytes);
            var outbytes = Encoding.Convert(Encoding.Default, codepage, inbytes);
            var convertedbytes = Encoding.Convert(codepage, Encoding.Default, outbytes);
            var output = Encoding.Default.GetString(convertedbytes);

            foreach (var idx in Enumerable.Range(0, input.Length - 1))
            {
                if (input[idx] != output[idx])
                {
                    yield return char.MinValue;
                    continue;
                }
                
                yield return input[idx];
            }
        }
    }
}