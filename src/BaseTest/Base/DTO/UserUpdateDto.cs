using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest.Base.DTO
{
    public class UserUpdateDto
    {
        /// <summary>
        /// 1|sell,2|buy
        /// </summary>
        public int Type { get; set; }
        public long UserID { get; set; }
        public double Amount { get; set; }
    }
}
