using AutoMapper;
using BLL.DTOs;
using DAL.Entities;
using DAL.Reposistories;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class AccountService
    {
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;

        public AccountService(IGenericRepository<Account> accountRepository, IMapper mapper, EmailService emailService)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService;
        }

        public async Task<ResponseDTO> CreateAccountAsync(String verificationToken ,AccountCreateRequestDTO accountCreateRequest)
        {
            try
            {
                var account = _accountRepository.GetSingle(a => a.Token == verificationToken && a.TokenExpiration > DateTime.UtcNow && !a.IsVerified);
                if (account == null)
                {
                    return new ResponseDTO { Success = false, Message = "Invalid or expired verification token." };
                }
                account.AccountName = accountCreateRequest.AccountName;
                account.AccountPassword = accountCreateRequest.AccountPassword;
                account.IsVerified = true; // Mark the account as verified
                var existingAccount = _accountRepository.GetSingle(a => a.AccountEmail == account.AccountEmail);
                _accountRepository.Update(account);
                return new ResponseDTO { Success = true, Message = "Account created successfully." };
            }
            catch
            {
                return new ResponseDTO { Success = false, Message = "An error occurred while creating the account." };
            }
            
        }



        public async Task<ResponseDTO> RequestCreateAccountAsync(string email)
        {
            var existingAccount = _accountRepository.GetSingle(a => a.AccountEmail == email && a.IsVerified == true);
            if (existingAccount != null)
            {
                return new ResponseDTO { Success = false, Message = "Account with this email already exists." };
            }
            existingAccount = _accountRepository.GetSingle(a => a.AccountEmail == email);
            string verificationToken = GenerateSecureToken();
            DateTime expiryTime = DateTime.UtcNow.AddMinutes(5); // Token valid for 5 minutes
            string accountId;
            if (existingAccount != null && existingAccount.AccountEmail != null)
            {
                accountId = existingAccount.AccountId; // Use existing account ID if it exists
            }
            else
                accountId = Guid.NewGuid().ToString(); // Generate a new account ID if it doesn't exist
            var account = new Account
            {
                AccountId = accountId,
                AccountEmail = email,
                Token = verificationToken,
                TokenExpiration = expiryTime,
                IsVerified = false // Initially set to false until verified
            };
            string subject = "Account Verification Required";
            string body = $"Hello,\n\n" +
                          $"Please click the following link to verify your account:\n" +
                          $"{verificationToken}\n\n" +
                          $"This link will expire in 5 minutes. If you did not request this, please ignore this email.\n\n" +
                          $"Regards,\nYour App Team";
            try
            {
                await _emailService.SendEmailAsync(email, subject, body);
                if (existingAccount == null)
                {
                    _accountRepository.Create(account);
                }
                else
                    _accountRepository.Update(account); // Save the account with the token and expiration
                return new ResponseDTO { Success = true, Message = "Verification email sent successfully." };
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger)
                Console.WriteLine($"Error sending verification email to {email}: {ex.Message}");
                return new ResponseDTO { Success = false, Message = "Failed to send verification email." };
            }

        }
        public async Task<ResponseDTO> LoginAccountAsync(AccountRequestDTO accountRequest)
        {
            var account = _accountRepository.GetSingle(a => a.AccountEmail == accountRequest.AccountEmail && a.AccountPassword == accountRequest.AccountPassword);
            if (account == null)
            {
                return new ResponseDTO { Success = false, Message = "Invalid email or password." };
            }
            return new ResponseDTO { Success = true, Message = "Login successful." };
        }

        public async Task<ResponseDTO> RequestPasswordResetAsync(string email)
        {
            var account = _accountRepository.GetSingle(a => a.AccountEmail == email);
            if (account == null)
            {
                return new ResponseDTO { Success = false, Message = "Account not found." };
            }
            string resetToken = GenerateSecureToken();
            DateTime expiryTime = DateTime.UtcNow.AddMinutes(5);
            account.Token = resetToken;
            account.TokenExpiration = expiryTime;
          
            string subject = "Password Reset Request";
            string body = $"Hello {account.AccountName ?? "User"},\n\n" +
                          $"You requested a password reset. Please click the following link to reset your password:\n" +
                          $"{resetToken}\n\n" +
                          $"This link will expire in 30 minutes. If you did not request this, please ignore this email.\n\n" +
                          $"Regards,\nYour App Team";
            try
            {
                await _emailService.SendEmailAsync(account.AccountEmail, subject, body);
                 _accountRepository.Update(account);
                return new ResponseDTO { Success = true , Message = "Send successful"};

            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger)
                Console.WriteLine($"Error sending password reset email to {account.AccountEmail}: {ex.Message}");
                return new ResponseDTO { Success = false, Message = "Fail" };
            }
        }

        public async Task<ResponseDTO> ResetPasswordAsync(string token, string newPassword)
        {
            var account = _accountRepository.GetSingle(a => a.Token == token && a.TokenExpiration > DateTime.UtcNow);
            if (account == null)
            {
                return new ResponseDTO { Success = false, Message = "Invalid or expired token." };
            }
            account.AccountPassword = newPassword;
            account.Token = null; // Clear the token after successful reset
            account.TokenExpiration = null; // Clear the expiration date
            _accountRepository.Update(account);
            return new ResponseDTO { Success = true, Message = "Password reset successfully." };
        }


        private string GenerateSecureToken()
        {
            // This generates a 256-bit token
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            // Convert to URL-safe Base64 string
            return Convert.ToBase64String(bytes)
                          .Replace('+', '-')
                          .Replace('/', '_')
                          .TrimEnd('='); // Remove padding '=' characters for cleaner URLs
        }
        public async Task<ResponseDTO<AccountRequestDTO>> GetAccountByEmailAsync(string email)
        {
            var account = _accountRepository.GetSingle(a => a.AccountEmail == email);
            if (account == null)
            {
                return new ResponseDTO<AccountRequestDTO> { Success = false, Message = "Account not found." };
            }
            var accountDto = _mapper.Map<AccountRequestDTO>(account);
            return new ResponseDTO<AccountRequestDTO> { Success = true, Data = accountDto };
        }

        public async Task<ResponseDTO> UpdateAccountAsync(AccountRequestDTO accountRequest)
        {
            var account = _mapper.Map<Account>(accountRequest);
            var existingAccount = _accountRepository.GetSingle(a => a.AccountId == account.AccountId);
            if (existingAccount == null)
            {
                return new ResponseDTO { Success = false, Message = "Account not found." };
            }
            existingAccount.AccountEmail = account.AccountEmail;
            existingAccount.AccountPassword = account.AccountPassword;
            existingAccount.AccountName = account.AccountName;

            _accountRepository.Update(existingAccount);
            return new ResponseDTO { Success = true, Message = "Account updated successfully." };
        }

       
    }
}
