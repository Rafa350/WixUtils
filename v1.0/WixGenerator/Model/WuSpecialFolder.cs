namespace WixGenerator.Model {

    public sealed class WuSpecialFolder: WuCompositeEntity, IWuChildOf<WuProject> {

        public enum Folder {
            ApplicationFolder,
            ComonFilesFolder,
            ProgramMenuFolder
        }

        public required Folder _folder { get; init; }

        public override void AcceptVisitor(WuVisitor visitor) => 
            visitor.Visit(this);
    }
}
