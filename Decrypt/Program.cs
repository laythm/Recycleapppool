using System.Text.Json;

namespace Decrypt
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE3NjE4MDA4NDEsImV4cCI6Njc2MTg4NzI0MSwiaXNzIjoiaHR0cHM6Ly9pYW0uY21zLmdvdi5hZSIsImF1ZCI6WyJwcm9maWxlIiwib2ZmbGluZV9hY2Nlc3MiLCJJZGVudGl0eVNlcnZlckFwaSJdLCJjbGllbnRfaWQiOiI1ZDJjOGZhNS05ZjU4LTQzMGMtYmNmMi01ZjQzNjZkNDI1ZGMiLCJzdWIiOiIxMzUiLCJhdXRoX3RpbWUiOjE3NjE4MDA4NDAsImlkcCI6ImxvY2FsIiwiRGlzcGxheU5hbWUiOiJTeXN0ZW0gMSBBZG1pbiIsIkVtYWlsIjoiYWRtaW5AZGdlLmdvdi5hZSIsIkxvZ2luUHJvdmlkZXJUeXBlIjoyLCJJZCI6MTM1LCJGaXJzdE5hbWUiOiJTeXN0ZW0gMiIsIkxhc3ROYW1lIjoiQWRtaW4iLCJVc2VybmFtZSI6IkFkbWluIiwiTWlkZGxlTmFtZSI6Ii0iLCJTdHJ1Y3R1cmVJZCI6IjEiLCJNYW5hZ2VySWQiOiI1NzgyNSIsIlN0cnVjdHVyZUlkcyI6IjEvNjQ0My84MTcwIiwiR3JvdXBJZHMiOiIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJDUiB3aXRoIFJvbGUiLCJBcHBsaWNhdGlvblJvbGVJZCI6Ijc0NyIsIlVzZXJUeXBlSWQiOiIxIiwiQ2xpZW50cyI6WyJ7XCJSb2xlSWRcIjo3NDcsXCJSb2xlXCI6XCJDUiB3aXRoIFJvbGVcIixcIkNsaWVudElkXCI6XCI1ZDJjOGZhNS05ZjU4LTQzMGMtYmNmMi01ZjQzNjZkNDI1ZGNcIn0iLCJ7XCJSb2xlSWRcIjo3NDcsXCJSb2xlXCI6XCJDUiB3aXRoIFJvbGVcIixcIkNsaWVudElkXCI6XCJhNGMyMjUxYi03Y2Y5LTQwMTItODdhOC04YjUxODdjYTQ4ZjZcIn0iXSwianRpIjoiNTBFOTY2Q0Q1NDAwRDY2ODA1MUJEQUZBNjAzQTExMDciLCJzaWQiOiIwRDE5RDIzNjZBNDNEQUUzNjhERDIxMEQyMzQxNzI3QiIsImlhdCI6MTc2MTgwMDg0MSwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsIklkZW50aXR5U2VydmVyQXBpIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.2QPbbGF8B1H5PoNvzT1tOw1xErM2NNRzcPUTvFggTXo");
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent("A9B5EC26B7B26CBA97ABBC39DE2679452CF331fccxvvbnbmfsdfdjkhr3894SDFvcnmjC81A9B5EC26B7B9FD86EE84F2D2594326A"), "Name");
            form.Add(new StringContent("A9B5ECCBA926B7BCBA92CF331Cnbvnvbnmasdqweq134sedxcv81A9B5EN0m4iUC26B7B9FD8bvSDFsdfsdfoihcvbuh2594326A"), "NameAr");
            form.Add(new StringContent("A9B5ECCBA926B7BCBA92CF331Cefrw6547881A9B5EN0m4iUC26B7B9FD8bvSDFsdfsdbvne6wr24867343545345foihcvbuh2594326A"), "Title");
            form.Add(new StringContent("a"), "TitleAr");
            form.Add(new StringContent("D:\\uaepass"), "Profession");
            form.Add(new StringContent("Intalio.Encryption.EncryptionUtility"), "ProfessionAr");
            form.Add(new StringContent("asd"), "Email");

            var response = await client.PostAsync("http://localhost:5000/api/document/AddEitDesignatedPersonee", form);

            // 🟢 Ensure success
            response.EnsureSuccessStatusCode();

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            // ✅ Print response
            Console.WriteLine("Response Status: " + response.StatusCode);
            Console.WriteLine("Response Body:");

              
            string jsonText = System.Text.Encoding.UTF8.GetString(AdvancedEncryption.DecryptBytes(responseBytes, "asd"));
 

            var jsonElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(jsonText);
            string pretty = System.Text.Json.JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine(pretty);

            Console.WriteLine("Hello, World!");
            return 1;
        }
    }
}
