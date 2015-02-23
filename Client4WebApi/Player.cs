
using System;
using System.Xml.Serialization;

namespace Client4WebApi
{
    [Serializable]
    public class Player
    {
      
        public string Team { get; set; }
        public string TeamId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Age { get; set; } // Have it as DOB and then calculate years + days.
        public string Role { get; set; }
        
        public string IsCaptain { get; set; }
        //public int Runs { get; set; }
        //public int Wickets { get; set; }

        //public string Nationality { get; set; }
        public string ProfileUrl { get; set; }
        public string ImageUrl { get; set; }
 
    }
    public class Captain
    {
        [XmlAttribute]
        public string IsCaptain { get; set; }
    }

}
