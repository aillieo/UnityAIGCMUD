namespace AillieoUtils.MUD
{
    using AillieoUtils.AIGC;

    public class SectionData
    {
        public readonly TextProperty text = new TextProperty();
        public readonly ArrayProperty<string> choices = new ArrayProperty<string>();
        public readonly TextProperty imagePrompt = new TextProperty();
        public readonly TextureProperty image = new TextureProperty();
    }
}
