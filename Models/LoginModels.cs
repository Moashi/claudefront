// Models/LoginModels.cs
using System;
using System.Collections.Generic;

namespace FrontFitLife.Models.LoginModels
{
    #region classes
    public class Session
    {
        public string Token { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime LastActivity { get; set; }
    }
    // Clase para los errores
    public class ErrorModel
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }

    // Clase para el usuario
    public class User
    {
        public string Cedula { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    public class BaseResponse
    {
        public bool Result { get; set; }
        public List<ErrorModel> Error { get; set; } = new List<ErrorModel>();
        public string Message { get; set; }

        public bool IsSuccess => Result && !Error.Any();
        public string ErrorMessage => Error?.FirstOrDefault()?.Message ?? Message;
    }

    public class  BaseRequest
    {
        public string Token { get; set; }

    }

    #endregion

    #region Request

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Cedula { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
    }

    public class GetUserProfileRequest : BaseRequest
    {
        public string Cedula { get; set; }
    }

    public class UpdateUserProfileRequest : BaseRequest
    {
     
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }

    #endregion

    #region Response

    public class LoginResponse : BaseResponse
    {
        public string Token { get; set; }
        public User User { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
    public class GetUserProfileResponse : BaseResponse
    {
       public User User { get; set; }
    }
    public class UpdateUserProfileResponse : BaseResponse
    {
        public User User { get; set; }
        public string Message { get; set; }
    }

    #endregion
}