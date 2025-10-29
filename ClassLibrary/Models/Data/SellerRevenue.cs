#nullable disable
using System;

namespace ClassLibrary.Models.Data;

public partial class SellerRevenue
{
    public int SellerRevenueId { get; set; }

    public int OrderId { get; set; }

    public int SellerId { get; set; }

    /// <summary>
    /// ยอดขายรวมของร้านนี้ในออเดอร์นี้ (ก่อนหักค่าคอมมิชชั่น)
    /// </summary>
    public decimal GrossAmount { get; set; }

    /// <summary>
    /// อัตราค่าคอมมิชชั่น Platform (%) - Default 0
    /// </summary>
    public decimal CommissionRate { get; set; } = 0;

    /// <summary>
    /// จำนวนเงินค่าคอมมิชชั่นที่ Platform เก็บ
    /// </summary>
    public decimal CommissionAmount { get; set; } = 0;

    /// <summary>
    /// ยอดเงินสุทธิที่ร้านจะได้รับ (หลังหักค่าคอมมิชชั่น)
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// สถานะการจ่ายเงิน: Pending, Settled, Failed
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// วันที่โอนเงินให้ร้านแล้ว
    /// </summary>
    public DateTime? SettledAt { get; set; }

    /// <summary>
    /// หมายเหตุการโอนเงิน (เช่น เลขที่อ้างอิง)
    /// </summary>
    public string SettlementNote { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual Order Order { get; set; }

    public virtual Seller Seller { get; set; }
}
