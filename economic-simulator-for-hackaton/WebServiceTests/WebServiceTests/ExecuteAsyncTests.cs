using economic_simulator_for_hackaton;

namespace WebServiceTests.ProgramTests
{
    public class ExecuteAsyncTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ExecuteAsyncTest()
        {
            //Append
            var webService = new WebService();
            var task = Task.Run(()=> webService.ExecuteAsync([]));

            //Act
            var client = new HttpClient();
            var requestUri = "http://localhost:5000/api/message"; 

            var guid = Guid.NewGuid().ToString();
            var message = "test message";

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(guid);
            request.Content = new StringContent($"\"{message}\"", System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert

            Console.WriteLine(response);

            //Assert.That(responseContent.ToString(), Is.Empty);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(responseContent.Contains(message), Is.True);
        }
    }
}