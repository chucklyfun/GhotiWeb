using System;
using Utilities.Data;

namespace GameLogic.User
{
    public class User : EntityBase
    {
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}