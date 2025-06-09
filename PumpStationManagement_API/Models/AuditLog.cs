using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace PumpStationManagement_API.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        [Required]
        public int EntityId { get; set; } // ID của bản ghi bị thay đổi (ví dụ: PumpId, StationId)

        [Required]
        public string EntityType { get; set; } // Loại thực thể (PumpStation, Pump, OperatingData, v.v.)

        [Required]
        public string ActionType { get; set; } // Loại hành động (Create, Update, Delete, v.v.)

        [Required]
        public string ContentBefore { get; set; } // Nội dung trước khi thay đổi (JSON hoặc chuỗi)

        [Required]
        public string ContentAfter { get; set; } // Nội dung sau khi thay đổi (JSON hoặc chuỗi)

        [Required]
        public DateTime ActionDate { get; set; } // Thời gian thực hiện

        [Required]
        public int PerformedBy { get; set; } // ID của người thực hiện (liên kết với bảng Users)

        [ForeignKey("PerformedBy")]
        public User PerformedByNavigation { get; set; } // Quan hệ với bảng Users

        public string Description { get; set; }
    }
}
