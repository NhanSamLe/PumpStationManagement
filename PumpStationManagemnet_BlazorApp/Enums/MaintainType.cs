using System.ComponentModel;

namespace PumpStationManagemnet_BlazorApp.Enums
{
    enum MaintainType
    {
        /// <summary>
        /// Active
        /// </summary>
        [Description("Bảo trì định kỳ")]
        Preventive = 0,

        /// <summary>
        /// Ignored
        /// </summary>
        [Description("Sửa chữa")]
        Corrective = 1,

        /// <summary>
        /// Resolved
        /// </summary>
        [Description("Khẩn cấp")]
        Emergency = 2,
    }
}
