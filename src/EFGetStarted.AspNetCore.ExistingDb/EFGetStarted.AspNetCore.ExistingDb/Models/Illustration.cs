using System;
using System.Collections.Generic;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
    public partial class Illustration
    {
        public Illustration()
        {
            ProductModelIllustration = new HashSet<ProductModelIllustration>();
        }

        public int IllustrationId { get; set; }
        public string Diagram { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<ProductModelIllustration> ProductModelIllustration { get; set; }
    }
}
