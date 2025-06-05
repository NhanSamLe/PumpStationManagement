using System.ComponentModel;

namespace PumpStationManagemnet_BlazorApp.Enums
{
    enum MaintainStatus
    {
        /// <summary>
        /// Active
        /// </summary>
        [Description("Đang thực hiện")]
        InProgress = 0,

        /// <summary>
        /// Ignored
        /// </summary>
        [Description("Đã hoàn thành")]
        Completed = 1,

        /// <summary>
        /// Resolved
        /// </summary>
        [Description("Đã lên lịch")]
        Scheduled = 2,
    }
}
