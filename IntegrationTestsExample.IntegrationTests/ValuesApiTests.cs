using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;

namespace IntegrationTestsExample.IntegrationTests
{
    public class ValuesApiTests
    {
        private readonly TestServer server;

        public ValuesApiTests()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(@"../../../../IntegrationTestsExample")
                .UseStartup<IntegrationTestsExample.Startup>();

            this.server = new TestServer(builder);
        }

        [Test]
        public async Task GetValuesTest()
        {
            // given
            var correctResult = JsonConvert.SerializeObject(new[] { "value1", "value2" });

            // when
            using (var client = this.server.CreateClient())
            {
                var response = await client.GetAsync("/api/values");
                var result = await response.Content.ReadAsStringAsync();

                // then
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Should().Be(correctResult);
            }
        }

        [Test]
        public async Task GetValue_When_ValueIdLowerThan1_Then_BadRequest()
        {
            // given
            var correctResult = (new[] { "value1", "value2" }).ToString();

            // when
            using (var client = this.server.CreateClient())
            {
                var response = await client.GetAsync("/api/values/0");

                // then
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [TestCase(1)]
        [TestCase(9)]
        [TestCase(17)]
        [TestCase(155)]
        public async Task GetValue_When_ValueIdHigherThan1_Then_ReturnValueId(int valueId)
        {
            // when
            using (var client = this.server.CreateClient())
            {
                var response = await client.GetAsync($"/api/values/{valueId}");
                var result = await response.Content.ReadAsStringAsync();

                // then
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Should().Be(valueId.ToString());
            }
        }

        [Test]
        public async Task DeleteValue_When_ValueDoesntExist_Then_NotFound()
        {
            // when
            using (var client = this.server.CreateClient())
            {
                var response = await client.DeleteAsync($"/api/values/0");

                // then
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }
    }
}
