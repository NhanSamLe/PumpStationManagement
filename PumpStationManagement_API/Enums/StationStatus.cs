using System.ComponentModel;

namespace PumpStationManagement_API.Enums;

enum StationStatus
{
    /// <summary>
    /// Active
    /// </summary>
    [Description("Đang hoạt động")]
    Active = 0,

    /// <summary>
    /// Maintenance
    /// </summary>
    [Description("Bảo trì")]
    Maintenance = 1,

    /// <summary>
    /// Inactive
    /// </summary>
    [Description("Ngừng hoạt động")]
    Inactive = 2,
}

