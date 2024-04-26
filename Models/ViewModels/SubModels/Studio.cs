namespace Capstone.Models.ViewModels.SubModels
{
    public class Studio
    {
        public string Name {  get; set; }
        public bool IsMain {  get; set; }
        public bool IsAnimationStudio {  get; set; }

        public Studio() { }
        public Studio(string Name, bool IsMain, bool IsAnimationStudio) {
            this.Name = Name;
            this.IsMain = IsMain;
            this.IsAnimationStudio = IsAnimationStudio;
        }
    }
}
