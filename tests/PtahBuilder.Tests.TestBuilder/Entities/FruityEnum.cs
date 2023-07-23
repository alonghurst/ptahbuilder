using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtahBuilder.Tests.TestBuilder.Entities
{
    public enum FruityEnum
    {
        [Display(Description = "Used to indicate that the item is not to be considered fruity")]
        NotFruity = 0,
        ABitFruity = 1,
        VeryFruity = 2
    }
}
