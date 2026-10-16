using App.Application.Models.Requests;
using App.Application.Options;
using App.Application.Security;
using App.Application.Services;
using App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace App.Application.Tests;

public sealed class PasswordResetTests : IDisposable
{
    private const string Site = "https://marketly.test";

    private readonly TestDatabase db = new();
    private readonly AuthService service;

    public PasswordResetTests()
    {
        // No SMTP host: the link is handed back instead of being e-mailed.
        service = new AuthService(
            db.Repo<UserEntity>(),
            db.Repo<RoleEntity>(),
            Microsoft.Extensions.Options.Options.Create(new EmailOptions()),
            db.Clock,
            NullLogger<AuthService>.Instance);
    }

    public void Dispose() => db.Dispose();

    private async Task<string> RequestTokenAsync(string email = "buyer@example.com")
    {
        var result = await service.RequestPasswordResetAsync(email, Site);
        var link = result.Data!.UndeliveredLink!;
        Assert.StartsWith($"{Site}/renew-password/", link);
        return link[(link.LastIndexOf('/') + 1)..];
    }

    private Task<App.Application.Results.ServiceResult<App.Application.Models.Dtos.AuthUserDto>> LoginAsync(string password) =>
        service.ValidateLoginAsync(new LoginRequest { Email = "buyer@example.com", Password = password });

    [Fact]
    public async Task Unknown_addresses_get_the_same_answer_and_no_link()
    {
        var known = await service.RequestPasswordResetAsync("buyer@example.com", Site);
        var unknown = await service.RequestPasswordResetAsync("kimse@example.com", Site);

        Assert.Equal(known.Message, unknown.Message);
        Assert.NotNull(known.Data!.UndeliveredLink);
        Assert.Null(unknown.Data!.UndeliveredLink);
    }

    [Fact]
    public async Task Only_a_hash_of_the_token_is_stored()
    {
        var token = await RequestTokenAsync();

        var user = await db.Fresh().Set<UserEntity>().SingleAsync(u => u.Email == "buyer@example.com");
        Assert.NotEqual(token, user.ResetPasswordToken);
        Assert.Equal(ResetToken.Hash(token), user.ResetPasswordToken);
    }

    [Fact]
    public async Task A_link_changes_the_password_once()
    {
        var token = await RequestTokenAsync();

        var first = await service.ResetPasswordAsync(new RenewPasswordRequest { VerificationCode = token, Password = "YeniSifre1!" });
        var second = await service.ResetPasswordAsync(new RenewPasswordRequest { VerificationCode = token, Password = "BaskaSifre2!" });

        Assert.True(first.Success);
        Assert.False(second.Success);
        Assert.True((await LoginAsync("YeniSifre1!")).Success);
        Assert.False((await LoginAsync("Demo123!")).Success);
    }

    [Fact]
    public async Task A_link_expires_after_an_hour()
    {
        var token = await RequestTokenAsync();

        db.Clock.Advance(AuthService.ResetTokenLifetime + TimeSpan.FromMinutes(1));

        Assert.False((await service.VerifyResetTokenAsync(token)).Success);
    }

    [Fact]
    public async Task A_second_request_within_a_minute_does_not_send_a_new_link()
    {
        await RequestTokenAsync();

        var again = await service.RequestPasswordResetAsync("buyer@example.com", Site);

        Assert.Null(again.Data!.UndeliveredLink);
    }

    [Fact]
    public async Task Malformed_tokens_are_rejected_without_a_lookup()
    {
        Assert.False((await service.VerifyResetTokenAsync("../../etc")).Success);
        Assert.False((await service.VerifyResetTokenAsync("")).Success);
    }
}
