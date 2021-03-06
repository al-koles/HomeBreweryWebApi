// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace WebApplication3.Models
{
    public partial class ClientRecipe
    {
        public ClientRecipe()
        {
            Attempts = new HashSet<Attempt>();
        }

        public string ClientId { get; set; }
        public int RecipeId { get; set; }

        public virtual Client Client { get; set; }
        public virtual Recipe Recipe { get; set; }
        public virtual ICollection<Attempt> Attempts { get; set; }
    }
}