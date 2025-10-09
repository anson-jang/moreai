using System.ComponentModel.DataAnnotations;

namespace moreai.web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "請輸入帳號")]
    [Display(Name = "帳號")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "請輸入密碼")]
    [Display(Name = "密碼")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Display(Name = "記住我")]
    public bool RememberMe { get; set; }
    
    public string? ReturnUrl { get; set; }
}