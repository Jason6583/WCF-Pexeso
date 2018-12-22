using System;
using System.ComponentModel.DataAnnotations;

namespace PexesoDatabase
{
    [Serializable]
    public class PexesoPlayer
    {
        public PexesoPlayer()
        {
        }

        public PexesoPlayer(string nickName, string password)
        {
            NickName = nickName;
            Password = BCrypt.Net.BCrypt.HashString(password);
        }


        [Display(Name = "Players nickname")]
        [Key]
        public string NickName { get; set; }
        public string Password { get; set; }

    }
}
