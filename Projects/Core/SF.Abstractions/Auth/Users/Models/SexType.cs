using System.ComponentModel.DataAnnotations;

namespace SF.Auth.Users
{
	public enum SexType
    {
        [Display(Name = "未指定")]
        Unknown,
        [Display(Name = "男")]
        Male,
        [Display(Name = "女")]
        Female
    }

}

