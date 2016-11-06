using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Calculator.GestureRecognizer
{
    public static class TrainingSetIo
    {
        public static async Task<TrainingSet> ReadGestureFromXmlAsync(string fileName)
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
        
        public static async Task WriteGestureAsXmlAsync(TrainingSet gesture, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                var serializer = new XmlSerializer(typeof(TrainingSet));
                serializer.Serialize(writer, gesture);
                await writer.FlushAsync();
            }
        }

        public static async Task WriteGestureAsBinaryAsync(TrainingSet trainingSet, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, trainingSet);
                await stream.FlushAsync();
                stream.Close();
            }
        }

        public static async Task<TrainingSet> ReadGestureFromBinaryAsync(string fileName)
        {
            return await Task<TrainingSet>.Factory.StartNew(() =>
            {
                try
                {
                    using (var stream = new FileStream(fileName, FileMode.Open))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        var trainingSet = (TrainingSet) formatter.Deserialize(stream);
                        return trainingSet;
                    }
                }
                catch (FileNotFoundException)
                {
                    return new TrainingSet(new List<Gesture>());
                }
            });
        }
    }
}