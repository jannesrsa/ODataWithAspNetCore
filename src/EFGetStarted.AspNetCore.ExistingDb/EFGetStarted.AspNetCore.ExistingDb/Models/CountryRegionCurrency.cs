﻿using System;
using System.Collections.Generic;

namespace EFGetStarted.AspNetCore.ExistingDb.Models
{
    public partial class CountryRegionCurrency
    {
        public string CountryRegionCode { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime ModifiedDate { get; set; }

        public CountryRegion CountryRegionCodeNavigation { get; set; }
        public Currency CurrencyCodeNavigation { get; set; }
    }
}
