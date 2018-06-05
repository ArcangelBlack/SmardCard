using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartCardWebApplication.Models.ViewModel
{
    public class CardsViewmodel
    {
        public CardsViewmodel()
        {
            Cards = new List<CardViewModel>();
        }

        public List<CardViewModel> Cards { get; set; }
    }

    public class CardViewModel
    {
        public string FriendlyName { get; set; }
        public string SerialNumber { get; set; }
        public string Issuer { get; set; }

    }
}