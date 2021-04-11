using System;
using System.ComponentModel.DataAnnotations;

namespace globelinkapi.Models
{
    public class UsersRequest{

        
        public string id { get; set; }

        //[Required]
        public string userName { get; set; }

        [Required]
        public string firstName { get; set; }

        [Required]
        public string lastName { get; set; }

        //[Required]
        public string password { get; set; }

        //[Required]
        public string status { get; set; }

        //[Required]
        public string createdBy { get; set; }

        //[Required]
        public string updatedBy { get; set; }

    }
}