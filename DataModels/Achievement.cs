namespace UniCodeProject.API.DataModels
{
    public class Achievement
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string PictureUrl { get; set; } = null!;
    }
}
