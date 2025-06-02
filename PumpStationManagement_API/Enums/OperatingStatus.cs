using System.ComponentModel;

namespace PumpStationManagement_API.Enums;

enum OperatingStatus
{
    /// <summary>
    /// Good
    /// </summary>
    [Description("Tốt")]
    Good = 0,

    /// <summary>
    /// Normal
    /// </summary>
    [Description("Bình thường")]
    Normal = 1,

    /// <summary>
    /// Warning
    /// </summary>
    [Description("Cảnh báo")]
    Warning = 2,

    /// <summary>
    /// Warning
    /// </summary>
    [Description("Nghiêm trọng")]
    Critial = 3,
}

