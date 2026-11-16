using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Xunit;

namespace API.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class ProfilesControllerTests
    {
        private (HttpClient client, DataContext context) CreateClient(Action<HttpClient> authenticate)
        {
            var factory = new TestProgram();
            var client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            authenticate(client);
            return (client, context);
        }

        [Fact]
        public async Task UpdateProfile_Guest_Returns403WithNoMutation()
        {
            var (client, context) = CreateClient(c => c.AuthenticateAsGuest());

            var response = await client.PutAsJsonAsync("/api/profiles",
                new { DisplayName = "Changed By Guest", Bio = "Changed by guest" });

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var user = await context.Users.AsNoTracking().SingleAsync(u => u.Id == TestProgram.TestUserId);
            user.DisplayName.Should().Be("Test User");
            user.Bio.Should().BeNull();
        }

        [Fact]
        public async Task UpdateProfile_Developer_Returns200AndUpdatesProfile()
        {
            var (client, context) = CreateClient(c => c.AuthenticateAsDeveloper());

            var response = await client.PutAsJsonAsync("/api/profiles",
                new { DisplayName = "Updated User", Bio = "Updated bio" });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var user = await context.Users.AsNoTracking().SingleAsync(u => u.Id == TestProgram.TestUserId);
            user.DisplayName.Should().Be("Updated User");
            user.Bio.Should().Be("Updated bio");
        }

        [Fact]
        public async Task UploadAvatar_Guest_Returns403WithNoMutation()
        {
            var (client, context) = CreateClient(c => c.AuthenticateAsGuest());

            using var form = new MultipartFormDataContent();
            var file = new ByteArrayContent(new byte[] { 0x89, 0x50, 0x4E, 0x47 });
            file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            form.Add(file, "file", "avatar.png");

            var response = await client.PostAsync("/api/profiles/avatar", form);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var user = await context.Users.AsNoTracking().SingleAsync(u => u.Id == TestProgram.TestUserId);
            user.AvatarBlobName.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAvatar_Guest_Returns403()
        {
            var (client, _) = CreateClient(c => c.AuthenticateAsGuest());

            var response = await client.DeleteAsync("/api/profiles/avatar");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task ChangePassword_Guest_Returns403WithNoMutation()
        {
            var (client, context) = CreateClient(c => c.AuthenticateAsGuest());

            var hashBefore = (await context.Users.AsNoTracking()
                .SingleAsync(u => u.Id == TestProgram.TestUserId)).PasswordHash;

            var response = await client.PostAsJsonAsync("/api/account/changePassword",
                new { CurrentPassword = "Password123!", NewPassword = "ChangedByGuest1!", ConfirmNewPassword = "ChangedByGuest1!" });

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var user = await context.Users.AsNoTracking().SingleAsync(u => u.Id == TestProgram.TestUserId);
            user.PasswordHash.Should().Be(hashBefore);
        }
    }
}
