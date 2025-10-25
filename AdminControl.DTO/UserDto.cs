﻿namespace AdminControl.DTO
{
    public class UserDto
    {
        public int UserID { get; set; }
        public string Login { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;
    }
}