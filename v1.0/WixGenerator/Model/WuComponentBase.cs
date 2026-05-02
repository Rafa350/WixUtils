namespace WixGenerator.Model {

    public abstract class WuComponentBase: WuCompositeEntity, IWuChildOf<WuComponentGroup> {

        private readonly Guid _guid;

        public WuComponentBase() {

            _guid = Guid.NewGuid();
        }

        public WuComponentBase Add<T>(T entity) where T : IWuChildOf<WuComponentBase> {

            AddEntity(entity);
            return this;
        }

        public Guid Guid =>
            _guid;
    }
}
