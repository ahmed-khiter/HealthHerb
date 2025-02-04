﻿//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Threading.Tasks;

//namespace HealthHerb.Help
//{
//    public static class Countries
//    {
//        public static List<string> CountryList()
//        {
//            List<string> countries = new List<string>() {
//                            " Andorra",
//                            " United Arab Emirates              "," Afghanistan",
//                            " Antigua and Barbuda",
//                            " Albania",
//                            " Armenia",
//                            " Angola",
//                            " Argentina",
//                            " Austria",
//                            " Australia",
//                            " Azerbaijan",
//                            " Bosnia and Herzegovina",
//                            " Barbados",
//                            " Bangladesh",
//                            " Belgium",
//                            " Burkina Faso",
//                            " Bulgaria ",
//                            " Bahrain",
//                            " Burundi",
//                            " Benin ",
//                            " Brunei Darussalam",
//                            " Bolivia (Plurinational State of)",
//                            " Brazil",
//                            " Bahamas",
//                            " Bhutan",
//                            " Botswana",
//                            " Belarus",
//                            " Belize",
//                            " Canada",
//                            " Democratic Republic of the Congo",
//                            " Central African Republic",
//                            " Congo",
//                            " Switzerland",
//                            " Côte d'Ivoire",
//                            " Chile",
//                            " Cameroon",
//                            " China",
//                            " Colombia",
//                            " Costa Rica",
//                            " Cuba",
//                            " Cape Verde",
//                            " Cyprus",
//                            " Czech Republic",
//                            " Germany",
//                            " Djibouti",
//                            " Denmark",
//                            " Dominica",
//                            " Dominican Republic",
//                            " Algeria",
//                            " Ecuador",
//                            " Estonia",
//                            " Egypt",
//                            " Eritrea",
//                            " Spain",
//                            " Ethiopia",
//                            " Finland",
//                            " Fiji",
//                            " Micronesia (Federated States of)",
//                            " France",
//                            " Gabon",
//                            " United Kingdom of Great Britain anNorthern Ireland", 
//                            " Grenada",
//                            " Georgia",
//                            " Ghana",
//                            " Gambia",
//                            " Guinea",
//                            " Equatorial Guinea",
//                            " Greece",
//                            " Guatemala",
//                            " Guinea-Bissau",
//                            " Guyana",
//                            " Honduras",
//                            " Croatia",
//                            " Haiti",
//                            " Hungary",
//                            " Indonesia",
//                            " Ireland",
//                            " Israel",
//                            " India",
//                            " Iraq",
//                            " Iran (Islamic Republic of)",
//                            " Iceland",
//                            " Italy",
//                            " Jamaica",
//                            " Jordan",
//                            " Japan ",
//                            " Kenya ",
//                            " Kyrgyzstan",
//                            " Cambodia",
//                            " Kiribati",
//                            " Comoros  ",
//                            " Saint Kitts and Nevis ",
//                            " Democratic People's Republic of Koea",
//                            " Republic of Korea ",
//                            " Kuwait  ",
//                            " Kazakhstan                        ",
//                            " Lao People's Democratic Republic",
//                            " Lebanon ",
//                            " Saint Lucia",
//                            " Liechtenstein ",
//                            " Sri Lanka ",
//                            " Liberia  ",
//                            " Lesotho   ",
//                            " Lithuania ",
//                            " Luxembourg  ",
//                            " Latvia",
//                            " Libyan Arab Jamahiriya",
//                            " Morocco  ",
//                            " Monaco",
//                            " Republic of Moldova",
//                            " Montenegro",
//                            " Madagascar",
//                            " Marshall Islands",
//                            " Mali",
//                            " Myanmar ",
//                            " Mongolia ",
//                            " Mauritania",
//                            " Malta ",
//                            " Mauritius ",
//                            " Maldives",
//                            " Malawi ",
//                            " Mexico ",
//                            " Malaysia ",
//                            " Mozambique",
//                            " Namibia ",
//                            " Niger  ",
//                            " Nigeria ",
//                            " Nicaragua",
//                            " Netherlands",
//                            " Norway ",
//                            " Nepal ",
//                            " Nauru ",
//                            " New Zealand                       ",
//                            " Oman ",
//                            " Panama",
//                            " Peru",
//                            " Papua New Guinea",
//                            " Philippines",
//                            " Pakistan ",
//                            " Poland ",
//                            " Portugal",
//                            " Palau  ",
//                            " Paraguay ",
//                            " Qatar",
//                            " Romania",
//                            " Serbia ",
//                            " Russian Federation",
//                            " Rwanda ",
//                            " Saudi Arabia ",
//                            " Solomon Islands  ",
//                            " Seychelles",
//                            " Sudan ",
//                            " Sweden ",
//                            " Singapore ",
//                            " Slovenia ",
//                            " Slovakia ",
//                            " Sierra Leone ",
//                            " San Marino ",
//                            " Senegal",
//                            " Somalia ",
//                            " Suriname ",
//                            " South Sudan ",
//                            " Sao Tome and Principe",
//                            " El Salvador   ",
//                            " Syrian Arab Republic ",
//                            " Swaziland ",
//                            " Chad ",
//                            " Togo ",
//                            " Thailand ",
//                            " Tajikistan ",
//                            " Timor-Leste ",
//                            " Turkmenistan",
//                            " Tunisia ",
//                            " Tonga ",
//                            " Turkey ",
//                            " Trinidad and Tobago",
//                            " Tuvalu",
//                            " United Republic of Tanzania",
//                            " Ukraine",
//                            " Uganda",
//                            " United States of America ",
//                            " Uruguay",
//                            " Uzbekistan",
//                            " Saint Vincent and the Grenadines",
//                            " Venezuela (Bolivarian Republic of)",
//                            " Viet Nam ",
//                            " Vanuatu",
//                            " Samoa ",
//                            " Yemen ",
//                            " South Africa ",
//                            " Zambia",
//                            " Zimbabwe",};

//            //Creating list
//            List<string> CultureList = new List<string>();

//            //getting  the specific  CultureInfo from CultureInfo class
//            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

//            foreach (CultureInfo getCulture in getCultureInfo)
//            {
//                //creating the object of RegionInfo class
//                RegionInfo GetRegionInfo = new RegionInfo(getCulture.LCID);
//                //adding each county Name into the arraylist
//                if (!(CultureList.Contains(GetRegionInfo.EnglishName)))
//                {
//                    CultureList.Add(GetRegionInfo.EnglishName);
//                }
//            }
//            //sorting array by using sort method to get countries in order
//            CultureList.Sort();
//            //returning country list
//            return CultureList;
//        }

//    }
//}
