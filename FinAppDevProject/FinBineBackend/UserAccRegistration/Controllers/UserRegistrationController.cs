using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using FinBineBackend.UserAccRegistration.Models;
using FinBineBackend.UserAccRegistration.Services;
using FinBineBackend.UserAccRegistration.Logs.Services;

namespace FinBineBackend.UserAccRegistration.Controllers
{
    [ApiController]
    [Route("api/user/registration")]
    public class UserRegistrationController : ControllerBase
    {
        private static readonly string[] AllowedProviders = { "Google", "Microsoft", "Yahoo" };

        private readonly UserRegistrationService _registrationService;
        private readonly UserRegistrationLoggingService _regLogger;

        public UserRegistrationController(
            UserRegistrationService registrationService,
            UserRegistrationLoggingService regLogger)
        {
            _registrationService = registrationService;
            _regLogger = regLogger;
        }

        [HttpPost("email")]
        public async Task<IActionResult> RegisterWithEmail([FromBody] RegisterUserRequest request)
        {
            var fieldsError = ValidateSharedFields(
                request.FirstName, request.LastName, request.DisplayName,
                request.Email, request.Country, request.Currency, request.Timezone,
                request.DateOfBirth, request.Email);

            if (fieldsError != null) return fieldsError;

            var result = await _registrationService.RegisterWithEmailAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("sso")]
        public async Task<IActionResult> RegisterWithSSO([FromBody] RegisterUserSSORequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                _regLogger.LogRegistrationRejected("Validation", request.Email, "Missing SSO token.");
                return BadRequest(new RegisterUserResponse { Success = false, Message = "Missing sign-in token." });
            }

            if (!AllowedProviders.Contains(request.Provider, StringComparer.OrdinalIgnoreCase))
            {
                _regLogger.LogRegistrationRejected("Validation", request.Email, $"Unsupported provider: {request.Provider}");
                return BadRequest(new RegisterUserResponse { Success = false, Message = "Unsupported sign-in provider." });
            }

            var fieldsError = ValidateSharedFields(
                request.FirstName, request.LastName, request.DisplayName,
                request.Email, request.Country, request.Currency, request.Timezone,
                request.DateOfBirth, request.Email);

            if (fieldsError != null) return fieldsError;

            var mappedRequest = new RegisterUserRequest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                Country = request.Country,
                Currency = request.Currency,
                Timezone = request.Timezone
            };

            var result = await _registrationService.RegisterWithSSOAsync(mappedRequest, request.Token, request.Provider);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Shared server-side validation for both endpoints — never trust
        // the frontend alone. Rejections here are logged as Warnings
        // under Source: "Validation".
        private IActionResult? ValidateSharedFields(
            string firstName, string lastName, string displayName,
            string email, string country, string currency, string timezone,
            DateOnly dateOfBirth, string logEmail)
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(displayName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(country) ||
                string.IsNullOrWhiteSpace(currency) ||
                string.IsNullOrWhiteSpace(timezone))
            {
                _regLogger.LogRegistrationRejected("Validation", logEmail, "Missing required fields.");
                return BadRequest(new RegisterUserResponse { Success = false, Message = "Missing required fields." });
            }

            if (!Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            {
                _regLogger.LogRegistrationRejected("Validation", logEmail, "Invalid email format.");
                return BadRequest(new RegisterUserResponse { Success = false, Message = "Enter a valid email address." });
            }

            if (CalculateAge(dateOfBirth) < 18)
            {
                _regLogger.LogRegistrationRejected("Validation", logEmail, "Under 18.");
                return BadRequest(new RegisterUserResponse { Success = false, Message = "You must be 18 years or older to register." });
            }

            return null;
        }

        private static int CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;
            return age;
        }
    }
}