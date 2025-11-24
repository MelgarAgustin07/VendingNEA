using System;
using System.Collections.Generic;

namespace VendingNEA_0.Models
{
    public partial class Contiene
    {
        public int CodCondicion { get; set; }
        public int NumAcuerdo { get; set; }

        public virtual Condicion CodCondicionNavigation { get; set; } = null!;
        public virtual Acuerdo NumAcuerdoNavigation { get; set; } = null!;
    }
}
