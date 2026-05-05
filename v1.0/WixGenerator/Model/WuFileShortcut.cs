namespace WixGenerator.Model {

    public sealed class WuFileShortcut: WuEntity, IWuChildOf<WuComponentBase> {

        public required string InstallDir { get; init; }
        public required string Title { get; init; }
        public required string TargetName { get; init; }
        public required string TargetDir { get; init; }
        public string Description { get; init; } = String.Empty;
        public string IconFile { get; set; } = String.Empty;

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
