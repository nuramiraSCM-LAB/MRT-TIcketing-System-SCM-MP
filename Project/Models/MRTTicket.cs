using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class MRTTicket
    {
        
        [Display(Name = "Date & Time")]
        
        public DateTime TicketDateTime
        {
            get
            {
                
                return DateTime.Now;
            }

            set { }
        }
        
        [Display(Name = "Date & Time")]
        [DisplayFormat(DataFormatString = "{0:f}")]
        public DateTime ViewDateTime { get; set; }

        [Display(Name = "Ticket Id")]
        public string TicketId
        {
            get
            {
                string hexTicks = DateTime.Now.Ticks.ToString("{0:d MMMMM, yyyy}");
                return hexTicks.Substring(hexTicks.Length - 15, 9);
            }

            set { }
        }

        [Display(Name = "Ticket Id")]
        public string ViewId { get; set; }

        [Required(ErrorMessage = "Your name is required")]
        [StringLength(50, ErrorMessage = "5 to 50 characters.", MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "IC Number/Passport")]
        public string IC { get; set; }

        [Required(ErrorMessage = "Your phone number is required")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int IndexStatus { get; set; }

        [Required]
        [Display(Name = "From Station")]
        public int IndexFStation { get; set; }

        [Required]
        [Display(Name = "To Station")]
        public int IndexTStation { get; set; }

        [Required]
        [Display(Name = "Package")]
        public int IndexPackage { get; set; }
        
        public double AmountPackage
        {
            
            get
            {
                //bool single , Return;
                    return fares[IndexFStation, IndexTStation];
            }
        }

        [DisplayFormat(DataFormatString = "{0:c2}")]
        [Display(Name = "SubTotal")]
        public double Subtotal
        {
            get
            {
                if (IndexPackage == 0)
                    return fares[IndexFStation, IndexTStation];
                else
                    return fares[IndexFStation, IndexTStation] + AmountPackage;
            }
            set { }
        }



        [DisplayFormat(DataFormatString = "{0:c2}")]
        [Display(Name = "Paid Amount")]
        public double Amount
        {
            get
            {
                if (IndexStatus == 1)
                    return Subtotal - (Subtotal * 0.5);
                else if (IndexStatus == 2)
                    return Subtotal - (Subtotal * 0.6);
                else if (IndexStatus == 3)
                    return Subtotal - (Subtotal * 0.4);
                else
                    return Subtotal;

            }
            set { }
        }

        [Display(Name = "Discount")]
        public string Discount
        {
            get
            {
                if (IndexStatus == 1)
                    return "50%";
                else if (IndexStatus == 2)
                    return "60%";
                else if (IndexStatus == 3)
                    return "40%";
                else
                    return "-";

            }
            set { }
        }


        //MRT Fares

        double[,] fares =
        {
            {0.80,1.20,1.80,2.00,2.60,2.70,3.10,3.30,3.20,3.50,3.30,3.40,3.10,3.20,3.30,3.40,3.50,3.60,3.70,3.90,4.00,4.10,4.30,4.50,4.60,4.80,4.80,5.00,5.30,5.40,5.50},
            {1.20,0.80,1.50,1.80,2.30,2.70,2.80,3.10,3.40,3.30,3.70,3.30,3.70,3.80,3.20,3.30,3.40,3.50,3.60,3.80,3.90,4.00,4.20,4.40,4.50,4.60,4.70,4.90,5.20,5.20,5.40},
            {1.80,1.50,0.80,1.10,1.80,2.10,2.60,2.60,3.00,3.20,3.30,3.50,3.40,3.50,3.60,3.70,3.20,3.30,3.40,3.50,3.60,3.80,3.90,4.10,4.30,4.40,4.50,4.60,4.90,5.00,5.10},
            {2.00,1.80,1.10,0.80,1.60,1.90,2.30,2.60,2.80,3.00,3.10,3.30,3.80,3.40,3.40,3.60,3.80,3.20,3.30,3.40,3.50,3.70,3.80,4.00,4.10,4.30,4.40,4.50,4.80,4.90,5.00},
            {2.60,2.30,1.80,1.60,0.80,1.30,1.80,2.00,2.40,2.80,3.00,3.20,3.30,3.50,3.60,3.20,3.40,3.60,3.70,3.20,3.20,3.40,3.50,3.70,3.90,4.00,4.10,4.30,4.60,4.60,4.80},
            {2.70,2.70,2.10,1.90,1.30,0.80,1.30,1.70,2.00,2.40,2.70,2.90,3.10,3.30,3.40,3.60,3.80,3.40,3.50,3.70,3.80,3.20,3.40,3.60,3.70,3.90,4.00,4.10,4.40,4.50,4.60},
            {3.10,2.80,2.60,2.30,1.80,1.30,0.80,1.30,1.70,2.00,2.60,2.80,3.20,3.40,3.10,3.30,3.50,3.70,3.20,3.50,3.60,3.80,3.20,3.40,3.60,3.70,3.80,3.90,4.20,4.30,4.40},
            {3.30,3.10,2.60,2.60,2.00,1.70,1.30,0.80,1.30,1.70,2.20,2.50,2.90,3.10,3.20,3.40,3.20,3.40,3.60,3.30,3.40,3.60,3.80,3.40,3.60,3.60,3.60,3.80,4.10,4.20,4.30},
            {3.20,3.40,3.00,2.80,2.40,2.00,1.70,1.30,0.80,1.20,1.80,2.10,2.80,2.80,2.90,3.10,3.40,3.10,3.30,3.60,3.70,3.40,3.60,3.80,3.20,3.40,3.50,3.60,3.90,4.00,4.10},
            {3.50,3.30,3.20,3.00,2.80,2.40,2.00,1.70,1.20,0.80,1.60,1.80,2.50,2.70,2.60,2.80,3.10,3.10,3.10,3.30,3.50,3.70,3.40,3.60,3.80,3.20,3.30,3.50,3.80,3.90,4.00},
            {3.30,3.70,3.30,3.10,3.00,2.70,2.60,2.20,1.80,1.60,0.80,1.10,1.80,2.10,2.20,2.50,2.80,2.80,3.00,3.30,3.50,3.30,3.50,3.30,3.50,3.70,3.80,3.20,3.50,3.60,3.70},
            {3.40,3.30,3.50,3.30,3.20,2.90,2.80,2.50,2.10,1.80,1.10,0.80,1.70,1.90,2.00,2.30,2.60,2.60,2.80,3.10,3.30,3.10,3.40,3.70,3.30,3.50,3.60,3.10,3.40,3.50,3.60},
            {3.10,3.70,3.40,3.80,3.30,3.10,3.20,2.90,2.80,2.50,1.80,1.70,0.80,1.20,1.30,1.60,1.90,2.10,2.30,2.70,2.70,3.00,3.30,3.20,3.40,3.70,3.20,3.50,3.10,3.20,3.30},
            {3.20,3.80,3.50,3.40,3.50,3.30,3.40,3.10,2.80,2.70,2.10,1.90,1.20,0.80,1.00,1.30,1.70,1.80,2.10,2.50,2.70,2.80,3.00,3.50,3.30,3.50,3.60,3.30,3.70,3.80,3.20},
            {3.30,3.20,3.60,3.40,3.60,3.40,3.10,3.20,2.90,2.60,2.20,2.00,1.30,1.00,0.80,1.10,1.50,1.80,1.90,2.30,2.50,2.60,2.90,3.30,3.20,3.40,3.50,3.80,3.60,3.70,3.20},
            {3.40,3.30,3.70,3.60,3.20,3.60,3.30,3.40,3.10,2.80,2.50,2.30,1.60,1.30,1.10,0.80,1.20,1.50,1.80,2.10,2.30,2.60,2.70,3.10,3.40,3.20,3.30,3.60,3.50,3.60,3.80},
            {3.50, 3.40, 3.20, 3.80, 3.40, 3.80, 3.50, 3.20, 3.40, 3.10, 2.80, 2.60, 1.90, 1.70, 1.50, 1.20, 0.80, 1.10, 1.40, 1.80, 1.90, 2.30, 2.70, 2.90, 3.10, 3.40, 3.10, 3.40, 3.30, 3.40, 3.60},
            {3.60, 3.50, 3.30, 3.20, 3.60, 3.40, 3.70, 3.40, 3.10, 3.30, 2.80, 2.60, 2.10, 1.80, 1.80, 1.50, 1.10, 0.80, 1.10, 1.50, 1.80, 2.10, 2.40, 2.60, 2.90, 3.20, 3.30, 3.20, 3.70, 3.30, 3.40},
            {3.70, 3.60, 3.40, 3.30, 3.70, 3.50, 3.20, 3.60, 3.30, 3.10, 3.00, 2.80, 2.30, 2.10, 1.90, 1.80, 1.40, 1.10, 0.80, 1.30, 1.50, 1.80, 2.20, 2.70, 2.70, 3.00, 3.20, 3.10, 3.60, 3.70, 3.30},
            {3.90, 3.80, 3.50, 3.40, 3.20, 3.70, 3.50, 3.30, 3.60, 3.30, 3.30, 3.10, 2.70, 2.50, 2.30, 2.10, 1.80, 1.50, 1.30, 0.80, 1.10, 1.50, 1.80, 2.30, 2.60, 2.70, 2.80, 3.20, 3.30, 3.40, 3.60},
            {4.00, 3.90, 3.60, 3.50, 3.20, 3.80, 3.60, 3.40, 3.70, 3.50, 3.50, 3.30, 2.70, 2.70, 2.50, 2.30, 1.90, 1.80, 1.50, 1.10, 0.80, 1.30, 1.70, 2.10, 2.40, 2.80, 2.70, 3.00, 3.10, 3.30, 3.50},
            {4.10, 4.00, 3.80, 3.70, 3.40, 3.20, 3.80, 3.60, 3.40, 3.70, 3.30, 3.10, 3.00, 2.80, 2.60, 2.60, 2.30, 2.10, 1.80, 1.50, 1.30, 0.80, 1.20, 1.80, 2.00, 2.40, 2.60, 2.70, 3.30, 3.40, 3.20},
            {4.30, 4.20, 3.90, 3.80, 3.50, 3.40, 3.20, 3.80, 3.60, 3.40, 3.50, 3.40, 3.30, 3.00, 2.90, 2.70, 2.70, 2.40, 2.20, 1.80, 1.70, 1.20, 0.80, 1.40, 1.80, 2.00, 2.20, 2.60, 3.00, 3.10, 3.40},
            {4.50, 4.40, 4.10, 4.00, 3.70, 3.60, 3.40, 3.30, 3.80, 3.60, 3.30, 3.70, 3.20, 3.50, 3.30, 3.10, 2.90, 2.60, 2.27, 2.30, 2.10, 1.80, 1.40, 0.80, 1.20, 1.60, 1.80, 2.10, 2.60, 2.70, 3.00},
            {4.60, 4.50, 4.30, 4.10, 3.90, 3.70, 3.60, 3.40, 3.20, 3.80, 3.50, 3.30, 3.40, 3.30, 3.20, 3.40, 3.10, 2.90, 2.70, 2.60, 2.40, 2.00, 1.80, 1.20, 0.80, 1.30, 1.50, 1.80, 2.50, 2.70, 2.70},
            {4.80, 4.60, 4.40, 4.30, 4.00, 3.90, 3.70, 3.60, 3.40, 3.20, 3.70, 3.50, 3.70, 3.50, 3.40, 3.20, 3.40, 3.20, 2.60, 2.70, 2.80, 2.40, 2.00, 1.60, 1.30, 0.80, 1.10, 1.50, 2.20, 2.30, 2.70},
            {4.80, 4.70, 4.50, 4.40, 4.10, 4.00, 3.80, 3.60, 3.50, 3.30, 3.80, 3.60, 3.20, 3.60, 3.50, 3.30, 3.10, 3.30, 3.20, 2.80, 2.70, 2.60, 2.20, 1.80, 1.50, 1.10, 0.80, 1.30, 2.00, 2.20, 2.50},
            {5.00, 4.90, 4.60, 4.50, 4.30, 4.10, 3.90, 3.80, 3.60, 3.50, 3.20, 3.10, 3.50, 3.30, 3.80, 3.60, 3.40, 3.20, 3.10, 3.20, 3.00, 2.70, 2.60, 2.10, 1.80, 1.50, 1.30, 1.50, 1.70, 1.80, 2.10},
            {5.30, 5.20, 4.90, 4.80, 4.60, 4.40, 4.20, 4.10, 3.90, 3.80, 3.50, 3.40, 3.10, 3.70, 3.60, 3.50, 3.30, 3.70, 3.60, 3.30, 3.10, 3.30, 3.00, 2.60, 2.50, 2.20, 2.00, 1.70, 0.80, 1.10, 1.40},
            {5.40, 5.20, 5.00, 4.90, 4.60, 4.50, 4.30, 4.20, 4.00, 3.90, 3.60, 3.50, 3.20, 3.80, 3.70, 3.60, 3.40, 3.30, 3.70, 3.40, 3.30, 3.40, 3.10, 2.70, 2.70, 2.30, 2.20, 1.80, 1.10, 0.80, 1.20},
            {5.50, 5.40, 5.10, 5.00, 4.80, 4.60, 4.40, 4.30, 4.10, 4.00, 3.70, 3.60, 3.30, 3.20, 3.20, 3.80, 3.60, 3.40, 3.30, 3.60, 3.50, 3.20, 3.40, 3.00, 2.70, 2.70, 2.50, 2.10, 1.40, 1.20, 0.80},


        };

        public IDictionary<int, string> DictPackage
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    {0,"One Way"},
                    {1,"Return"}
                };
            }
            set { }
        }

        public IDictionary<int,string> DictFStation
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    {0,"Sungai Buloh" },
                    {1, "Kampung Selamat"},
                    {2, "Kwasa Damansara"},
                    {3, "Kwasa Sentral"},
                    {4, "Kota Damansara"},
                    {5, "Surian"},
                    {6, "Mutiara Damansara"},
                    {7, "Bandar Utama"},
                    {8, "Taman Tun Dr Ismail"},
                    {9, "Phileo Damansara"},
                    {10, "Pusat Bandar Damansara"},
                    {11, "Semantan"},
                    {12, "Muzium Negara"},
                    {13, "Pasar Seni"},
                    {14, "Merdeka"},
                    {15, "Bukit Bintang"},
                    {16, "Tun Razak Exchange"},
                    {17, "Cochrane"},
                    {18, "Maluri"},
                    {19, "Taman Pertama"},
                    {20, "Taman Midah"},
                    {21, "Taman Mutiara"},
                    {22, "Taman Connaught"},
                    {23, "Taman Suntex"},
                    {24, "Sri Raya"},
                    {25, "Bandar Tun Hussein Onn"},
                    {26, "Batu Sebelas Cheras"},
                    {27, "Bukit Dukung"},
                    {28, "Sungai Jernih"},
                    {29, "Stadium Kajang"},
                    {30, "Kajang"}
                };
            }
        }

        public IDictionary<int, string> DictTStation
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    {0,"Sungai Buloh" },
                    {1, "Kampung Selamat"},
                    {2, "Kwasa Damansara"},
                    {3, "Kwasa Sentral"},
                    {4, "Kota Damansara"},
                    {5, "Surian"},
                    {6, "Mutiara Damansara"},
                    {7, "Bandar Utama"},
                    {8, "Taman Tun Dr Ismail"},
                    {9, "Phileo Damansara"},
                    {10, "Pusat Bandar Damansara"},
                    {11, "Semantan"},
                    {12, "Muzium Negara"},
                    {13, "Pasar Seni"},
                    {14, "Merdeka"},
                    {15, "Bukit Bintang"},
                    {16, "Tun Razak Exchange"},
                    {17, "Cochrane"},
                    {18, "Maluri"},
                    {19, "Taman Pertama"},
                    {20, "Taman Midah"},
                    {21, "Taman Mutiara"},
                    {22, "Taman Connaught"},
                    {23, "Taman Suntex"},
                    {24, "Sri Raya"},
                    {25, "Bandar Tun Hussein Onn"},
                    {26, "Batu Sebelas Cheras"},
                    {27, "Bukit Dukung"},
                    {28, "Sungai Jernih"},
                    {29, "Stadium Kajang"},
                    {30, "Kajang"}
                };
            }
            set { }
        }

        public IDictionary<int, string> DictStatus 
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    {0, "None"},
                    {1,"Senior Citizen" },
                    {2, "Disabled"},
                    {3, "Student"}
                };
            }
            set { }
        }



    }

    public class LoginAcc
    {
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
       [Required]
        public string Phone { get; set; }

    }
}
