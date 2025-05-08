//Create a new model named ClassInformationTable. This class will represent a filtered version of the main class data. 
//It should include all necessary display fields except for the ID, which must still be part of the class for backend operations like edit, delete, and details
// — but it should not be shown in the table UI.

using System.ComponentModel.DataAnnotations;

namespace Projectw9.Models
{
    public class ClassInformationTable
    {
        public int Id { get; set; }
        
        [Display(Name = "Class Name")]
        public string ClassName { get; set; } // Class modelindeki "Name" ile uyumsuz
        
        [Display(Name = "Number of Students")]
        public int? StudentCount { get; set; } // Class modelindeki "StudentCount" ile uyumsuz
        
        [Display(Name = "Class Description")]
        public string Description { get; set; }

        
          // ClassInformationModel'den dönüşüm (GEREKSİZ, SİLİNEBİLİR)
        public static ClassInformationTable FromClassInformationModel(ClassInformationModel model)
        {
            return new ClassInformationTable
            {
                Id = model.Id,
                ClassName = model.ClassName,
                StudentCount = model.StudentCount,
                Description = model.Description
            };
        }

        // Class'tan dönüşüm (BU METODU DÜZELTİN)
        public static ClassInformationTable FromClassInformationModel(Class c)
        {
            return new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName, 
                StudentCount = c.StudentCount, 
                Description = c.Description
            };
        }
    }
}
