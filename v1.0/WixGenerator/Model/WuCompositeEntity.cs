namespace WixGenerator.Model {

    public abstract class WuCompositeEntity: WuEntity {

        private readonly List<IWuEntity> _entities = new List<IWuEntity>();

        protected void AddEntity(IWuEntity entity) {

            _entities.Add(entity);
        }

        public IEnumerable<IWuEntity> Entities =>
            _entities;
    }
}
