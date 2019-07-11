using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlowerHouse.Utility
{
    public class CheckUtility
    {
        #region 验证字段

        /// <summary>
        /// 判断Email地址是否合法
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsEmail(string address)
        {
            return Regex.IsMatch(address, @"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 判断是否为Ip地址
        /// </summary>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public static bool IsIpAddress(string Ip)
        {
            return !string.IsNullOrEmpty(Ip) && Regex.IsMatch(Ip.Replace(" ", ""), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){1,3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 验证手机号格式是否合法
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsPhone(string phone)
        {
            //return Regex.IsMatch(phone, @"^[1][3,5,7,9]\d{9}$");
            return !string.IsNullOrEmpty(phone) && Regex.IsMatch(phone, @"^\d{11}$");
        }

        public static bool IsUserName(string username)
        {
            //用户名不能有特殊字符，不能为空，首尾不能出现下划线，不能全为数字
            return !string.IsNullOrEmpty(username) && Regex.IsMatch(username, @"^[a-zA-Z0-9_s·]+$") && !Regex.IsMatch(username, @"(^_)|(__)|(_+$)") && !Regex.IsMatch(username, @"^\d+\d+\d$");
        }

        public static bool IsPhoneCode(string phoneCode)
        {
            return !string.IsNullOrEmpty(phoneCode) && Regex.IsMatch(phoneCode, @"^\d{4}");
        }
        #endregion
    }
}
