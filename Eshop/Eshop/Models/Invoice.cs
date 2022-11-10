using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eshop.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [DisplayName("Mã HĐ")]
        public string Code { get; set; }

        public int AccountId { get; set; }

        // Navigation reference property cho khóa ngoại đến Account 
        [DisplayName("Khách hàng")]
        public Account Account { get; set; }

        [DisplayName("Ngày lập")]
        [DataType(DataType.DateTime)]
        public DateTime IssuedDate { get; set; } = DateTime.Now;

        [DisplayName("Địa chỉ giao hàng")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string ShippingAddress { get; set; }

        [DisplayName("SĐT giao hàng")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [RegularExpression("0\\d{9}", ErrorMessage = "SĐT không hợp lệ")]
        public string ShippingPhone { get; set; }

        [DisplayName("Tổng tiền")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        [DefaultValue(0)]
        public int Total { get; set; } = 0;

        [DisplayName("Còn hiệu lực")]
        [DefaultValue(true)]
        public bool Status { get; set; } = true;

        // Collection reference property cho khóa ngoại từ InvoiceDetail
        public List<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
