namespace WixGenerator.Model {

    public sealed class WuFeature: WuCompositeEntity, IWuChildOf<WuProject>, IWuChildOf<WuFeature> {

        public enum VisibilityValue {
            Collapsed,
            Expanded,
            Hidden
        }

        public VisibilityValue Visibility { get; set; } = VisibilityValue.Expanded;

        private readonly string _title;
        private readonly string _description;

        public WuFeature(string title, string description) {

            ArgumentNullException.ThrowIfNullOrEmpty(title, nameof(title));

            _title = title;
            _description = String.IsNullOrEmpty(description) ? title : description;
        }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);

        public WuFeature Add<T>(T entity) where T: IWuChildOf<WuFeature> {

            AddEntity(entity);
            return this;
        }

        public string Title =>
            _title;

        public string Description =>
            _description;

        public IEnumerable<WuFeature> Features =>
            Entities.OfType<WuFeature>();

        public IEnumerable<WuComponentGroup> Groups =>
            Entities.OfType<WuComponentGroup>();
    }
}
