using System;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace DepremVeriProjesi
{
    class Program
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly string _apiUrl = "https://api.orhanaydogdu.com.tr/deprem/";

        static async Task Main(string[] args)
        {
            try
            {
                // API'den veri çekiyoruz
                HttpResponseMessage response = await _client.GetAsync(_apiUrl);
                response.EnsureSuccessStatusCode(); // Hata kontrolü

                // Gelen yanıtı okuyoruz
                string responseBody = await response.Content.ReadAsStringAsync();

                // Eğer yanıt boşsa bir mesaj yazdırıyoruz
                if (string.IsNullOrEmpty(responseBody))
                {
                    Console.WriteLine("API'den boş veri alındı.");
                    return;
                }

                Console.WriteLine("API'den gelen veri:");
                Console.WriteLine(responseBody);

                // MongoDB'ye kaydetme işlemi
                SaveToDatabase(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine("API çağrısında bir hata oluştu: " + ex.Message);
            }
        }

        private static void SaveToDatabase(string jsonData)
        {
            try
            {
                // MongoDB'ye bağlanıyoruz
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("DepremVeriDB");
                var collection = database.GetCollection<BsonDocument>("Depremler");

                // JSON verisini BsonDocument olarak parse ediyoruz
                var document = BsonDocument.Parse(jsonData);

                // Veriyi MongoDB'ye kaydediyoruz
                collection.InsertOne(document);

                Console.WriteLine("Veri başarıyla MongoDB'ye kaydedildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("MongoDB'ye veri kaydederken bir hata oluştu: " + ex.Message);
            }
        }
    }
}
