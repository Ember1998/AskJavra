using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.Models.Root
{
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? TagDescription { get; set; }
        public Tag() { }
        public Tag(string name, string? tagDescription)
        {
            Name = name;
            TagDescription = tagDescription;
        }
        public Tag(int id, string name, string? tagDescription)
        {
            Id = id;
            Name = name;
            TagDescription = tagDescription;
        }
    }

}
