namespace AskJavra.ViewModels.Dto
{
    public class TagDto
    {
        public string Name { get; set; }
        public string? TagDescription { get; set; }
        public TagDto(string name, string? tagDescription)
        {
            Name = name;
            TagDescription = tagDescription;
        }
        public TagDto() { Name = "Nada"; }
    }
    public class TagAndIdDto : TagDto
    {
        public string Id { get; set; }
    }
    public class TagViewDto
    {
        public string Name { get; set; }
        public string? TagDescription { get; set; }
    }
}
