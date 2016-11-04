using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Calculator.GestureRecognizer
{
    public static class TrainingSetIo
    {
        public static async Task<TrainingSet> ReadGestureAsync(string fileName)
        {
            return await Task<TrainingSet>.Factory.StartNew(() =>
            {
                using (var stream = new FileStream(fileName, FileMode.Open))
                using (var reader = new StreamReader(stream))
                {
                    var serializer = new XmlSerializer(typeof(Gesture));
                    var gesture = (TrainingSet) serializer.Deserialize(reader);
                    return gesture;
                }
            });
        }
        
        public static async Task WriteGestureAsync(TrainingSet gesture, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                var serializer = new XmlSerializer(typeof(TrainingSet));
                serializer.Serialize(writer, gesture);
                await writer.FlushAsync();
            }
        }
    }
}