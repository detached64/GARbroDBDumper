using GameRes;
using GameRes.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SchemeDumper
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Dumping...");
            using (Stream stream = File.OpenRead(".\\GameData\\Formats.dat"))
            {
                int version = FormatCatalog.Instance.GetSerializedSchemeVersion(stream);
                using (ZLibStream zs = new ZLibStream(stream, CompressionMode.Decompress, true))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    SchemeDataBase db = bin.Deserialize(zs) as SchemeDataBase;
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        Formatting = Formatting.Indented,
                        ContractResolver = new DefaultContractResolver
                        {
                            SerializeCompilerGeneratedMembers = true,
                            NamingStrategy = new CamelCaseNamingStrategy()
                        },
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    };
                    string json = JsonConvert.SerializeObject(db, settings);
                    File.WriteAllText(".\\GameData\\Formats.json", json);
                }
            }
            Console.WriteLine("Scheme dumped.");
            Console.ReadLine();
        }
    }
}
