namespace WixGenerator.Model {

    public sealed class WuComponentGroup: WuCompositeEntity, IWuChildOf<WuProject>, IWuChildOf<WuComponentGroup>, IWuChildOf<WuFeature> {

        private readonly string _name;

        public WuComponentGroup(string name) {

            _name = name;
        }

        public WuComponentGroup Add<T>(T entity) where T : IWuChildOf<WuComponentGroup> {

            AddEntity(entity);
            return this;
        }

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);

        public string Name =>
            _name;
    }
}
