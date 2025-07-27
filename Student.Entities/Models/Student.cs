using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studentmanage.Entities.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required,Range(10, 30,ErrorMessage ="Should be age between 10 and 30")]
        public int Age { get; set; }

        public string Grade { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    }
}
