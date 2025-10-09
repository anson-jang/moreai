using System.ComponentModel.DataAnnotations;

namespace moreai.Models
{
    public class MemberProfileViewModel
    {
        [Required(ErrorMessage = "請輸入電話")]
        [Phone(ErrorMessage = "請輸入有效的電話號碼")]
        [Display(Name = "電話")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "請輸入地址")]
        [Display(Name = "地址")]
        public string Address { get; set; }

        [Required(ErrorMessage = "請輸入公司名稱")]
        [Display(Name = "公司名稱")]
        public string CompanyName { get; set; }
    }
}