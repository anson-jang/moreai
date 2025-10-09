using System.ComponentModel.DataAnnotations;

namespace moreai.web.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "請輸入使用者名稱")]
    [Display(Name = "使用者名稱")]
    [StringLength(50, ErrorMessage = "{0} 必須至少 {2} 個字元長，最多 {1} 個字元。", MinimumLength = 3)]
    public required string Username { get; set; }

    [Required(ErrorMessage = "請輸入電子郵件")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    [Display(Name = "電子郵件")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "請輸入密碼")]
    [StringLength(100, ErrorMessage = "{0} 必須至少 {2} 個字元長，最多 {1} 個字元。", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "密碼")]
    public required string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "確認密碼")]
    [Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "請輸入名字")]
    [Display(Name = "名字")]
    [StringLength(50)]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "請輸入姓氏")]
    [Display(Name = "姓氏")]
    [StringLength(50)]
    public required string LastName { get; set; }
}