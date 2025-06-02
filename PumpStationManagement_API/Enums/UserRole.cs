using System.ComponentModel;

namespace PumpStationManagement_API.Enums;

enum UserRole
{
    /// <summary>
    /// User
    /// </summary>
    [Description("Người dùng")]
    User = 0,

    /// <summary>
    /// Admin
    /// </summary>
    [Description("Quản trị viên")]
    Admin = 1,
}

