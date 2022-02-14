﻿using Microsoft.Extensions.Logging;

namespace zblesk_web.Services;

public class FakeMailerService : IMailerService
{
    private readonly ILogger<FakeMailerService> _logger;

    public FakeMailerService(
        ILogger<FakeMailerService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task SendMail(string subject, string body, params string[] recipients)
    {
        _logger.LogWarning("No mailer configured. Not sending newsletter:\nto: {@recipients}\nsubj: {subject}\n{body}\n\n", recipients, subject, body);
        return Task.CompletedTask;
    }
}
