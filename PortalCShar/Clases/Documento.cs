using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalCShar.Clases
{
    public class Documento
    {
        public string num_cpe { get; set; }
        public string seriecorrelativo { get; set; }
        public string rucemisor { get; set; }
        public string tipodocumento { get; set; }
        public string des_tipodocumento { get; set; }
        public string serie { get; set; }
        public string ndocumento { get; set; }
        public string fechaemision { get; set; }
        public decimal montototal { get; set; }
        public string monto { get; set; }
    }
}