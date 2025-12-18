using System;

namespace AdminControl.DTO
{
    public class AdminActionLogDto
    {
        public int LogID { get; set; }
        public int AdminUserID { get; set; }
        public string AdminUserName { get; set; } = string.Empty;
        public int? TargetUserID { get; set; }
        public string? TargetUserName { get; set; }
        public int ActionTypeID { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public string ActionDescription { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminActionLogCreateDto
    {
        public int AdminUserID { get; set; }
        public int? TargetUserID { get; set; }
        public int ActionTypeID { get; set; }
        public string? Reason { get; set; }
    }
}
